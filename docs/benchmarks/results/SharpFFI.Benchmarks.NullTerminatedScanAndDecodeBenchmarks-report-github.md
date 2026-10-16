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
| Method                                 | Job                | Runtime            | Length | Mean       | Error     | StdDev    | Median     | Ratio | RatioSD | Gen0   | Allocated | Alloc Ratio |
|--------------------------------------- |------------------- |------------------- |------- |-----------:|----------:|----------:|-----------:|------:|--------:|-------:|----------:|------------:|
| **Utf8CString_AsReadOnlyView**             | **.NET 10.0**          | **.NET 10.0**          | **16**     |   **1.361 ns** | **0.0198 ns** | **0.0176 ns** |   **1.363 ns** |  **0.16** |    **0.00** |      **-** |         **-** |          **NA** |
| Utf8CString_AsReadOnlyView             | .NET 6.0           | .NET 6.0           | 16     |   6.047 ns | 0.0933 ns | 0.0872 ns |   6.069 ns |  0.70 |    0.01 |      - |         - |          NA |
| Utf8CString_AsReadOnlyView             | .NET 8.0           | .NET 8.0           | 16     |   2.589 ns | 0.0078 ns | 0.0069 ns |   2.589 ns |  0.30 |    0.00 |      - |         - |          NA |
| Utf8CString_AsReadOnlyView             | .NET Framework 4.8 | .NET Framework 4.8 | 16     |   8.613 ns | 0.0354 ns | 0.0314 ns |   8.611 ns |  1.00 |    0.00 |      - |         - |          NA |
|                                        |                    |                    |        |            |           |           |            |       |         |        |           |             |
| Utf16CString_AsReadOnlyView            | .NET 10.0          | .NET 10.0          | 16     |   1.491 ns | 0.0482 ns | 0.0644 ns |   1.457 ns |  0.22 |    0.01 |      - |         - |          NA |
| Utf16CString_AsReadOnlyView            | .NET 6.0           | .NET 6.0           | 16     |   6.119 ns | 0.0235 ns | 0.0220 ns |   6.121 ns |  0.89 |    0.01 |      - |         - |          NA |
| Utf16CString_AsReadOnlyView            | .NET 8.0           | .NET 8.0           | 16     |   2.827 ns | 0.0178 ns | 0.0166 ns |   2.827 ns |  0.41 |    0.00 |      - |         - |          NA |
| Utf16CString_AsReadOnlyView            | .NET Framework 4.8 | .NET Framework 4.8 | 16     |   6.909 ns | 0.0705 ns | 0.0659 ns |   6.897 ns |  1.00 |    0.01 |      - |         - |          NA |
|                                        |                    |                    |        |            |           |           |            |       |         |        |           |             |
| Utf8CString_ToString_UnknownLength     | .NET 10.0          | .NET 10.0          | 16     |   9.286 ns | 0.1165 ns | 0.0910 ns |   9.266 ns |  0.28 |    0.00 | 0.0033 |      56 B |        0.88 |
| Utf8CString_ToString_UnknownLength     | .NET 6.0           | .NET 6.0           | 16     |  14.205 ns | 0.1571 ns | 0.1312 ns |  14.159 ns |  0.44 |    0.01 | 0.0033 |      56 B |        0.88 |
| Utf8CString_ToString_UnknownLength     | .NET 8.0           | .NET 8.0           | 16     |  12.320 ns | 0.1881 ns | 0.1571 ns |  12.242 ns |  0.38 |    0.01 | 0.0033 |      56 B |        0.88 |
| Utf8CString_ToString_UnknownLength     | .NET Framework 4.8 | .NET Framework 4.8 | 16     |  32.659 ns | 0.5570 ns | 0.4938 ns |  32.483 ns |  1.00 |    0.02 | 0.0102 |      64 B |        1.00 |
|                                        |                    |                    |        |            |           |           |            |       |         |        |           |             |
| NativeUtf8CString_ToString_KnownLength | .NET 10.0          | .NET 10.0          | 16     |   9.269 ns | 0.2195 ns | 0.2527 ns |   9.231 ns |  0.43 |    0.01 | 0.0033 |      56 B |        0.88 |
| NativeUtf8CString_ToString_KnownLength | .NET 6.0           | .NET 6.0           | 16     |   8.990 ns | 0.1922 ns | 0.3704 ns |   8.913 ns |  0.42 |    0.02 | 0.0033 |      56 B |        0.88 |
| NativeUtf8CString_ToString_KnownLength | .NET 8.0           | .NET 8.0           | 16     |  10.470 ns | 0.1871 ns | 0.1750 ns |  10.367 ns |  0.49 |    0.01 | 0.0033 |      56 B |        0.88 |
| NativeUtf8CString_ToString_KnownLength | .NET Framework 4.8 | .NET Framework 4.8 | 16     |  21.321 ns | 0.0973 ns | 0.0910 ns |  21.303 ns |  1.00 |    0.01 | 0.0102 |      64 B |        1.00 |
|                                        |                    |                    |        |            |           |           |            |       |         |        |           |             |
| Baseline_Marshal_PtrToStringUTF8       | .NET 10.0          | .NET 10.0          | 16     |   8.918 ns | 0.1046 ns | 0.0874 ns |   8.959 ns |  0.47 |    0.01 | 0.0033 |      56 B |        0.88 |
| Baseline_Marshal_PtrToStringUTF8       | .NET 6.0           | .NET 6.0           | 16     |  10.540 ns | 0.2234 ns | 0.2089 ns |  10.460 ns |  0.56 |    0.01 | 0.0033 |      56 B |        0.88 |
| Baseline_Marshal_PtrToStringUTF8       | .NET 8.0           | .NET 8.0           | 16     |  11.066 ns | 0.0839 ns | 0.0655 ns |  11.051 ns |  0.58 |    0.00 | 0.0033 |      56 B |        0.88 |
| Baseline_Marshal_PtrToStringUTF8       | .NET Framework 4.8 | .NET Framework 4.8 | 16     |  18.920 ns | 0.1114 ns | 0.1042 ns |  18.891 ns |  1.00 |    0.01 | 0.0102 |      64 B |        1.00 |
|                                        |                    |                    |        |            |           |           |            |       |         |        |           |             |
| Utf16CString_ToString                  | .NET 10.0          | .NET 10.0          | 16     |   5.407 ns | 0.1293 ns | 0.1210 ns |   5.382 ns |  0.27 |    0.01 | 0.0033 |      56 B |        0.88 |
| Utf16CString_ToString                  | .NET 6.0           | .NET 6.0           | 16     |   9.199 ns | 0.0919 ns | 0.0815 ns |   9.178 ns |  0.46 |    0.00 | 0.0033 |      56 B |        0.88 |
| Utf16CString_ToString                  | .NET 8.0           | .NET 8.0           | 16     |   7.716 ns | 0.1159 ns | 0.1084 ns |   7.704 ns |  0.39 |    0.01 | 0.0033 |      56 B |        0.88 |
| Utf16CString_ToString                  | .NET Framework 4.8 | .NET Framework 4.8 | 16     |  20.024 ns | 0.1319 ns | 0.1234 ns |  20.099 ns |  1.00 |    0.01 | 0.0102 |      64 B |        1.00 |
|                                        |                    |                    |        |            |           |           |            |       |         |        |           |             |
| Baseline_Marshal_PtrToStringUni        | .NET 10.0          | .NET 10.0          | 16     |   5.244 ns | 0.1393 ns | 0.1303 ns |   5.228 ns |  0.69 |    0.02 | 0.0033 |      56 B |        0.88 |
| Baseline_Marshal_PtrToStringUni        | .NET 6.0           | .NET 6.0           | 16     |   5.757 ns | 0.0218 ns | 0.0182 ns |   5.753 ns |  0.76 |    0.00 | 0.0033 |      56 B |        0.88 |
| Baseline_Marshal_PtrToStringUni        | .NET 8.0           | .NET 8.0           | 16     |   5.910 ns | 0.1234 ns | 0.1154 ns |   5.855 ns |  0.78 |    0.02 | 0.0033 |      56 B |        0.88 |
| Baseline_Marshal_PtrToStringUni        | .NET Framework 4.8 | .NET Framework 4.8 | 16     |   7.577 ns | 0.0356 ns | 0.0297 ns |   7.590 ns |  1.00 |    0.01 | 0.0102 |      64 B |        1.00 |
|                                        |                    |                    |        |            |           |           |            |       |         |        |           |             |
| **Utf8CString_AsReadOnlyView**             | **.NET 10.0**          | **.NET 10.0**          | **128**    |   **1.664 ns** | **0.0322 ns** | **0.0301 ns** |   **1.664 ns** |  **0.21** |    **0.00** |      **-** |         **-** |          **NA** |
| Utf8CString_AsReadOnlyView             | .NET 6.0           | .NET 6.0           | 128    |   6.548 ns | 0.0803 ns | 0.0751 ns |   6.565 ns |  0.81 |    0.01 |      - |         - |          NA |
| Utf8CString_AsReadOnlyView             | .NET 8.0           | .NET 8.0           | 128    |   2.868 ns | 0.0235 ns | 0.0220 ns |   2.868 ns |  0.35 |    0.00 |      - |         - |          NA |
| Utf8CString_AsReadOnlyView             | .NET Framework 4.8 | .NET Framework 4.8 | 128    |   8.093 ns | 0.0174 ns | 0.0163 ns |   8.092 ns |  1.00 |    0.00 |      - |         - |          NA |
|                                        |                    |                    |        |            |           |           |            |       |         |        |           |             |
| Utf16CString_AsReadOnlyView            | .NET 10.0          | .NET 10.0          | 128    |   2.356 ns | 0.0200 ns | 0.0178 ns |   2.348 ns |  0.26 |    0.00 |      - |         - |          NA |
| Utf16CString_AsReadOnlyView            | .NET 6.0           | .NET 6.0           | 128    |   7.248 ns | 0.0163 ns | 0.0136 ns |   7.242 ns |  0.81 |    0.01 |      - |         - |          NA |
| Utf16CString_AsReadOnlyView            | .NET 8.0           | .NET 8.0           | 128    |   4.176 ns | 0.0152 ns | 0.0142 ns |   4.174 ns |  0.47 |    0.00 |      - |         - |          NA |
| Utf16CString_AsReadOnlyView            | .NET Framework 4.8 | .NET Framework 4.8 | 128    |   8.936 ns | 0.0860 ns | 0.0805 ns |   8.921 ns |  1.00 |    0.01 |      - |         - |          NA |
|                                        |                    |                    |        |            |           |           |            |       |         |        |           |             |
| Utf8CString_ToString_UnknownLength     | .NET 10.0          | .NET 10.0          | 128    |  15.391 ns | 0.3397 ns | 0.4043 ns |  15.153 ns |  0.27 |    0.01 | 0.0167 |     280 B |        0.97 |
| Utf8CString_ToString_UnknownLength     | .NET 6.0           | .NET 6.0           | 128    |  22.772 ns | 0.4725 ns | 0.4853 ns |  22.703 ns |  0.40 |    0.01 | 0.0167 |     280 B |        0.97 |
| Utf8CString_ToString_UnknownLength     | .NET 8.0           | .NET 8.0           | 128    |  18.503 ns | 0.0928 ns | 0.0868 ns |  18.505 ns |  0.33 |    0.00 | 0.0167 |     280 B |        0.97 |
| Utf8CString_ToString_UnknownLength     | .NET Framework 4.8 | .NET Framework 4.8 | 128    |  56.372 ns | 0.3968 ns | 0.3712 ns |  56.365 ns |  1.00 |    0.01 | 0.0459 |     289 B |        1.00 |
|                                        |                    |                    |        |            |           |           |            |       |         |        |           |             |
| NativeUtf8CString_ToString_KnownLength | .NET 10.0          | .NET 10.0          | 128    |  15.212 ns | 0.0758 ns | 0.0633 ns |  15.239 ns |  0.33 |    0.00 | 0.0167 |     280 B |        0.97 |
| NativeUtf8CString_ToString_KnownLength | .NET 6.0           | .NET 6.0           | 128    |  16.496 ns | 0.1417 ns | 0.1183 ns |  16.462 ns |  0.36 |    0.00 | 0.0167 |     280 B |        0.97 |
| NativeUtf8CString_ToString_KnownLength | .NET 8.0           | .NET 8.0           | 128    |  14.162 ns | 0.0691 ns | 0.0613 ns |  14.166 ns |  0.31 |    0.00 | 0.0167 |     280 B |        0.97 |
| NativeUtf8CString_ToString_KnownLength | .NET Framework 4.8 | .NET Framework 4.8 | 128    |  45.648 ns | 0.1352 ns | 0.1264 ns |  45.690 ns |  1.00 |    0.00 | 0.0459 |     289 B |        1.00 |
|                                        |                    |                    |        |            |           |           |            |       |         |        |           |             |
| Baseline_Marshal_PtrToStringUTF8       | .NET 10.0          | .NET 10.0          | 128    |  14.157 ns | 0.0651 ns | 0.0608 ns |  14.153 ns |  0.22 |    0.00 | 0.0167 |     280 B |        0.97 |
| Baseline_Marshal_PtrToStringUTF8       | .NET 6.0           | .NET 6.0           | 128    |  18.251 ns | 0.0848 ns | 0.0794 ns |  18.232 ns |  0.28 |    0.00 | 0.0167 |     280 B |        0.97 |
| Baseline_Marshal_PtrToStringUTF8       | .NET 8.0           | .NET 8.0           | 128    |  15.265 ns | 0.0654 ns | 0.0612 ns |  15.252 ns |  0.24 |    0.00 | 0.0167 |     280 B |        0.97 |
| Baseline_Marshal_PtrToStringUTF8       | .NET Framework 4.8 | .NET Framework 4.8 | 128    |  64.381 ns | 0.4846 ns | 0.4296 ns |  64.338 ns |  1.00 |    0.01 | 0.0459 |     289 B |        1.00 |
|                                        |                    |                    |        |            |           |           |            |       |         |        |           |             |
| Utf16CString_ToString                  | .NET 10.0          | .NET 10.0          | 128    |  10.677 ns | 0.0722 ns | 0.0640 ns |  10.680 ns |  0.40 |    0.00 | 0.0167 |     280 B |        0.97 |
| Utf16CString_ToString                  | .NET 6.0           | .NET 6.0           | 128    |  15.260 ns | 0.1274 ns | 0.1191 ns |  15.255 ns |  0.58 |    0.00 | 0.0167 |     280 B |        0.97 |
| Utf16CString_ToString                  | .NET 8.0           | .NET 8.0           | 128    |  12.605 ns | 0.1341 ns | 0.1254 ns |  12.663 ns |  0.48 |    0.00 | 0.0167 |     280 B |        0.97 |
| Utf16CString_ToString                  | .NET Framework 4.8 | .NET Framework 4.8 | 128    |  26.392 ns | 0.0596 ns | 0.0558 ns |  26.380 ns |  1.00 |    0.00 | 0.0459 |     289 B |        1.00 |
|                                        |                    |                    |        |            |           |           |            |       |         |        |           |             |
| Baseline_Marshal_PtrToStringUni        | .NET 10.0          | .NET 10.0          | 128    |  10.699 ns | 0.1335 ns | 0.1249 ns |  10.716 ns |  0.51 |    0.01 | 0.0167 |     280 B |        0.97 |
| Baseline_Marshal_PtrToStringUni        | .NET 6.0           | .NET 6.0           | 128    |  10.958 ns | 0.0584 ns | 0.0518 ns |  10.955 ns |  0.52 |    0.00 | 0.0167 |     280 B |        0.97 |
| Baseline_Marshal_PtrToStringUni        | .NET 8.0           | .NET 8.0           | 128    |  10.797 ns | 0.1805 ns | 0.1689 ns |  10.777 ns |  0.52 |    0.01 | 0.0167 |     280 B |        0.97 |
| Baseline_Marshal_PtrToStringUni        | .NET Framework 4.8 | .NET Framework 4.8 | 128    |  20.947 ns | 0.0497 ns | 0.0440 ns |  20.955 ns |  1.00 |    0.00 | 0.0459 |     289 B |        1.00 |
|                                        |                    |                    |        |            |           |           |            |       |         |        |           |             |
| **Utf8CString_AsReadOnlyView**             | **.NET 10.0**          | **.NET 10.0**          | **2048**   |   **7.574 ns** | **0.0796 ns** | **0.0744 ns** |   **7.531 ns** |  **0.22** |    **0.00** |      **-** |         **-** |          **NA** |
| Utf8CString_AsReadOnlyView             | .NET 6.0           | .NET 6.0           | 2048   |  15.930 ns | 0.2207 ns | 0.2064 ns |  15.870 ns |  0.47 |    0.01 |      - |         - |          NA |
| Utf8CString_AsReadOnlyView             | .NET 8.0           | .NET 8.0           | 2048   |   9.844 ns | 0.0883 ns | 0.0982 ns |   9.834 ns |  0.29 |    0.00 |      - |         - |          NA |
| Utf8CString_AsReadOnlyView             | .NET Framework 4.8 | .NET Framework 4.8 | 2048   |  34.069 ns | 0.4786 ns | 0.4477 ns |  33.951 ns |  1.00 |    0.02 |      - |         - |          NA |
|                                        |                    |                    |        |            |           |           |            |       |         |        |           |             |
| Utf16CString_AsReadOnlyView            | .NET 10.0          | .NET 10.0          | 2048   |  18.644 ns | 0.2164 ns | 0.1919 ns |  18.604 ns |  0.46 |    0.01 |      - |         - |          NA |
| Utf16CString_AsReadOnlyView            | .NET 6.0           | .NET 6.0           | 2048   |  30.582 ns | 0.0858 ns | 0.0802 ns |  30.554 ns |  0.75 |    0.01 |      - |         - |          NA |
| Utf16CString_AsReadOnlyView            | .NET 8.0           | .NET 8.0           | 2048   |  19.901 ns | 0.0488 ns | 0.0407 ns |  19.910 ns |  0.49 |    0.01 |      - |         - |          NA |
| Utf16CString_AsReadOnlyView            | .NET Framework 4.8 | .NET Framework 4.8 | 2048   |  40.928 ns | 0.5871 ns | 0.5204 ns |  40.708 ns |  1.00 |    0.02 |      - |         - |          NA |
|                                        |                    |                    |        |            |           |           |            |       |         |        |           |             |
| Utf8CString_ToString_UnknownLength     | .NET 10.0          | .NET 10.0          | 2048   |  90.877 ns | 1.4585 ns | 1.3643 ns |  90.793 ns |  0.17 |    0.00 | 0.2462 |    4120 B |        0.99 |
| Utf8CString_ToString_UnknownLength     | .NET 6.0           | .NET 6.0           | 2048   | 153.621 ns | 2.5183 ns | 2.3557 ns | 153.332 ns |  0.29 |    0.00 | 0.2460 |    4120 B |        0.99 |
| Utf8CString_ToString_UnknownLength     | .NET 8.0           | .NET 8.0           | 2048   |  93.750 ns | 1.1652 ns | 1.0899 ns |  94.188 ns |  0.18 |    0.00 | 0.2462 |    4120 B |        0.99 |
| Utf8CString_ToString_UnknownLength     | .NET Framework 4.8 | .NET Framework 4.8 | 2048   | 530.979 ns | 1.8246 ns | 1.4245 ns | 531.084 ns |  1.00 |    0.00 | 0.6571 |    4142 B |        1.00 |
|                                        |                    |                    |        |            |           |           |            |       |         |        |           |             |
| NativeUtf8CString_ToString_KnownLength | .NET 10.0          | .NET 10.0          | 2048   |  86.435 ns | 1.6881 ns | 2.0095 ns |  86.904 ns |  0.17 |    0.00 | 0.2462 |    4120 B |        0.99 |
| NativeUtf8CString_ToString_KnownLength | .NET 6.0           | .NET 6.0           | 2048   | 147.773 ns | 2.9262 ns | 2.5940 ns | 147.145 ns |  0.29 |    0.01 | 0.2460 |    4120 B |        0.99 |
| NativeUtf8CString_ToString_KnownLength | .NET 8.0           | .NET 8.0           | 2048   |  82.763 ns | 1.2059 ns | 1.1280 ns |  83.030 ns |  0.16 |    0.00 | 0.2462 |    4120 B |        0.99 |
| NativeUtf8CString_ToString_KnownLength | .NET Framework 4.8 | .NET Framework 4.8 | 2048   | 510.379 ns | 8.6632 ns | 7.2342 ns | 507.544 ns |  1.00 |    0.02 | 0.6571 |    4142 B |        1.00 |
|                                        |                    |                    |        |            |           |           |            |       |         |        |           |             |
| Baseline_Marshal_PtrToStringUTF8       | .NET 10.0          | .NET 10.0          | 2048   |  89.332 ns | 1.7183 ns | 1.8385 ns |  89.796 ns |  0.10 |    0.00 | 0.2462 |    4120 B |        0.99 |
| Baseline_Marshal_PtrToStringUTF8       | .NET 6.0           | .NET 6.0           | 2048   | 144.113 ns | 1.1528 ns | 0.9001 ns | 144.305 ns |  0.16 |    0.00 | 0.2460 |    4120 B |        0.99 |
| Baseline_Marshal_PtrToStringUTF8       | .NET 8.0           | .NET 8.0           | 2048   |  90.301 ns | 1.3209 ns | 1.2356 ns |  90.572 ns |  0.10 |    0.00 | 0.2462 |    4120 B |        0.99 |
| Baseline_Marshal_PtrToStringUTF8       | .NET Framework 4.8 | .NET Framework 4.8 | 2048   | 885.569 ns | 4.3349 ns | 3.6198 ns | 885.038 ns |  1.00 |    0.01 | 0.6561 |    4142 B |        1.00 |
|                                        |                    |                    |        |            |           |           |            |       |         |        |           |             |
| Utf16CString_ToString                  | .NET 10.0          | .NET 10.0          | 2048   |  95.865 ns | 1.9544 ns | 3.9924 ns |  94.714 ns |  0.74 |    0.03 | 0.2462 |    4120 B |        0.99 |
| Utf16CString_ToString                  | .NET 6.0           | .NET 6.0           | 2048   | 116.159 ns | 2.1979 ns | 3.9067 ns | 115.538 ns |  0.90 |    0.03 | 0.2462 |    4120 B |        0.99 |
| Utf16CString_ToString                  | .NET 8.0           | .NET 8.0           | 2048   |  95.940 ns | 1.9295 ns | 4.1945 ns |  94.004 ns |  0.74 |    0.03 | 0.2462 |    4120 B |        0.99 |
| Utf16CString_ToString                  | .NET Framework 4.8 | .NET Framework 4.8 | 2048   | 129.623 ns | 1.3174 ns | 1.1001 ns | 129.360 ns |  1.00 |    0.01 | 0.6578 |    4142 B |        1.00 |
|                                        |                    |                    |        |            |           |           |            |       |         |        |           |             |
| Baseline_Marshal_PtrToStringUni        | .NET 10.0          | .NET 10.0          | 2048   |  94.280 ns | 1.7795 ns | 1.7477 ns |  94.641 ns |  0.34 |    0.01 | 0.2462 |    4120 B |        0.99 |
| Baseline_Marshal_PtrToStringUni        | .NET 6.0           | .NET 6.0           | 2048   | 108.709 ns | 1.9400 ns | 1.7198 ns | 108.700 ns |  0.39 |    0.01 | 0.2462 |    4120 B |        0.99 |
| Baseline_Marshal_PtrToStringUni        | .NET 8.0           | .NET 8.0           | 2048   |  98.383 ns | 1.8963 ns | 2.0290 ns |  98.527 ns |  0.35 |    0.01 | 0.2462 |    4120 B |        0.99 |
| Baseline_Marshal_PtrToStringUni        | .NET Framework 4.8 | .NET Framework 4.8 | 2048   | 280.267 ns | 2.4710 ns | 1.9292 ns | 279.434 ns |  1.00 |    0.01 | 0.6576 |    4142 B |        1.00 |
