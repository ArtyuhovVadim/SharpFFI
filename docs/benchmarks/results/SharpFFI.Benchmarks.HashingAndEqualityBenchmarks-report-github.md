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
| Method                                         | Job                | Runtime            | ByteLength | Mean        | Error     | StdDev    | Ratio | RatioSD | Allocated | Alloc Ratio |
|----------------------------------------------- |------------------- |------------------- |----------- |------------:|----------:|----------:|------:|--------:|----------:|------------:|
| **NativeUtf8_GetHashCode_Marvin**                  | **.NET 10.0**          | **.NET 10.0**          | **3**          |   **1.3439 ns** | **0.0370 ns** | **0.0328 ns** |  **0.21** |    **0.01** |         **-** |          **NA** |
| NativeUtf8_GetHashCode_Marvin                  | .NET 6.0           | .NET 6.0           | 3          |   1.5356 ns | 0.0121 ns | 0.0113 ns |  0.24 |    0.00 |         - |          NA |
| NativeUtf8_GetHashCode_Marvin                  | .NET 8.0           | .NET 8.0           | 3          |   1.6403 ns | 0.0120 ns | 0.0112 ns |  0.26 |    0.00 |         - |          NA |
| NativeUtf8_GetHashCode_Marvin                  | .NET Framework 4.8 | .NET Framework 4.8 | 3          |   6.2863 ns | 0.0488 ns | 0.0457 ns |  1.00 |    0.01 |         - |          NA |
|                                                |                    |                    |            |             |           |           |       |         |           |             |
| ReadOnlyUtf8CString_GetHashCode                | .NET 10.0          | .NET 10.0          | 3          |   3.2408 ns | 0.0037 ns | 0.0033 ns |  0.23 |    0.00 |         - |          NA |
| ReadOnlyUtf8CString_GetHashCode                | .NET 6.0           | .NET 6.0           | 3          |   9.1677 ns | 0.1919 ns | 0.1602 ns |  0.64 |    0.01 |         - |          NA |
| ReadOnlyUtf8CString_GetHashCode                | .NET 8.0           | .NET 8.0           | 3          |   4.8064 ns | 0.0308 ns | 0.0288 ns |  0.34 |    0.00 |         - |          NA |
| ReadOnlyUtf8CString_GetHashCode                | .NET Framework 4.8 | .NET Framework 4.8 | 3          |  14.2430 ns | 0.1431 ns | 0.1338 ns |  1.00 |    0.01 |         - |          NA |
|                                                |                    |                    |            |             |           |           |       |         |           |             |
| Baseline_ManagedString_GetHashCode             | .NET 10.0          | .NET 10.0          | 3          |   2.0753 ns | 0.0029 ns | 0.0026 ns |  1.19 |    0.00 |         - |          NA |
| Baseline_ManagedString_GetHashCode             | .NET 6.0           | .NET 6.0           | 3          |   1.7972 ns | 0.0025 ns | 0.0019 ns |  1.03 |    0.00 |         - |          NA |
| Baseline_ManagedString_GetHashCode             | .NET 8.0           | .NET 8.0           | 3          |   1.6586 ns | 0.0023 ns | 0.0020 ns |  0.95 |    0.00 |         - |          NA |
| Baseline_ManagedString_GetHashCode             | .NET Framework 4.8 | .NET Framework 4.8 | 3          |   1.7451 ns | 0.0065 ns | 0.0050 ns |  1.00 |    0.00 |         - |          NA |
|                                                |                    |                    |            |             |           |           |       |         |           |             |
| StringView_Equals_SameContentDifferentPointers | .NET 10.0          | .NET 10.0          | 3          |   0.5114 ns | 0.0030 ns | 0.0025 ns |  0.03 |    0.00 |         - |          NA |
| StringView_Equals_SameContentDifferentPointers | .NET 6.0           | .NET 6.0           | 3          |  11.1192 ns | 0.1228 ns | 0.1149 ns |  0.63 |    0.01 |         - |          NA |
| StringView_Equals_SameContentDifferentPointers | .NET 8.0           | .NET 8.0           | 3          |   1.5215 ns | 0.0195 ns | 0.0183 ns |  0.09 |    0.00 |         - |          NA |
| StringView_Equals_SameContentDifferentPointers | .NET Framework 4.8 | .NET Framework 4.8 | 3          |  17.7568 ns | 0.0227 ns | 0.0212 ns |  1.00 |    0.00 |         - |          NA |
|                                                |                    |                    |            |             |           |           |       |         |           |             |
| CString_Equals_SameContentDifferentPointers    | .NET 10.0          | .NET 10.0          | 3          |   2.7279 ns | 0.0064 ns | 0.0056 ns |  0.12 |    0.00 |         - |          NA |
| CString_Equals_SameContentDifferentPointers    | .NET 6.0           | .NET 6.0           | 3          |  14.5037 ns | 0.1070 ns | 0.0835 ns |  0.66 |    0.00 |         - |          NA |
| CString_Equals_SameContentDifferentPointers    | .NET 8.0           | .NET 8.0           | 3          |   6.6152 ns | 0.0059 ns | 0.0046 ns |  0.30 |    0.00 |         - |          NA |
| CString_Equals_SameContentDifferentPointers    | .NET Framework 4.8 | .NET Framework 4.8 | 3          |  22.1002 ns | 0.0559 ns | 0.0523 ns |  1.00 |    0.00 |         - |          NA |
|                                                |                    |                    |            |             |           |           |       |         |           |             |
| **NativeUtf8_GetHashCode_Marvin**                  | **.NET 10.0**          | **.NET 10.0**          | **6**          |   **1.7162 ns** | **0.0122 ns** | **0.0108 ns** |  **0.26** |    **0.01** |         **-** |          **NA** |
| NativeUtf8_GetHashCode_Marvin                  | .NET 6.0           | .NET 6.0           | 6          |   1.8725 ns | 0.0134 ns | 0.0126 ns |  0.28 |    0.01 |         - |          NA |
| NativeUtf8_GetHashCode_Marvin                  | .NET 8.0           | .NET 8.0           | 6          |   1.9513 ns | 0.0076 ns | 0.0071 ns |  0.29 |    0.01 |         - |          NA |
| NativeUtf8_GetHashCode_Marvin                  | .NET Framework 4.8 | .NET Framework 4.8 | 6          |   6.6569 ns | 0.1400 ns | 0.1870 ns |  1.00 |    0.04 |         - |          NA |
|                                                |                    |                    |            |             |           |           |       |         |           |             |
| ReadOnlyUtf8CString_GetHashCode                | .NET 10.0          | .NET 10.0          | 6          |   3.6982 ns | 0.0358 ns | 0.0335 ns |  0.25 |    0.00 |         - |          NA |
| ReadOnlyUtf8CString_GetHashCode                | .NET 6.0           | .NET 6.0           | 6          |   9.0606 ns | 0.0116 ns | 0.0103 ns |  0.60 |    0.01 |         - |          NA |
| ReadOnlyUtf8CString_GetHashCode                | .NET 8.0           | .NET 8.0           | 6          |   4.8559 ns | 0.0247 ns | 0.0231 ns |  0.32 |    0.01 |         - |          NA |
| ReadOnlyUtf8CString_GetHashCode                | .NET Framework 4.8 | .NET Framework 4.8 | 6          |  15.0492 ns | 0.2694 ns | 0.2389 ns |  1.00 |    0.02 |         - |          NA |
|                                                |                    |                    |            |             |           |           |       |         |           |             |
| Baseline_ManagedString_GetHashCode             | .NET 10.0          | .NET 10.0          | 6          |   2.7709 ns | 0.0259 ns | 0.0242 ns |  1.10 |    0.01 |         - |          NA |
| Baseline_ManagedString_GetHashCode             | .NET 6.0           | .NET 6.0           | 6          |   3.2712 ns | 0.0030 ns | 0.0028 ns |  1.30 |    0.01 |         - |          NA |
| Baseline_ManagedString_GetHashCode             | .NET 8.0           | .NET 8.0           | 6          |   2.8210 ns | 0.0303 ns | 0.0283 ns |  1.12 |    0.01 |         - |          NA |
| Baseline_ManagedString_GetHashCode             | .NET Framework 4.8 | .NET Framework 4.8 | 6          |   2.5183 ns | 0.0173 ns | 0.0144 ns |  1.00 |    0.01 |         - |          NA |
|                                                |                    |                    |            |             |           |           |       |         |           |             |
| StringView_Equals_SameContentDifferentPointers | .NET 10.0          | .NET 10.0          | 6          |   0.3892 ns | 0.0051 ns | 0.0042 ns |  0.02 |    0.00 |         - |          NA |
| StringView_Equals_SameContentDifferentPointers | .NET 6.0           | .NET 6.0           | 6          |  14.5216 ns | 0.1622 ns | 0.1517 ns |  0.77 |    0.02 |         - |          NA |
| StringView_Equals_SameContentDifferentPointers | .NET 8.0           | .NET 8.0           | 6          |   1.3555 ns | 0.0085 ns | 0.0079 ns |  0.07 |    0.00 |         - |          NA |
| StringView_Equals_SameContentDifferentPointers | .NET Framework 4.8 | .NET Framework 4.8 | 6          |  18.7666 ns | 0.3768 ns | 0.4627 ns |  1.00 |    0.03 |         - |          NA |
|                                                |                    |                    |            |             |           |           |       |         |           |             |
| CString_Equals_SameContentDifferentPointers    | .NET 10.0          | .NET 10.0          | 6          |   2.5965 ns | 0.0194 ns | 0.0172 ns |  0.11 |    0.00 |         - |          NA |
| CString_Equals_SameContentDifferentPointers    | .NET 6.0           | .NET 6.0           | 6          |  14.2167 ns | 0.1309 ns | 0.1224 ns |  0.60 |    0.01 |         - |          NA |
| CString_Equals_SameContentDifferentPointers    | .NET 8.0           | .NET 8.0           | 6          |   6.4435 ns | 0.0389 ns | 0.0344 ns |  0.27 |    0.00 |         - |          NA |
| CString_Equals_SameContentDifferentPointers    | .NET Framework 4.8 | .NET Framework 4.8 | 6          |  23.6733 ns | 0.3051 ns | 0.2548 ns |  1.00 |    0.01 |         - |          NA |
|                                                |                    |                    |            |             |           |           |       |         |           |             |
| **NativeUtf8_GetHashCode_Marvin**                  | **.NET 10.0**          | **.NET 10.0**          | **64**         |  **13.4718 ns** | **0.1915 ns** | **0.1698 ns** |  **0.72** |    **0.01** |         **-** |          **NA** |
| NativeUtf8_GetHashCode_Marvin                  | .NET 6.0           | .NET 6.0           | 64         |  13.2880 ns | 0.0570 ns | 0.0505 ns |  0.71 |    0.01 |         - |          NA |
| NativeUtf8_GetHashCode_Marvin                  | .NET 8.0           | .NET 8.0           | 64         |  13.6107 ns | 0.0955 ns | 0.0847 ns |  0.72 |    0.01 |         - |          NA |
| NativeUtf8_GetHashCode_Marvin                  | .NET Framework 4.8 | .NET Framework 4.8 | 64         |  18.8189 ns | 0.2856 ns | 0.2672 ns |  1.00 |    0.02 |         - |          NA |
|                                                |                    |                    |            |             |           |           |       |         |           |             |
| ReadOnlyUtf8CString_GetHashCode                | .NET 10.0          | .NET 10.0          | 64         |  14.4663 ns | 0.2124 ns | 0.1774 ns |  0.49 |    0.01 |         - |          NA |
| ReadOnlyUtf8CString_GetHashCode                | .NET 6.0           | .NET 6.0           | 64         |  23.7476 ns | 0.1653 ns | 0.1465 ns |  0.80 |    0.01 |         - |          NA |
| ReadOnlyUtf8CString_GetHashCode                | .NET 8.0           | .NET 8.0           | 64         |  16.5361 ns | 0.1575 ns | 0.1473 ns |  0.56 |    0.01 |         - |          NA |
| ReadOnlyUtf8CString_GetHashCode                | .NET Framework 4.8 | .NET Framework 4.8 | 64         |  29.6133 ns | 0.1387 ns | 0.1297 ns |  1.00 |    0.01 |         - |          NA |
|                                                |                    |                    |            |             |           |           |       |         |           |             |
| Baseline_ManagedString_GetHashCode             | .NET 10.0          | .NET 10.0          | 64         |  27.3151 ns | 0.0831 ns | 0.0736 ns |  1.59 |    0.03 |         - |          NA |
| Baseline_ManagedString_GetHashCode             | .NET 6.0           | .NET 6.0           | 64         |  27.3647 ns | 0.2067 ns | 0.1934 ns |  1.59 |    0.03 |         - |          NA |
| Baseline_ManagedString_GetHashCode             | .NET 8.0           | .NET 8.0           | 64         |  27.4714 ns | 0.1293 ns | 0.1146 ns |  1.60 |    0.03 |         - |          NA |
| Baseline_ManagedString_GetHashCode             | .NET Framework 4.8 | .NET Framework 4.8 | 64         |  17.1960 ns | 0.3524 ns | 0.3770 ns |  1.00 |    0.03 |         - |          NA |
|                                                |                    |                    |            |             |           |           |       |         |           |             |
| StringView_Equals_SameContentDifferentPointers | .NET 10.0          | .NET 10.0          | 64         |   0.4906 ns | 0.0024 ns | 0.0021 ns |  0.03 |    0.00 |         - |          NA |
| StringView_Equals_SameContentDifferentPointers | .NET 6.0           | .NET 6.0           | 64         |  12.4803 ns | 0.0306 ns | 0.0239 ns |  0.68 |    0.00 |         - |          NA |
| StringView_Equals_SameContentDifferentPointers | .NET 8.0           | .NET 8.0           | 64         |   1.5692 ns | 0.0258 ns | 0.0241 ns |  0.09 |    0.00 |         - |          NA |
| StringView_Equals_SameContentDifferentPointers | .NET Framework 4.8 | .NET Framework 4.8 | 64         |  18.3416 ns | 0.1323 ns | 0.1237 ns |  1.00 |    0.01 |         - |          NA |
|                                                |                    |                    |            |             |           |           |       |         |           |             |
| CString_Equals_SameContentDifferentPointers    | .NET 10.0          | .NET 10.0          | 64         |   4.5670 ns | 0.1095 ns | 0.1639 ns |  0.19 |    0.01 |         - |          NA |
| CString_Equals_SameContentDifferentPointers    | .NET 6.0           | .NET 6.0           | 64         |  16.5599 ns | 0.0750 ns | 0.0701 ns |  0.67 |    0.01 |         - |          NA |
| CString_Equals_SameContentDifferentPointers    | .NET 8.0           | .NET 8.0           | 64         |   7.2059 ns | 0.0994 ns | 0.0930 ns |  0.29 |    0.00 |         - |          NA |
| CString_Equals_SameContentDifferentPointers    | .NET Framework 4.8 | .NET Framework 4.8 | 64         |  24.6235 ns | 0.3151 ns | 0.2947 ns |  1.00 |    0.02 |         - |          NA |
|                                                |                    |                    |            |             |           |           |       |         |           |             |
| **NativeUtf8_GetHashCode_Marvin**                  | **.NET 10.0**          | **.NET 10.0**          | **1024**       | **236.0963 ns** | **3.6012 ns** | **3.1924 ns** |  **0.99** |    **0.01** |         **-** |          **NA** |
| NativeUtf8_GetHashCode_Marvin                  | .NET 6.0           | .NET 6.0           | 1024       | 235.0804 ns | 2.8552 ns | 2.6708 ns |  0.99 |    0.01 |         - |          NA |
| NativeUtf8_GetHashCode_Marvin                  | .NET 8.0           | .NET 8.0           | 1024       | 232.8075 ns | 0.5619 ns | 0.5256 ns |  0.98 |    0.00 |         - |          NA |
| NativeUtf8_GetHashCode_Marvin                  | .NET Framework 4.8 | .NET Framework 4.8 | 1024       | 237.8470 ns | 0.2067 ns | 0.1933 ns |  1.00 |    0.00 |         - |          NA |
|                                                |                    |                    |            |             |           |           |       |         |           |             |
| ReadOnlyUtf8CString_GetHashCode                | .NET 10.0          | .NET 10.0          | 1024       | 240.0647 ns | 2.8300 ns | 2.5087 ns |  0.93 |    0.01 |         - |          NA |
| ReadOnlyUtf8CString_GetHashCode                | .NET 6.0           | .NET 6.0           | 1024       | 253.7166 ns | 5.0775 ns | 7.1180 ns |  0.98 |    0.03 |         - |          NA |
| ReadOnlyUtf8CString_GetHashCode                | .NET 8.0           | .NET 8.0           | 1024       | 239.8682 ns | 1.1865 ns | 1.1099 ns |  0.93 |    0.00 |         - |          NA |
| ReadOnlyUtf8CString_GetHashCode                | .NET Framework 4.8 | .NET Framework 4.8 | 1024       | 258.0077 ns | 0.2337 ns | 0.2072 ns |  1.00 |    0.00 |         - |          NA |
|                                                |                    |                    |            |             |           |           |       |         |           |             |
| Baseline_ManagedString_GetHashCode             | .NET 10.0          | .NET 10.0          | 1024       | 470.0075 ns | 1.5248 ns | 1.4263 ns |  1.62 |    0.02 |         - |          NA |
| Baseline_ManagedString_GetHashCode             | .NET 6.0           | .NET 6.0           | 1024       | 472.2097 ns | 5.0297 ns | 4.4587 ns |  1.62 |    0.02 |         - |          NA |
| Baseline_ManagedString_GetHashCode             | .NET 8.0           | .NET 8.0           | 1024       | 472.3781 ns | 2.0129 ns | 1.6809 ns |  1.62 |    0.02 |         - |          NA |
| Baseline_ManagedString_GetHashCode             | .NET Framework 4.8 | .NET Framework 4.8 | 1024       | 290.9858 ns | 2.9739 ns | 2.7818 ns |  1.00 |    0.01 |         - |          NA |
|                                                |                    |                    |            |             |           |           |       |         |           |             |
| StringView_Equals_SameContentDifferentPointers | .NET 10.0          | .NET 10.0          | 1024       |   8.2423 ns | 0.0820 ns | 0.0767 ns |  0.29 |    0.01 |         - |          NA |
| StringView_Equals_SameContentDifferentPointers | .NET 6.0           | .NET 6.0           | 1024       |  18.9396 ns | 0.1348 ns | 0.1261 ns |  0.67 |    0.01 |         - |          NA |
| StringView_Equals_SameContentDifferentPointers | .NET 8.0           | .NET 8.0           | 1024       |   8.4193 ns | 0.0135 ns | 0.0113 ns |  0.30 |    0.01 |         - |          NA |
| StringView_Equals_SameContentDifferentPointers | .NET Framework 4.8 | .NET Framework 4.8 | 1024       |  28.4679 ns | 0.5705 ns | 0.6341 ns |  1.00 |    0.03 |         - |          NA |
|                                                |                    |                    |            |             |           |           |       |         |           |             |
| CString_Equals_SameContentDifferentPointers    | .NET 10.0          | .NET 10.0          | 1024       |  13.6978 ns | 0.1299 ns | 0.1215 ns |  0.26 |    0.00 |         - |          NA |
| CString_Equals_SameContentDifferentPointers    | .NET 6.0           | .NET 6.0           | 1024       |  32.7679 ns | 0.6505 ns | 0.6085 ns |  0.61 |    0.01 |         - |          NA |
| CString_Equals_SameContentDifferentPointers    | .NET 8.0           | .NET 8.0           | 1024       |  18.8541 ns | 0.0476 ns | 0.0445 ns |  0.35 |    0.00 |         - |          NA |
| CString_Equals_SameContentDifferentPointers    | .NET Framework 4.8 | .NET Framework 4.8 | 1024       |  53.4183 ns | 0.5152 ns | 0.4819 ns |  1.00 |    0.01 |         - |          NA |
