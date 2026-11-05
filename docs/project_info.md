# Архитектура и описание проекта SharpFFI

## Главная идея и философия
**SharpFFI** решает проблему сложного interop-взаимодействия между C/C++ и C#. Существующие генераторы часто требуют сложных и нестандартных конфигурационных файлов, а анализ C/C++ кода — это нестабильный и тяжелый процесс. 

SharpFFI «переворачивает» этот подход:
* **C# как Source of Truth (источник истины)**: Анализируется не C/C++ код, а сам C# код. Вся конфигурация задается через C#-атрибуты (`[FixedString]`, `[FixedArray]`, `[FixedList]`, `[Handle]`), что избавляет от боли внешних конфигурационных файлов.
* **Максимальная производительность и Zero-Allocation**: Упор на ручной контроль, маппинг памяти 1 к 1 и отсутствие скрытых аллокаций мусора (GC).
* **Безопасность и предсказуемость**: Использование safe buffer шаблонов вместо небезопасных fixed-буферов с висячими указателями, валидация сигнатур на уровне компилятора через собственные Roslyn-анализаторы.
* **Лучше написать чуть больше кода в C#, но получить 100% контроль** и прозрачный API, чем бороться с черным ящиком чужого генератора.

---

## Структура решения
Проект разбит на модули со строгим разделением ответственности:

1. **`SharpFFI.Annotations`**
   * Содержит легковесные декларативные атрибуты для разметки структур: `[FixedString]`, `[FixedArray]`, `[FixedList]`, `[Handle]`.
   * Не содержит логики рантайма, подключается без лишних зависимостей.

2. **`SharpFFI.Primitives`**
   * Высокопроизводительные типы (примитивы) для маппинга нативных данных в C#.
   * **Строки**: Полная система zero-allocation строк, разделенная по кодировкам (`Utf8` и `Utf16`):
     - `Utf8CString` / `ReadOnlyUtf8CString`, `Utf16CString` / `ReadOnlyUtf16CString`: тонкие non-owning указатели на нуль-терминированные строки.
     - `Utf8StringView` / `ReadOnlyUtf8StringView`, `Utf16StringView` / `ReadOnlyUtf16StringView`: `ref struct` представления со спанами и расширенными методами (`GetLength`, `GetHashCode`, `IndexOf`, `Split`, `Trim`).
     - `NativeUtf8CString` / `NativeUtf16CString`: строки, владеющие памятью в нативной куче (`NativeMemory` / `Marshal.AllocHGlobal`).
     - `Utf8ScopedCString` / `Utf16ScopedCString`: стековый маршалинг входных строк через эвристику `stackalloc` с автоматическим fallback на нативную кучу.
     - `Utf8CharEnumerator` / `Utf16CharEnumerator`: безаллокационные перечислители символов (`Rune`).
   * **Булевы типы**: Примитивы с фиксированным бинарным размером (`Bool8`, `Bool32`) для корректного маппинга `bool` и `BOOL`.

3. **`SharpFFI.Generators.Core`**
   * Фундамент для инкрементальных генераторов и анализаторов:
     - Описания диагностик `DiagnosticDescriptors` (`FFI0001`–`FFI0004` со статусом `Error`).
     - Анализаторы модификаторов типов `TypeModifierAnalyzer` (правила `Required`, `Forbidden`, `Allowed`).
     - Анализатор имен структур `FixedStringNameAnalyzer`.
     - Утилиты построения кода `SourceCodeBuilder` и расширения символов `NamedTypeSymbolExtensions`.

4. **`SharpFFI.Generators.CodeFixes`**
   * Roslyn CodeFix-провайдеры (например, `TypeModifierCodeFixProvider` для автоматического добавления `partial` или удаления запрещенных модификаторов).

5. **`SharpFFI.Generators.C`**
   * Инкрементальные генераторы исходного кода C#:
     - `FixedStringGenerator`: генерация нуль-терминированных UTF-8 строк фиксированного размера с безопасным внутренним представлением буфера (`Buffer`), свойствами `Capacity`, `Length`, методами `AsSpan()`, `AsView()`, конструкторами и операторами сравнения.
     - `FixedArrayGenerator`: генерация фиксированных generic-массивов (`[FixedArray(size)]`).
     - `FixedListGenerator`: генерация generic-списков фиксированной емкости (`[FixedList(capacity)]`) со счетчиком `_count`, методами `Add`, `RemoveAt`, `Clear`, индексатором и поддержкой `IEnumerable<T>`.
     - `HandleGenerator`: генерация типобезопасных дескрипторов (Opaque Handles) со свойством `Null`, конструкторами и проверками валидности.

6. **`SharpFFI.Generators.Cpp`**
   * Расширение генерации для сценариев C++.

7. **`SharpFFI.Demo`**
   * Консольное приложение, демонстрирующее сценарии использования всех примитивов и сгенерированных типов на рантаймах `netcoreapp3.1`, `net6.0`, `net10.0`.

8. **`SharpFFI.Benchmarks`**
   * Комплексный набор бенчмарков (BenchmarkDotNet) для замера скорости (CPU) и распределения памяти (GC) примитивов на различных платформах (`net6.0`, `net8.0`, `net10.0`).

9. **`SharpFFI.Primitives.Tests`**
   * Набор модульных тестов с полным покрытием функциональности примитивов.

---

## Roadmap и статус задач

### Базовые типы (Primitives & Runtime)
* [x] Поддержка UTF-8 и UTF-16 примитивов (View, CString, NativeCString, ScopedCString, Enumerator).
* [x] Публичные статические методы `GetLength` и `GetHashCode` в `Utf8StringView` и `Utf16StringView`.
* [ ] Реализовать поддержку современного форматирования `IFormattable`, `ISpanFormattable`, `IUtf8SpanFormattable` (см. [TODO.md](TODO.md#6-поддержка-современного-форматирования-строк-iformattable-ispanformattable-iutf8spanformattable)).
* [ ] Поддержка массивов строк.
* [ ] Обеспечить полную поддержку NativeAOT.
* [ ] Интеграция со стандартным генератором marshaller'ов (`LibraryImport`).

### Кодогенерация (Source Generators)
* [x] Генерация фиксированных строк (`[FixedString]`) с безопасным внутренним представлением без dangling pointer.
* [x] Генерация фиксированных массивов (`[FixedArray]`).
* [x] Генерация фиксированных списков (`[FixedList]`).
* [x] Генерация Opaque Handles (`[Handle]`) со свойством `Null`.
* [x] Вынос моделей генерации в чистые DTO без удержания `Location` / `SyntaxTree`.
* [ ] Поддержка вложенных структур (Nested Types) через `GetContainingTypes()`.
* [ ] Валидация дженерик-арности для `[FixedList]` и `[FixedArray]` (`FFI0005`).
* [ ] Запрет модификатора `readonly` для `[FixedList]` (`FFI0006`).
* [ ] Настраиваемый тип дескриптора в `[Handle]` (базовый `nint`, `uint`, `ulong`).
* [ ] Настраиваемый тип счетчика длины (`byte`, `ushort`) для `[FixedList]`.

### Анализаторы (Roslyn Analyzers)
* [x] Диагностика имени фиксированной строки `FFI0001` (`Error`).
* [x] Диагностика запрещенных модификаторов `FFI0002` (`Error`).
* [x] Диагностика обязательных модификаторов `FFI0003` (`Error`).
* [x] Диагностика partial для внешних типов `FFI0004` (`Error`).
* [x] CodeFixProvider для автоматического исправления модификаторов.
* [ ] Анализатор контроля времени жизни и утечек памяти для `ScopedCString` (требование `using` или явного `Dispose`).
