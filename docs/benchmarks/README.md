# Эталонные замеры производительности SharpFFI.Primitives (BenchmarkDotNet)

В данной директории сохранены результаты полного эталонного прогона бенчмарков производительности (CPU) и потребления памяти (GC) библиотеки `SharpFFI.Primitives`.

---

## 1. Метаданные тестового прогона

| Параметр | Значение |
| :--- | :--- |
| **Дата и время тестирования** | 16 октября 2026 г., 20:46 — 23:08 (MSK / UTC+3) |
| **Длительность прогона** | ~2 часа 22 минуты (полная матрица всех параметров и рантаймов) |
| **Git Commit Hash** | `1093ecdec0fc08f34cd0ee80c1aa35d6ac5d414f` |
| **Git Ветка** | `dev` |
| **Сообщение коммита** | `Добавлен проект с бенчмарками примитивов` |
| **Состояние репозитория** | Все 19 примитивов библиотеки `SharpFFI.Primitives` с 100% покрытием unit-тестами |

---

## 2. Аппаратное и программное окружение

```text
BenchmarkDotNet v0.15.8, Windows 11 (10.0.26300.9711)
AMD Ryzen 7 9700X 3.80GHz, 1 CPU, 16 logical and 8 physical cores
.NET SDK 10.0.102
  [Host]             : .NET 8.0.32 (8.0.32, 8.0.3226.50405), X64 RyuJIT x86-64-v4
  .NET 10.0          : .NET 10.0.13 (10.0.13, 10.0.1326.50504), X64 RyuJIT x86-64-v4
  .NET 8.0           : .NET 8.0.32 (8.0.32, 8.0.3226.50405), X64 RyuJIT x86-64-v4
  .NET 6.0           : .NET 6.0.36 (6.0.36, 6.0.3624.51421), X64 RyuJIT x86-64-v3
  .NET Framework 4.8 : .NET Framework 4.8 (4.8.10008.0), X64 RyuJIT VectorSize=256
```

---

## 3. Наборы бенчмарков и ссылки на отчеты

Все детальные отчеты лежат в подпапке [`results/`](results/):

| Сьют бенчмарков | Что тестирует | Отчет GitHub (.md) | Интерактивный (.html) | Сырые данные (.csv) |
| :--- | :--- | :---: | :---: | :---: |
| **`ScopedCStringBenchmarks`** | Маршалинг входных строк через `stackalloc` буфер, однопроходную эвристику vs fallback на нативную кучу vs `Marshal.StringToCoTaskMemUTF8` | [MD](results/SharpFFI.Benchmarks.ScopedCStringBenchmarks-report-github.md) | [HTML](results/SharpFFI.Benchmarks.ScopedCStringBenchmarks-report.html) | [CSV](results/SharpFFI.Benchmarks.ScopedCStringBenchmarks-report.csv) |
| **`NativeCStringBenchmarks`** | Жизненный цикл строк в нативной куче (`NativeMemory.Alloc`/`Free` vs `AllocHGlobal`), замер аллокаций и усечение хвоста при наличии `\0` | [MD](results/SharpFFI.Benchmarks.NativeCStringBenchmarks-report-github.md) | [HTML](results/SharpFFI.Benchmarks.NativeCStringBenchmarks-report.html) | [CSV](results/SharpFFI.Benchmarks.NativeCStringBenchmarks-report.csv) |
| **`NullTerminatedScanAndDecodeBenchmarks`** | Сканирование нуль-терминатора (`strlen`/`wcslen`), декодирование строк с неизвестной vs известной длиной, сравнение с `Marshal.PtrToStringUTF8` | [MD](results/SharpFFI.Benchmarks.NullTerminatedScanAndDecodeBenchmarks-report-github.md) | [HTML](results/SharpFFI.Benchmarks.NullTerminatedScanAndDecodeBenchmarks-report.html) | [CSV](results/SharpFFI.Benchmarks.NullTerminatedScanAndDecodeBenchmarks-report.csv) |
| **`Utf8CharEnumeratorBenchmarks`** | Декодирование и посимвольный обход UTF-8 без аллокаций (ASCII, Cyrillic, Emoji) против вызова `ToString()` и `ArrayPool` | [MD](results/SharpFFI.Benchmarks.Utf8CharEnumeratorBenchmarks-report-github.md) | [HTML](results/SharpFFI.Benchmarks.Utf8CharEnumeratorBenchmarks-report.html) | [CSV](results/SharpFFI.Benchmarks.Utf8CharEnumeratorBenchmarks-report.csv) |
| **`HashingAndEqualityBenchmarks`** | Вычисление хэша алгоритмом Marvin (границы 3, 6, 64, 1024 байт), побайтовое сравнение `ReadOnlyUtf8StringView` и `ReadOnlyUtf8CString` | [MD](results/SharpFFI.Benchmarks.HashingAndEqualityBenchmarks-report-github.md) | [HTML](results/SharpFFI.Benchmarks.HashingAndEqualityBenchmarks-report.html) | [CSV](results/SharpFFI.Benchmarks.HashingAndEqualityBenchmarks-report.csv) |

---

## 4. Главные выводы по результатам

1. **Zero-Allocation при маршалинге строк (`Utf8ScopedCString`)**:
   - При использовании стекового буфера (`stackalloc byte[256]`) передача коротких и средних строк занимает **~3.3–5.9 нс** на .NET 10.0 при **0 байт аллокаций в куче GC**.
   - Стандартный `Marshal.StringToCoTaskMemUTF8` работает в **4–10 раз медленнее** (24–60 нс) и на старых средах выделяет неуправляемую/управляемую память.
2. **Нулевые аллокации при итерации по UTF-8 (`Utf8CharEnumerator`)**:
   - Обход UTF-8 байтов напрямую через `foreach (char c in view)` полностью исключает аллокации (0 байт), тогда как конвертация `view.ToString()` аллоцирует строки в куче GC и тратит дополнительное процессорное время на создание `System.String`.
3. **Эффективность кэширования длины (`NativeUtf8CString`)**:
   - Наличие известного свойства `Length` позволяет декодировать строку в `string` без предварительного векторного сканирования `\0` (`strlen`), что дает стабильный выигрыш по времени при многократных обращениях.
4. **Прогресс JIT и BCL от .NET Framework 4.8 к .NET 10.0**:
   - Переход с `net48` на современные рантаймы (`net8.0` и `net10.0`) дает прирост скорости в **2–5 раз** на строковых операциях за счет векторных инструкций RyuJIT (AVX2/AVX-512) и полифиллов `NativeMemory`.
