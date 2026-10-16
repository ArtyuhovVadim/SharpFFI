using System.Buffers;
using System.Text;
using BenchmarkDotNet.Attributes;
using SharpFFI.Primitives.Utf8;

namespace SharpFFI.Benchmarks;

[MemoryDiagnoser]
public class Utf8CharEnumeratorBenchmarks
{
    [Params(128, 2048)]
    public int Length;

    [Params(Charset.Ascii, Charset.Cyrillic, Charset.EmojiHeavy)]
    public Charset Charset;

    private byte[] _utf8Bytes = null!;

    [GlobalSetup]
    public void Setup()
    {
        _utf8Bytes = TestData.GetUtf8Bytes(Length, Charset);
    }

    [Benchmark]
    public int Foreach_Utf8StringView_CharEnumerator()
    {
        var view = ReadOnlyUtf8StringView.FromUtf8Span(_utf8Bytes);
        int sum = 0;
        foreach (char c in view)
        {
            sum += c;
        }
        return sum;
    }

    [Benchmark]
    public int Baseline_ToString_ThenForeachChar()
    {
        var view = ReadOnlyUtf8StringView.FromUtf8Span(_utf8Bytes);
        int sum = 0;
        foreach (char c in view.ToString())
        {
            sum += c;
        }
        return sum;
    }

    [Benchmark]
    public int Baseline_ArrayPool_EncodingGetChars()
    {
        int maxChars = Encoding.UTF8.GetMaxCharCount(_utf8Bytes.Length);
        char[] rented = ArrayPool<char>.Shared.Rent(maxChars);
        try
        {
            int charCount = Encoding.UTF8.GetChars(_utf8Bytes, 0, _utf8Bytes.Length, rented, 0);
            int sum = 0;
            for (int i = 0; i < charCount; i++)
            {
                sum += rented[i];
            }
            return sum;
        }
        finally
        {
            ArrayPool<char>.Shared.Return(rented);
        }
    }
}
