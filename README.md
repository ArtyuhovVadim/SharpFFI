# SharpFFI

[![.NET](https://img.shields.io/badge/.NET-netstandard2.1%20%7C%20net6.0%20%7C%20net8.0%20%7C%20net10.0-blue.svg)](https://dotnet.microsoft.com/)
[![License](https://img.shields.io/badge/License-MIT-green.svg)](LICENSE)

**SharpFFI** — это высокопроизводительная модульная экосистема для бесшовного, типобезопасного и безаллокационного (Zero-Allocation) взаимодействия между **C#** и нативным кодом (**C / C++**).

В отличие от традиционных подходов, требующих сложных конфигурационных файлов или анализа нестабильных C/C++ заголовков, SharpFFI использует **C# как Source of Truth (источник истины)**: вы размечаете структуры лаконичными C#-атрибутами, а инкрементальные Roslyn-генераторы создают весь необходимый бойлерплейт для маппинга памяти 1:1.

---

## Ключевые возможности

* 🚀 **Zero-Allocation маршалинг**: Прямая работа с нативными буферами через `Span<T>` и `ReadOnlySpan<T>` без выделения промежуточных объектов в управляемой куче (GC).
* 🛡️ **Безопасное внутреннее представление**: Защита от dangling pointer (висячих указателей) благодаря safe buffer абстракциям вместо небезопасных fixed-буферов.
* ⚡ **Инкрементальные генераторы Roslyn (IIncrementalGenerator)**: Мгновенная кодогенерация с поддержкой кэширования и минимальным потреблением памяти в IDE.
* 🔍 **Встроенные Roslyn-анализаторы и CodeFixes**: Проверка правил модификаторов типов и корректности объявления структур на этапе компиляции с автоматическими исправлениями.
* 📦 **Поддержка UTF-8 и UTF-16**: Полный спектр строковых примитивов для работы с C-строками (`View`, `CString`, `NativeCString`, `ScopedCString`, `CharEnumerator`).
* 🎯 **Широкая поддержка рантаймов**: Таргетинг на `netstandard2.1`, `net6.0`, `net8.0` и современный `net10.0`.

---

## Архитектура решения

Решение разбито на независимые проекты со строгим разделением ответственности:

```text
SharpFFI/
├── src/
│   ├── SharpFFI.Annotations/          # Легковесные атрибуты ([FixedString], [FixedArray], [FixedList], [Handle])
│   ├── SharpFFI.Primitives/           # Высокопроизводительные примитивы (UTF-8/UTF-16 строки, Bool8/Bool32)
│   ├── SharpFFI.Generators.Core/      # Модели генераторов, анализаторы FFI0001–FFI0004, инфраструктура
│   ├── SharpFFI.Generators.CodeFixes/ # Автоматические CodeFix-провайдеры для Visual Studio и Rider
│   ├── SharpFFI.Generators.C/         # C# генераторы: FixedString, FixedArray, FixedList, Handle
│   ├── SharpFFI.Generators.Cpp/       # Расширение генераторов под C++
│   ├── SharpFFI.Demo/                 # Демонстрационное консольное приложение
│   ├── SharpFFI.Benchmarks/           # Эталонные замеры производительности BenchmarkDotNet
│   └── SharpFFI.Primitives.Tests/     # Комплект Unit-тестов со 100% покрытием
└── docs/                              # Документация, архитектурные отчеты и планы
```

---

## Примеры использования

### 1. Фиксированные строки (`[FixedString]`)

Генерирует нуль-терминированную UTF-8 строку фиксированного размера с безопасным внутренним буфером, свойствами `Capacity`, `Length`, методами `AsSpan()`, `AsView()` и операторами сравнения:

```csharp
using SharpFFI.Annotations;

// Имя должно соответствовать паттерну: Utf8FixedCString<Размер> или ReadOnlyUtf8FixedCString<Размер>
[FixedString]
public partial struct Utf8FixedCString32 { }

// Использование:
var str = new Utf8FixedCString32("Hello, Native!");
Console.WriteLine($"Capacity: {Utf8FixedCString32.Capacity}"); // 32 байта
Console.WriteLine($"Length: {str.Length}");                     // 14 байт
Console.WriteLine(str.ToString());                             // "Hello, Native!"

// Безаллокационный доступ через спаны и вьюшки:
ReadOnlySpan<byte> span = str.AsReadOnlySpan();
Utf8StringView view = str.AsView();
```

### 2. Фиксированные массивы (`[FixedArray]`)

Генерирует generic-массив фиксированного размера с бинарным маппингом 1:1:

```csharp
using SharpFFI.Annotations;

[FixedArray(16)]
public partial struct MatrixRow16<T> where T : unmanaged { }

// Использование:
var row = new MatrixRow16<float>();
row[0] = 1.0f;
Span<float> span = row.AsSpan();
```

### 3. Списки фиксированной емкости (`[FixedList]`)

Генерирует zero-allocation список с фиксированной емкостью, отслеживанием текущей длины `Count`, методами `Add`, `RemoveAt`, `Clear` и поддержкой `IEnumerable<T>`:

```csharp
using SharpFFI.Annotations;

[FixedList(32)]
public partial struct ArrayVec<T> where T : unmanaged { }

// Использование:
var list = new ArrayVec<int>();
list.Add(42);
list.Add(100);

Console.WriteLine($"Count: {list.Count}, Capacity: {list.Capacity}");
foreach (var item in list)
{
    Console.WriteLine(item);
}
```

### 4. Типобезопасные дескрипторы (`[Handle]`)

Генерирует Opaque Handle с проверками валидности, свойством `Null` и операторами приведения:

```csharp
using SharpFFI.Annotations;

[Handle]
public readonly partial struct GlTextureHandle { }

// Использование:
GlTextureHandle handle = GlTextureHandle.Null;
if (handle.IsNull)
{
    // Обработка неинициализированного дескриптора
}
```

### 5. Строковые примитивы `SharpFFI.Primitives`

* **`Utf8StringView` / `Utf16StringView`**: безаллокационные `ref struct` представления с поддержкой срезов, поиска и статических методов `GetLength` и `GetHashCode`.
* **`NativeUtf8CString` / `NativeUtf16CString`**: нуль-терминированные строки в нативной куче с автоматическим освобождением памяти при `Dispose()`.
* **`Utf8ScopedCString` / `Utf16ScopedCString`**: маршалинг в стековый буфер `stackalloc` с fallback на нативную кучу при переполнении:

```csharp
using SharpFFI.Primitives.Utf8;

// Стековый маршалинг: выделение на стеке, если строка помещается, иначе в нативной куче
using var scoped = new Utf8ScopedCString("Текст для нативного API");
NativeApi.CallFunction(scoped.Pointer);
```

---

## Диагностики Roslyn (FFI0001–FFI0004)

Все ошибки кодогенерации валидируются встроенными анализаторами на уровне компиляции:

| ID | Название | Тип | Описание |
| :--- | :--- | :---: | :--- |
| **`FFI0001`** | `InvalidNameError` | **Error** | Имя структуры с `[FixedString]` должно соответствовать `Utf8FixedCString<N>` или `ReadOnlyUtf8FixedCString<N>`. |
| **`FFI0002`** | `ForbiddenModifierError` | **Error** | Запрещенный модификатор для аннотированного типа (например, `ref` для `[FixedString]`). |
| **`FFI0003`** | `RequiredModifierMissingError` | **Error** | Отсутствует обязательный модификатор (например, `partial`). Поддерживается автоисправление CodeFix. |
| **`FFI0004`** | `EnclosingTypeMustBePartialError` | **Error** | Внешний охватывающий тип для вложенной структуры должен быть объявлен со словом `partial`. |

---

## Производительность и бенчмарки

Библиотека `SharpFFI.Primitives` протестирована полным набором эталонных тестов производительности (BenchmarkDotNet) на рантаймах .NET 6.0, .NET 8.0 и .NET 10.0:

* **0 байт аллокаций в управляемой куче (GC 0 Gen0/Gen1/Gen2)** при сканировании нуль-терминатора, перечислении символов `Rune` и хешировании алгоритмом Marvin.
* В разы меньший оверхед по сравнению со стандартным `Marshal.PtrToStringUTF8` и `Marshal.StringToCoTaskMemUTF8`.
* Подробные отчеты с интерактивными графиками и сырыми данными доступны в папке [`docs/benchmarks/`](docs/benchmarks/README.md).

---

## Дополнительная документация

* 📋 [Список задач и статус реализации (TODO)](docs/TODO.md)
* 📐 [Подробный анализ архитектуры проекта](docs/project_info.md)
* 📊 [Результаты эталонных замеров производительности](docs/benchmarks/README.md)

---

## Лицензия

Проект распространяется под лицензией MIT.
