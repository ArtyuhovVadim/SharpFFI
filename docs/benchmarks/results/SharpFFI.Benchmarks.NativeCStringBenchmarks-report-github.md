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
| Method                              | Job                | Runtime            | Length | Mean      | Error     | StdDev    | Median    | Ratio | RatioSD | Gen0   | Allocated | Alloc Ratio |
|------------------------------------ |------------------- |------------------- |------- |----------:|----------:|----------:|----------:|------:|--------:|-------:|----------:|------------:|
| **NativeUtf8_FromString_AndDispose**    | **.NET 10.0**          | **.NET 10.0**          | **16**     |  **49.38 ns** |  **0.994 ns** |  **1.327 ns** |  **48.63 ns** |  **0.66** |    **0.02** | **0.0019** |      **32 B** |        **1.00** |
| NativeUtf8_FromString_AndDispose    | .NET 6.0           | .NET 6.0           | 16     |  57.81 ns |  1.162 ns |  1.243 ns |  57.54 ns |  0.77 |    0.02 | 0.0019 |      32 B |        1.00 |
| NativeUtf8_FromString_AndDispose    | .NET 8.0           | .NET 8.0           | 16     |  48.39 ns |  0.607 ns |  0.568 ns |  48.52 ns |  0.64 |    0.01 | 0.0019 |      32 B |        1.00 |
| NativeUtf8_FromString_AndDispose    | .NET Framework 4.8 | .NET Framework 4.8 | 16     |  75.38 ns |  0.984 ns |  0.821 ns |  75.37 ns |  1.00 |    0.01 | 0.0050 |      32 B |        1.00 |
|                                     |                    |                    |        |           |           |           |           |       |         |        |           |             |
| NativeUtf8_FromUtf8Span_AndDispose  | .NET 10.0          | .NET 10.0          | 16     |  42.45 ns |  0.558 ns |  0.466 ns |  42.23 ns |  0.69 |    0.01 | 0.0019 |      32 B |        1.00 |
| NativeUtf8_FromUtf8Span_AndDispose  | .NET 6.0           | .NET 6.0           | 16     |  46.50 ns |  0.807 ns |  0.755 ns |  46.37 ns |  0.75 |    0.02 | 0.0019 |      32 B |        1.00 |
| NativeUtf8_FromUtf8Span_AndDispose  | .NET 8.0           | .NET 8.0           | 16     |  41.70 ns |  0.318 ns |  0.282 ns |  41.61 ns |  0.68 |    0.01 | 0.0019 |      32 B |        1.00 |
| NativeUtf8_FromUtf8Span_AndDispose  | .NET Framework 4.8 | .NET Framework 4.8 | 16     |  61.78 ns |  0.910 ns |  0.973 ns |  61.61 ns |  1.00 |    0.02 | 0.0050 |      32 B |        1.00 |
|                                     |                    |                    |        |           |           |           |           |       |         |        |           |             |
| NativeUtf16_FromString_AndDispose   | .NET 10.0          | .NET 10.0          | 16     |  42.91 ns |  0.275 ns |  0.244 ns |  42.96 ns |  0.69 |    0.01 | 0.0019 |      32 B |        1.00 |
| NativeUtf16_FromString_AndDispose   | .NET 6.0           | .NET 6.0           | 16     |  56.02 ns |  0.467 ns |  0.437 ns |  55.97 ns |  0.90 |    0.02 | 0.0019 |      32 B |        1.00 |
| NativeUtf16_FromString_AndDispose   | .NET 8.0           | .NET 8.0           | 16     |  45.30 ns |  0.881 ns |  0.943 ns |  44.89 ns |  0.73 |    0.02 | 0.0019 |      32 B |        1.00 |
| NativeUtf16_FromString_AndDispose   | .NET Framework 4.8 | .NET Framework 4.8 | 16     |  62.15 ns |  1.105 ns |  1.033 ns |  61.77 ns |  1.00 |    0.02 | 0.0050 |      32 B |        1.00 |
|                                     |                    |                    |        |           |           |           |           |       |         |        |           |             |
| TruncationOverhead_WithEmbeddedNull | .NET 10.0          | .NET 10.0          | 16     |  47.81 ns |  0.173 ns |  0.161 ns |  47.72 ns |  0.63 |    0.01 | 0.0019 |      32 B |        1.00 |
| TruncationOverhead_WithEmbeddedNull | .NET 6.0           | .NET 6.0           | 16     |  54.94 ns |  0.859 ns |  0.804 ns |  54.65 ns |  0.72 |    0.01 | 0.0019 |      32 B |        1.00 |
| TruncationOverhead_WithEmbeddedNull | .NET 8.0           | .NET 8.0           | 16     |  47.95 ns |  0.484 ns |  0.404 ns |  47.96 ns |  0.63 |    0.01 | 0.0019 |      32 B |        1.00 |
| TruncationOverhead_WithEmbeddedNull | .NET Framework 4.8 | .NET Framework 4.8 | 16     |  76.42 ns |  0.761 ns |  0.635 ns |  76.70 ns |  1.00 |    0.01 | 0.0050 |      32 B |        1.00 |
|                                     |                    |                    |        |           |           |           |           |       |         |        |           |             |
| **NativeUtf8_FromString_AndDispose**    | **.NET 10.0**          | **.NET 10.0**          | **128**    |  **57.93 ns** |  **0.635 ns** |  **0.530 ns** |  **57.98 ns** |  **0.54** |    **0.01** | **0.0019** |      **32 B** |        **1.00** |
| NativeUtf8_FromString_AndDispose    | .NET 6.0           | .NET 6.0           | 128    |  63.58 ns |  1.295 ns |  1.212 ns |  63.36 ns |  0.59 |    0.01 | 0.0019 |      32 B |        1.00 |
| NativeUtf8_FromString_AndDispose    | .NET 8.0           | .NET 8.0           | 128    |  56.46 ns |  0.609 ns |  0.570 ns |  56.15 ns |  0.53 |    0.01 | 0.0019 |      32 B |        1.00 |
| NativeUtf8_FromString_AndDispose    | .NET Framework 4.8 | .NET Framework 4.8 | 128    | 107.41 ns |  1.874 ns |  1.753 ns | 107.63 ns |  1.00 |    0.02 | 0.0050 |      32 B |        1.00 |
|                                     |                    |                    |        |           |           |           |           |       |         |        |           |             |
| NativeUtf8_FromUtf8Span_AndDispose  | .NET 10.0          | .NET 10.0          | 128    |  42.72 ns |  0.407 ns |  0.340 ns |  42.67 ns |  0.66 |    0.01 | 0.0019 |      32 B |        1.00 |
| NativeUtf8_FromUtf8Span_AndDispose  | .NET 6.0           | .NET 6.0           | 128    |  48.24 ns |  0.549 ns |  0.458 ns |  48.29 ns |  0.75 |    0.01 | 0.0019 |      32 B |        1.00 |
| NativeUtf8_FromUtf8Span_AndDispose  | .NET 8.0           | .NET 8.0           | 128    |  42.93 ns |  0.564 ns |  0.500 ns |  42.82 ns |  0.66 |    0.01 | 0.0019 |      32 B |        1.00 |
| NativeUtf8_FromUtf8Span_AndDispose  | .NET Framework 4.8 | .NET Framework 4.8 | 128    |  64.69 ns |  0.339 ns |  0.283 ns |  64.61 ns |  1.00 |    0.01 | 0.0050 |      32 B |        1.00 |
|                                     |                    |                    |        |           |           |           |           |       |         |        |           |             |
| NativeUtf16_FromString_AndDispose   | .NET 10.0          | .NET 10.0          | 128    |  46.90 ns |  0.710 ns |  0.629 ns |  46.62 ns |  0.69 |    0.01 | 0.0019 |      32 B |        1.00 |
| NativeUtf16_FromString_AndDispose   | .NET 6.0           | .NET 6.0           | 128    |  50.79 ns |  0.587 ns |  0.520 ns |  50.85 ns |  0.75 |    0.01 | 0.0019 |      32 B |        1.00 |
| NativeUtf16_FromString_AndDispose   | .NET 8.0           | .NET 8.0           | 128    |  45.03 ns |  0.643 ns |  0.570 ns |  44.89 ns |  0.66 |    0.01 | 0.0019 |      32 B |        1.00 |
| NativeUtf16_FromString_AndDispose   | .NET Framework 4.8 | .NET Framework 4.8 | 128    |  68.18 ns |  1.024 ns |  0.958 ns |  68.00 ns |  1.00 |    0.02 | 0.0050 |      32 B |        1.00 |
|                                     |                    |                    |        |           |           |           |           |       |         |        |           |             |
| TruncationOverhead_WithEmbeddedNull | .NET 10.0          | .NET 10.0          | 128    |  49.44 ns |  0.864 ns |  0.722 ns |  49.32 ns |  0.64 |    0.01 | 0.0019 |      32 B |        1.00 |
| TruncationOverhead_WithEmbeddedNull | .NET 6.0           | .NET 6.0           | 128    |  56.30 ns |  0.375 ns |  0.313 ns |  56.43 ns |  0.73 |    0.01 | 0.0019 |      32 B |        1.00 |
| TruncationOverhead_WithEmbeddedNull | .NET 8.0           | .NET 8.0           | 128    |  49.60 ns |  0.854 ns |  0.799 ns |  49.56 ns |  0.65 |    0.01 | 0.0019 |      32 B |        1.00 |
| TruncationOverhead_WithEmbeddedNull | .NET Framework 4.8 | .NET Framework 4.8 | 128    |  76.90 ns |  1.357 ns |  1.203 ns |  76.68 ns |  1.00 |    0.02 | 0.0050 |      32 B |        1.00 |
|                                     |                    |                    |        |           |           |           |           |       |         |        |           |             |
| **NativeUtf8_FromString_AndDispose**    | **.NET 10.0**          | **.NET 10.0**          | **2048**   | **103.65 ns** |  **1.120 ns** |  **1.047 ns** | **103.34 ns** |  **0.17** |    **0.00** | **0.0019** |      **32 B** |        **1.00** |
| NativeUtf8_FromString_AndDispose    | .NET 6.0           | .NET 6.0           | 2048   | 157.19 ns |  1.392 ns |  1.234 ns | 156.96 ns |  0.26 |    0.00 | 0.0019 |      32 B |        1.00 |
| NativeUtf8_FromString_AndDispose    | .NET 8.0           | .NET 8.0           | 2048   | 102.06 ns |  0.677 ns |  0.633 ns | 101.96 ns |  0.17 |    0.00 | 0.0019 |      32 B |        1.00 |
| NativeUtf8_FromString_AndDispose    | .NET Framework 4.8 | .NET Framework 4.8 | 2048   | 615.69 ns | 10.774 ns | 10.078 ns | 617.31 ns |  1.00 |    0.02 | 0.0048 |      32 B |        1.00 |
|                                     |                    |                    |        |           |           |           |           |       |         |        |           |             |
| NativeUtf8_FromUtf8Span_AndDispose  | .NET 10.0          | .NET 10.0          | 2048   |  65.77 ns |  1.321 ns |  1.103 ns |  66.11 ns |  0.64 |    0.01 | 0.0019 |      32 B |        1.00 |
| NativeUtf8_FromUtf8Span_AndDispose  | .NET 6.0           | .NET 6.0           | 2048   |  75.05 ns |  1.517 ns |  2.270 ns |  74.13 ns |  0.73 |    0.02 | 0.0019 |      32 B |        1.00 |
| NativeUtf8_FromUtf8Span_AndDispose  | .NET 8.0           | .NET 8.0           | 2048   |  60.52 ns |  0.647 ns |  0.573 ns |  60.37 ns |  0.59 |    0.01 | 0.0019 |      32 B |        1.00 |
| NativeUtf8_FromUtf8Span_AndDispose  | .NET Framework 4.8 | .NET Framework 4.8 | 2048   | 103.20 ns |  0.684 ns |  0.607 ns | 103.13 ns |  1.00 |    0.01 | 0.0050 |      32 B |        1.00 |
|                                     |                    |                    |        |           |           |           |           |       |         |        |           |             |
| NativeUtf16_FromString_AndDispose   | .NET 10.0          | .NET 10.0          | 2048   |  87.92 ns |  0.880 ns |  0.780 ns |  88.04 ns |  0.51 |    0.00 | 0.0019 |      32 B |        1.00 |
| NativeUtf16_FromString_AndDispose   | .NET 6.0           | .NET 6.0           | 2048   |  99.20 ns |  1.207 ns |  1.070 ns |  99.24 ns |  0.57 |    0.01 | 0.0019 |      32 B |        1.00 |
| NativeUtf16_FromString_AndDispose   | .NET 8.0           | .NET 8.0           | 2048   |  86.51 ns |  1.445 ns |  1.484 ns |  86.23 ns |  0.50 |    0.01 | 0.0019 |      32 B |        1.00 |
| NativeUtf16_FromString_AndDispose   | .NET Framework 4.8 | .NET Framework 4.8 | 2048   | 173.60 ns |  0.225 ns |  0.210 ns | 173.58 ns |  1.00 |    0.00 | 0.0050 |      32 B |        1.00 |
|                                     |                    |                    |        |           |           |           |           |       |         |        |           |             |
| TruncationOverhead_WithEmbeddedNull | .NET 10.0          | .NET 10.0          | 2048   |  48.43 ns |  0.462 ns |  0.410 ns |  48.33 ns |  0.64 |    0.01 | 0.0019 |      32 B |        1.00 |
| TruncationOverhead_WithEmbeddedNull | .NET 6.0           | .NET 6.0           | 2048   |  56.33 ns |  0.433 ns |  0.338 ns |  56.32 ns |  0.75 |    0.01 | 0.0019 |      32 B |        1.00 |
| TruncationOverhead_WithEmbeddedNull | .NET 8.0           | .NET 8.0           | 2048   |  48.43 ns |  0.486 ns |  0.431 ns |  48.47 ns |  0.64 |    0.01 | 0.0019 |      32 B |        1.00 |
| TruncationOverhead_WithEmbeddedNull | .NET Framework 4.8 | .NET Framework 4.8 | 2048   |  75.59 ns |  0.564 ns |  0.471 ns |  75.75 ns |  1.00 |    0.01 | 0.0050 |      32 B |        1.00 |
