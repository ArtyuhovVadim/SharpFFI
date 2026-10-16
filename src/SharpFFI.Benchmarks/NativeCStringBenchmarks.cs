using System.Text;
using BenchmarkDotNet.Attributes;
using SharpFFI.Primitives.Utf16;
using SharpFFI.Primitives.Utf8;

namespace SharpFFI.Benchmarks;

[MemoryDiagnoser]
public class NativeCStringBenchmarks
{
    [Params(16, 128, 2048)]
    public int Length;

    private string _str = null!;
    private byte[] _utf8Bytes = null!;
    private string _embeddedNullStr = null!;

    [GlobalSetup]
    public void Setup()
    {
        _str = TestData.GetString(Length, Charset.Ascii);
        _utf8Bytes = Encoding.UTF8.GetBytes(_str);

        var chars = new char[2048];
        for (int i = 0; i < chars.Length; i++)
        {
            chars[i] = 'a';
        }
        chars[16] = '\0';
        _embeddedNullStr = new string(chars);
    }

    [Benchmark]
    public int NativeUtf8_FromString_AndDispose()
    {
        using var s = NativeUtf8CString.FromString(_str);
        return s.Length;
    }

    [Benchmark]
    public int NativeUtf8_FromUtf8Span_AndDispose()
    {
        using var s = NativeUtf8CString.FromUtf8Span(_utf8Bytes);
        return s.Length;
    }

    [Benchmark]
    public int NativeUtf16_FromString_AndDispose()
    {
        using var s = NativeUtf16CString.FromString(_str);
        return s.Length;
    }

    [Benchmark]
    public int TruncationOverhead_WithEmbeddedNull()
    {
        using var s = NativeUtf8CString.FromString(_embeddedNullStr);
        return s.Length;
    }
}
