```

BenchmarkDotNet v0.14.0, Windows 11 (10.0.22631.4460/23H2/2023Update/SunValley3)
Intel Core i9-14900F, 1 CPU, 32 logical and 24 physical cores
.NET SDK 9.0.100
  [Host]     : .NET 9.0.0 (9.0.24.52809), X64 RyuJIT AVX2
  DefaultJob : .NET 9.0.0 (9.0.24.52809), X64 RyuJIT AVX2


```
| Method | Mean      | Error     | StdDev    |
|------- |----------:|----------:|----------:|
| Sha256 |  3.914 μs | 0.0782 μs | 0.1146 μs |
| Md5    | 12.499 μs | 0.2442 μs | 0.2999 μs |
