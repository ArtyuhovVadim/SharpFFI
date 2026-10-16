using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using BenchmarkDotNet.Attributes;
using SharpFFI.Primitives.Utf16;
using SharpFFI.Primitives.Utf8;

namespace SharpFFI.Benchmarks;

[MemoryDiagnoser]
public class ScopedCStringBenchmarks
{
    [Params(16, 96, 1024)]
    public int Length;

    [Params(Charset.Ascii, Charset.MixedUnicode)]
    public Charset Charset;

    private string _str = null!;
    private byte[] _utf8Bytes = null!;

    [GlobalSetup]
    public void Setup()
    {
        _str = TestData.GetString(Length, Charset);
        _utf8Bytes = Encoding.UTF8.GetBytes(_str);
    }

    [Benchmark]
    [MethodImpl(MethodImplOptions.NoInlining)]
    public int Utf8Scoped_FromString_StackBuffer()
    {
        Span<byte> buffer = stackalloc byte[256];
        using var s = Utf8ScopedCString.FromString(_str, buffer);
        return s.Length;
    }

    [Benchmark]
    [MethodImpl(MethodImplOptions.NoInlining)]
    public int Utf8Scoped_FromUtf8Span_StackBuffer()
    {
        Span<byte> buffer = stackalloc byte[256];
        using var s = Utf8ScopedCString.FromUtf8Span(_utf8Bytes, buffer);
        return s.Length;
    }

    [Benchmark]
    public int Baseline_Marshal_Utf8()
    {
#if NET6_0_OR_GREATER
        IntPtr ptr = Marshal.StringToCoTaskMemUTF8(_str);
        try
        {
            return (int)ptr;
        }
        finally
        {
            Marshal.FreeCoTaskMem(ptr);
        }
#else
        int byteCount = Encoding.UTF8.GetByteCount(_str);
        IntPtr ptr = Marshal.AllocHGlobal(byteCount + 1);
        try
        {
            byte[] bytes = Encoding.UTF8.GetBytes(_str);
            Marshal.Copy(bytes, 0, ptr, bytes.Length);
            Marshal.WriteByte(ptr, byteCount, 0);
            return byteCount;
        }
        finally
        {
            Marshal.FreeHGlobal(ptr);
        }
#endif
    }

    [Benchmark]
    [MethodImpl(MethodImplOptions.NoInlining)]
    public int Utf16Scoped_FromString_StackBuffer()
    {
        Span<char> buffer = stackalloc char[256];
        using var s = Utf16ScopedCString.FromString(_str, buffer);
        return s.Length;
    }

    [Benchmark]
    public int Baseline_Marshal_Utf16()
    {
        IntPtr ptr = Marshal.StringToHGlobalUni(_str);
        try
        {
            return (int)ptr;
        }
        finally
        {
            Marshal.FreeHGlobal(ptr);
        }
    }
}
