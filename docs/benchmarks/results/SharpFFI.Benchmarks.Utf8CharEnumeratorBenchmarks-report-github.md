```

BenchmarkDotNet v0.15.8, Windows 11 (10.0.26300.9711)
AMD Ryzen 7 9700X 3.80GHz, 1 CPU, 16 logical and 8 physical cores
.NET SDK 10.0.102
  [Host]             : .NET 8.0.32 (8.0.32, 8.0.3226.50405), X64 RyuJIT x86-64-v4
  .NET 10.0          : .NET 10.0.13 (10.0.13, 10.0.1326.50504), X64 RyuJIT x86-64-v4
  .NET 6.0           : .NET 6.0.36 (6.0.36, 6.0.3624.51421), X64 RyuJIT x86-64-v3
  .NET 8.0           : .NET 8.0.32 (8.0.32, 8.0.3226.50405), X64 RyuJIT x86-64-v4
  .NET Framework 4.8 : .NET Framework 4.8 (4.8.10008.0), X64 RyuJIT VectorSize=256


```
| Method                                | Job                | Runtime            | Length | Charset    | Mean        | Error     | StdDev     | Median      | Ratio | RatioSD | Gen0   | Allocated | Alloc Ratio |
|-------------------------------------- |------------------- |------------------- |------- |----------- |------------:|----------:|-----------:|------------:|------:|--------:|-------:|----------:|------------:|
| **Foreach_Utf8StringView_CharEnumerator** | **.NET 10.0**          | **.NET 10.0**          | **128**    | **Ascii**      |    **51.71 ns** |  **1.014 ns** |   **1.041 ns** |    **51.26 ns** |  **0.39** |    **0.01** |      **-** |         **-** |          **NA** |
| Foreach_Utf8StringView_CharEnumerator | .NET 6.0           | .NET 6.0           | 128    | Ascii      |   126.51 ns |  1.429 ns |   1.193 ns |   125.86 ns |  0.97 |    0.03 |      - |         - |          NA |
| Foreach_Utf8StringView_CharEnumerator | .NET 8.0           | .NET 8.0           | 128    | Ascii      |    49.08 ns |  1.005 ns |   1.196 ns |    48.26 ns |  0.37 |    0.01 |      - |         - |          NA |
| Foreach_Utf8StringView_CharEnumerator | .NET Framework 4.8 | .NET Framework 4.8 | 128    | Ascii      |   131.12 ns |  2.565 ns |   3.336 ns |   129.32 ns |  1.00 |    0.03 |      - |         - |          NA |
|                                       |                    |                    |        |            |             |           |            |             |       |         |        |           |             |
| Baseline_ToString_ThenForeachChar     | .NET 10.0          | .NET 10.0          | 128    | Ascii      |    38.41 ns |  0.385 ns |   0.321 ns |    38.38 ns |  0.52 |    0.01 | 0.0167 |     280 B |        0.97 |
| Baseline_ToString_ThenForeachChar     | .NET 6.0           | .NET 6.0           | 128    | Ascii      |    45.34 ns |  0.340 ns |   0.284 ns |    45.28 ns |  0.61 |    0.01 | 0.0167 |     280 B |        0.97 |
| Baseline_ToString_ThenForeachChar     | .NET 8.0           | .NET 8.0           | 128    | Ascii      |    35.19 ns |  0.690 ns |   0.611 ns |    35.00 ns |  0.48 |    0.01 | 0.0167 |     280 B |        0.97 |
| Baseline_ToString_ThenForeachChar     | .NET Framework 4.8 | .NET Framework 4.8 | 128    | Ascii      |    73.89 ns |  1.052 ns |   0.984 ns |    73.42 ns |  1.00 |    0.02 | 0.0459 |     289 B |        1.00 |
|                                       |                    |                    |        |            |             |           |            |             |       |         |        |           |             |
| Baseline_ArrayPool_EncodingGetChars   | .NET 10.0          | .NET 10.0          | 128    | Ascii      |    32.69 ns |  0.624 ns |   0.584 ns |    32.47 ns |  0.43 |    0.01 |      - |         - |          NA |
| Baseline_ArrayPool_EncodingGetChars   | .NET 6.0           | .NET 6.0           | 128    | Ascii      |    46.82 ns |  0.393 ns |   0.307 ns |    46.69 ns |  0.62 |    0.01 |      - |         - |          NA |
| Baseline_ArrayPool_EncodingGetChars   | .NET 8.0           | .NET 8.0           | 128    | Ascii      |    40.18 ns |  0.317 ns |   0.281 ns |    40.04 ns |  0.53 |    0.01 |      - |         - |          NA |
| Baseline_ArrayPool_EncodingGetChars   | .NET Framework 4.8 | .NET Framework 4.8 | 128    | Ascii      |    76.14 ns |  1.431 ns |   1.339 ns |    75.66 ns |  1.00 |    0.02 |      - |         - |          NA |
|                                       |                    |                    |        |            |             |           |            |             |       |         |        |           |             |
| **Foreach_Utf8StringView_CharEnumerator** | **.NET 10.0**          | **.NET 10.0**          | **128**    | **Cyrillic**   |    **96.63 ns** |  **0.415 ns** |   **0.324 ns** |    **96.57 ns** |  **0.46** |    **0.00** |      **-** |         **-** |          **NA** |
| Foreach_Utf8StringView_CharEnumerator | .NET 6.0           | .NET 6.0           | 128    | Cyrillic   |   177.93 ns |  3.377 ns |   3.316 ns |   176.24 ns |  0.84 |    0.02 |      - |         - |          NA |
| Foreach_Utf8StringView_CharEnumerator | .NET 8.0           | .NET 8.0           | 128    | Cyrillic   |   130.88 ns |  2.574 ns |   2.643 ns |   130.12 ns |  0.62 |    0.01 |      - |         - |          NA |
| Foreach_Utf8StringView_CharEnumerator | .NET Framework 4.8 | .NET Framework 4.8 | 128    | Cyrillic   |   210.69 ns |  2.423 ns |   2.266 ns |   210.12 ns |  1.00 |    0.01 |      - |         - |          NA |
|                                       |                    |                    |        |            |             |           |            |             |       |         |        |           |             |
| Baseline_ToString_ThenForeachChar     | .NET 10.0          | .NET 10.0          | 128    | Cyrillic   |   109.17 ns |  1.204 ns |   1.068 ns |   108.58 ns |  0.37 |    0.00 | 0.0167 |     280 B |        0.97 |
| Baseline_ToString_ThenForeachChar     | .NET 6.0           | .NET 6.0           | 128    | Cyrillic   |   117.33 ns |  1.576 ns |   1.231 ns |   117.35 ns |  0.39 |    0.00 | 0.0167 |     280 B |        0.97 |
| Baseline_ToString_ThenForeachChar     | .NET 8.0           | .NET 8.0           | 128    | Cyrillic   |   105.88 ns |  1.763 ns |   1.649 ns |   105.09 ns |  0.36 |    0.01 | 0.0167 |     280 B |        0.97 |
| Baseline_ToString_ThenForeachChar     | .NET Framework 4.8 | .NET Framework 4.8 | 128    | Cyrillic   |   297.97 ns |  2.208 ns |   1.843 ns |   298.77 ns |  1.00 |    0.01 | 0.0458 |     289 B |        1.00 |
|                                       |                    |                    |        |            |             |           |            |             |       |         |        |           |             |
| Baseline_ArrayPool_EncodingGetChars   | .NET 10.0          | .NET 10.0          | 128    | Cyrillic   |    74.46 ns |  1.262 ns |   1.180 ns |    73.74 ns |  0.46 |    0.01 |      - |         - |          NA |
| Baseline_ArrayPool_EncodingGetChars   | .NET 6.0           | .NET 6.0           | 128    | Cyrillic   |    83.47 ns |  1.668 ns |   1.393 ns |    82.64 ns |  0.52 |    0.01 |      - |         - |          NA |
| Baseline_ArrayPool_EncodingGetChars   | .NET 8.0           | .NET 8.0           | 128    | Cyrillic   |    81.46 ns |  0.131 ns |   0.102 ns |    81.47 ns |  0.50 |    0.01 |      - |         - |          NA |
| Baseline_ArrayPool_EncodingGetChars   | .NET Framework 4.8 | .NET Framework 4.8 | 128    | Cyrillic   |   161.48 ns |  3.090 ns |   2.890 ns |   160.73 ns |  1.00 |    0.02 |      - |         - |          NA |
|                                       |                    |                    |        |            |             |           |            |             |       |         |        |           |             |
| **Foreach_Utf8StringView_CharEnumerator** | **.NET 10.0**          | **.NET 10.0**          | **128**    | **EmojiHeavy** |   **134.88 ns** |  **2.242 ns** |   **2.097 ns** |   **134.07 ns** |  **0.59** |    **0.01** |      **-** |         **-** |          **NA** |
| Foreach_Utf8StringView_CharEnumerator | .NET 6.0           | .NET 6.0           | 128    | EmojiHeavy |   177.60 ns |  1.836 ns |   1.533 ns |   177.67 ns |  0.78 |    0.01 |      - |         - |          NA |
| Foreach_Utf8StringView_CharEnumerator | .NET 8.0           | .NET 8.0           | 128    | EmojiHeavy |   139.47 ns |  2.792 ns |   3.324 ns |   139.02 ns |  0.61 |    0.02 |      - |         - |          NA |
| Foreach_Utf8StringView_CharEnumerator | .NET Framework 4.8 | .NET Framework 4.8 | 128    | EmojiHeavy |   226.98 ns |  3.910 ns |   3.658 ns |   225.63 ns |  1.00 |    0.02 |      - |         - |          NA |
|                                       |                    |                    |        |            |             |           |            |             |       |         |        |           |             |
| Baseline_ToString_ThenForeachChar     | .NET 10.0          | .NET 10.0          | 128    | EmojiHeavy |   224.05 ns |  0.354 ns |   0.277 ns |   224.03 ns |  0.89 |    0.01 | 0.0167 |     280 B |        0.97 |
| Baseline_ToString_ThenForeachChar     | .NET 6.0           | .NET 6.0           | 128    | EmojiHeavy |   259.26 ns |  1.496 ns |   1.249 ns |   259.81 ns |  1.03 |    0.01 | 0.0167 |     280 B |        0.97 |
| Baseline_ToString_ThenForeachChar     | .NET 8.0           | .NET 8.0           | 128    | EmojiHeavy |   230.34 ns |  1.916 ns |   1.600 ns |   230.13 ns |  0.92 |    0.01 | 0.0167 |     280 B |        0.97 |
| Baseline_ToString_ThenForeachChar     | .NET Framework 4.8 | .NET Framework 4.8 | 128    | EmojiHeavy |   251.11 ns |  2.183 ns |   1.823 ns |   250.90 ns |  1.00 |    0.01 | 0.0458 |     289 B |        1.00 |
|                                       |                    |                    |        |            |             |           |            |             |       |         |        |           |             |
| Baseline_ArrayPool_EncodingGetChars   | .NET 10.0          | .NET 10.0          | 128    | EmojiHeavy |   122.11 ns |  0.665 ns |   0.519 ns |   122.18 ns |  0.71 |    0.01 |      - |         - |          NA |
| Baseline_ArrayPool_EncodingGetChars   | .NET 6.0           | .NET 6.0           | 128    | EmojiHeavy |   139.56 ns |  0.318 ns |   0.265 ns |   139.50 ns |  0.81 |    0.01 |      - |         - |          NA |
| Baseline_ArrayPool_EncodingGetChars   | .NET 8.0           | .NET 8.0           | 128    | EmojiHeavy |   125.71 ns |  2.536 ns |   2.605 ns |   124.36 ns |  0.73 |    0.02 |      - |         - |          NA |
| Baseline_ArrayPool_EncodingGetChars   | .NET Framework 4.8 | .NET Framework 4.8 | 128    | EmojiHeavy |   173.15 ns |  1.664 ns |   1.390 ns |   173.08 ns |  1.00 |    0.01 |      - |         - |          NA |
|                                       |                    |                    |        |            |             |           |            |             |       |         |        |           |             |
| **Foreach_Utf8StringView_CharEnumerator** | **.NET 10.0**          | **.NET 10.0**          | **2048**   | **Ascii**      |   **785.02 ns** | **14.883 ns** |  **15.284 ns** |   **781.73 ns** |  **0.43** |    **0.01** |      **-** |         **-** |          **NA** |
| Foreach_Utf8StringView_CharEnumerator | .NET 6.0           | .NET 6.0           | 2048   | Ascii      | 1,904.64 ns |  6.258 ns |   4.886 ns | 1,905.68 ns |  1.05 |    0.01 |      - |         - |          NA |
| Foreach_Utf8StringView_CharEnumerator | .NET 8.0           | .NET 8.0           | 2048   | Ascii      |   769.77 ns |  7.103 ns |   5.931 ns |   768.20 ns |  0.42 |    0.01 |      - |         - |          NA |
| Foreach_Utf8StringView_CharEnumerator | .NET Framework 4.8 | .NET Framework 4.8 | 2048   | Ascii      | 1,815.99 ns | 21.795 ns |  20.387 ns | 1,810.13 ns |  1.00 |    0.02 |      - |         - |          NA |
|                                       |                    |                    |        |            |             |           |            |             |       |         |        |           |             |
| Baseline_ToString_ThenForeachChar     | .NET 10.0          | .NET 10.0          | 2048   | Ascii      |   474.48 ns |  5.215 ns |   4.623 ns |   473.00 ns |  0.53 |    0.01 | 0.2460 |    4120 B |        0.99 |
| Baseline_ToString_ThenForeachChar     | .NET 6.0           | .NET 6.0           | 2048   | Ascii      |   534.86 ns |  4.616 ns |   3.604 ns |   533.02 ns |  0.59 |    0.00 | 0.2460 |    4120 B |        0.99 |
| Baseline_ToString_ThenForeachChar     | .NET 8.0           | .NET 8.0           | 2048   | Ascii      |   480.99 ns |  5.372 ns |   4.194 ns |   481.09 ns |  0.53 |    0.00 | 0.2460 |    4120 B |        0.99 |
| Baseline_ToString_ThenForeachChar     | .NET Framework 4.8 | .NET Framework 4.8 | 2048   | Ascii      |   900.44 ns |  4.591 ns |   3.584 ns |   901.18 ns |  1.00 |    0.01 | 0.6571 |    4142 B |        1.00 |
|                                       |                    |                    |        |            |             |           |            |             |       |         |        |           |             |
| Baseline_ArrayPool_EncodingGetChars   | .NET 10.0          | .NET 10.0          | 2048   | Ascii      |   400.77 ns |  1.686 ns |   1.316 ns |   400.20 ns |  0.53 |    0.00 |      - |         - |          NA |
| Baseline_ArrayPool_EncodingGetChars   | .NET 6.0           | .NET 6.0           | 2048   | Ascii      |   469.30 ns |  0.701 ns |   0.586 ns |   468.94 ns |  0.62 |    0.00 |      - |         - |          NA |
| Baseline_ArrayPool_EncodingGetChars   | .NET 8.0           | .NET 8.0           | 2048   | Ascii      |   410.02 ns |  2.557 ns |   2.135 ns |   409.14 ns |  0.54 |    0.00 |      - |         - |          NA |
| Baseline_ArrayPool_EncodingGetChars   | .NET Framework 4.8 | .NET Framework 4.8 | 2048   | Ascii      |   756.54 ns |  1.391 ns |   1.162 ns |   756.34 ns |  1.00 |    0.00 |      - |         - |          NA |
|                                       |                    |                    |        |            |             |           |            |             |       |         |        |           |             |
| **Foreach_Utf8StringView_CharEnumerator** | **.NET 10.0**          | **.NET 10.0**          | **2048**   | **Cyrillic**   | **1,586.57 ns** | **25.544 ns** |  **23.894 ns** | **1,577.16 ns** |  **0.52** |    **0.02** |      **-** |         **-** |          **NA** |
| Foreach_Utf8StringView_CharEnumerator | .NET 6.0           | .NET 6.0           | 2048   | Cyrillic   | 2,628.24 ns | 13.377 ns |  10.444 ns | 2,627.32 ns |  0.87 |    0.02 |      - |         - |          NA |
| Foreach_Utf8StringView_CharEnumerator | .NET 8.0           | .NET 8.0           | 2048   | Cyrillic   | 1,899.28 ns | 10.646 ns |   8.890 ns | 1,897.49 ns |  0.63 |    0.02 |      - |         - |          NA |
| Foreach_Utf8StringView_CharEnumerator | .NET Framework 4.8 | .NET Framework 4.8 | 2048   | Cyrillic   | 3,037.82 ns | 59.771 ns |  79.793 ns | 2,993.30 ns |  1.00 |    0.04 |      - |         - |          NA |
|                                       |                    |                    |        |            |             |           |            |             |       |         |        |           |             |
| Baseline_ToString_ThenForeachChar     | .NET 10.0          | .NET 10.0          | 2048   | Cyrillic   | 1,578.72 ns | 28.438 ns |  26.601 ns | 1,570.91 ns |  0.40 |    0.01 | 0.2460 |    4120 B |        0.99 |
| Baseline_ToString_ThenForeachChar     | .NET 6.0           | .NET 6.0           | 2048   | Cyrillic   | 1,711.42 ns | 25.724 ns |  24.062 ns | 1,699.99 ns |  0.43 |    0.01 | 0.2460 |    4120 B |        0.99 |
| Baseline_ToString_ThenForeachChar     | .NET 8.0           | .NET 8.0           | 2048   | Cyrillic   | 1,636.28 ns | 13.489 ns |  11.264 ns | 1,632.16 ns |  0.41 |    0.01 | 0.2460 |    4120 B |        0.99 |
| Baseline_ToString_ThenForeachChar     | .NET Framework 4.8 | .NET Framework 4.8 | 2048   | Cyrillic   | 3,989.17 ns | 69.483 ns |  61.595 ns | 3,954.87 ns |  1.00 |    0.02 | 0.6561 |    4142 B |        1.00 |
|                                       |                    |                    |        |            |             |           |            |             |       |         |        |           |             |
| Baseline_ArrayPool_EncodingGetChars   | .NET 10.0          | .NET 10.0          | 2048   | Cyrillic   | 1,105.60 ns | 10.115 ns |   7.897 ns | 1,109.15 ns |  0.55 |    0.00 |      - |         - |          NA |
| Baseline_ArrayPool_EncodingGetChars   | .NET 6.0           | .NET 6.0           | 2048   | Cyrillic   | 1,114.77 ns |  4.490 ns |   3.749 ns | 1,112.97 ns |  0.55 |    0.00 |      - |         - |          NA |
| Baseline_ArrayPool_EncodingGetChars   | .NET 8.0           | .NET 8.0           | 2048   | Cyrillic   | 1,097.87 ns |  8.199 ns |   6.847 ns | 1,095.27 ns |  0.55 |    0.00 |      - |         - |          NA |
| Baseline_ArrayPool_EncodingGetChars   | .NET Framework 4.8 | .NET Framework 4.8 | 2048   | Cyrillic   | 2,013.83 ns | 12.766 ns |  11.317 ns | 2,016.23 ns |  1.00 |    0.01 |      - |         - |          NA |
|                                       |                    |                    |        |            |             |           |            |             |       |         |        |           |             |
| **Foreach_Utf8StringView_CharEnumerator** | **.NET 10.0**          | **.NET 10.0**          | **2048**   | **EmojiHeavy** | **2,167.04 ns** | **26.202 ns** |  **23.227 ns** | **2,163.12 ns** |  **0.65** |    **0.01** |      **-** |         **-** |          **NA** |
| Foreach_Utf8StringView_CharEnumerator | .NET 6.0           | .NET 6.0           | 2048   | EmojiHeavy | 2,614.42 ns | 50.347 ns |  59.934 ns | 2,580.98 ns |  0.79 |    0.02 |      - |         - |          NA |
| Foreach_Utf8StringView_CharEnumerator | .NET 8.0           | .NET 8.0           | 2048   | EmojiHeavy | 2,283.12 ns | 17.825 ns |  14.885 ns | 2,281.62 ns |  0.69 |    0.01 |      - |         - |          NA |
| Foreach_Utf8StringView_CharEnumerator | .NET Framework 4.8 | .NET Framework 4.8 | 2048   | EmojiHeavy | 3,317.11 ns | 25.639 ns |  20.017 ns | 3,316.53 ns |  1.00 |    0.01 |      - |         - |          NA |
|                                       |                    |                    |        |            |             |           |            |             |       |         |        |           |             |
| Baseline_ToString_ThenForeachChar     | .NET 10.0          | .NET 10.0          | 2048   | EmojiHeavy | 3,519.09 ns | 18.744 ns |  15.652 ns | 3,522.98 ns |  0.98 |    0.03 | 0.2441 |    4120 B |        0.99 |
| Baseline_ToString_ThenForeachChar     | .NET 6.0           | .NET 6.0           | 2048   | EmojiHeavy | 3,784.45 ns | 60.421 ns |  56.518 ns | 3,776.01 ns |  1.05 |    0.04 | 0.2441 |    4120 B |        0.99 |
| Baseline_ToString_ThenForeachChar     | .NET 8.0           | .NET 8.0           | 2048   | EmojiHeavy | 3,556.45 ns | 34.799 ns |  27.169 ns | 3,537.25 ns |  0.99 |    0.03 | 0.2441 |    4120 B |        0.99 |
| Baseline_ToString_ThenForeachChar     | .NET Framework 4.8 | .NET Framework 4.8 | 2048   | EmojiHeavy | 3,610.36 ns | 71.110 ns | 131.807 ns | 3,542.11 ns |  1.00 |    0.05 | 0.6561 |    4142 B |        1.00 |
|                                       |                    |                    |        |            |             |           |            |             |       |         |        |           |             |
| Baseline_ArrayPool_EncodingGetChars   | .NET 10.0          | .NET 10.0          | 2048   | EmojiHeavy | 1,710.13 ns | 12.541 ns |   9.791 ns | 1,707.87 ns |  0.69 |    0.01 |      - |         - |          NA |
| Baseline_ArrayPool_EncodingGetChars   | .NET 6.0           | .NET 6.0           | 2048   | EmojiHeavy | 1,932.78 ns | 31.485 ns |  26.292 ns | 1,921.49 ns |  0.78 |    0.01 |      - |         - |          NA |
| Baseline_ArrayPool_EncodingGetChars   | .NET 8.0           | .NET 8.0           | 2048   | EmojiHeavy | 1,690.57 ns |  1.201 ns |   1.064 ns | 1,690.75 ns |  0.68 |    0.00 |      - |         - |          NA |
| Baseline_ArrayPool_EncodingGetChars   | .NET Framework 4.8 | .NET Framework 4.8 | 2048   | EmojiHeavy | 2,488.42 ns | 19.160 ns |  16.985 ns | 2,490.73 ns |  1.00 |    0.01 |      - |         - |          NA |
