using System.Runtime.InteropServices;
using System.Text;
using BenchmarkDotNet.Attributes;
using SharpFFI.Primitives.Utf16;
using SharpFFI.Primitives.Utf8;

namespace SharpFFI.Benchmarks;

[MemoryDiagnoser]
public unsafe class NullTerminatedScanAndDecodeBenchmarks
{
    [Params(16, 128, 2048)]
    public int Length;

    private byte* _utf8Ptr;
    private char* _utf16Ptr;
    private ReadOnlyUtf8CString _cstr;
    private ReadOnlyUtf16CString _utf16Cstr;
    private NativeUtf8CString _nativeStr = null!;

    [GlobalSetup]
    public void Setup()
    {
        string str = TestData.GetString(Length, Charset.Ascii);
        byte[] utf8Bytes = Encoding.UTF8.GetBytes(str);

        // UTF-8 native null-terminated buffer
        _utf8Ptr = (byte*)Marshal.AllocHGlobal(utf8Bytes.Length + 1);
        Marshal.Copy(utf8Bytes, 0, (IntPtr)_utf8Ptr, utf8Bytes.Length);
        _utf8Ptr[utf8Bytes.Length] = 0;
        _cstr = ReadOnlyUtf8CString.FromPointer(_utf8Ptr);

        // NativeUtf8CString with known cached length
        _nativeStr = NativeUtf8CString.FromString(str);

        // UTF-16 native null-terminated buffer
        char[] utf16Chars = str.ToCharArray();
        _utf16Ptr = (char*)Marshal.AllocHGlobal((utf16Chars.Length + 1) * sizeof(char));
        Marshal.Copy(utf16Chars, 0, (IntPtr)_utf16Ptr, utf16Chars.Length);
        _utf16Ptr[utf16Chars.Length] = '\0';
        _utf16Cstr = ReadOnlyUtf16CString.FromPointer(_utf16Ptr);
    }

    [GlobalCleanup]
    public void Cleanup()
    {
        if (_utf8Ptr != null)
        {
            Marshal.FreeHGlobal((IntPtr)_utf8Ptr);
            _utf8Ptr = null;
        }

        _nativeStr?.Dispose();
        _nativeStr = null!;

        if (_utf16Ptr != null)
        {
            Marshal.FreeHGlobal((IntPtr)_utf16Ptr);
            _utf16Ptr = null;
        }
    }

    [Benchmark]
    public int Utf8CString_AsReadOnlyView() => _cstr.AsReadOnlyView().Length;

    [Benchmark]
    public int Utf16CString_AsReadOnlyView() => _utf16Cstr.AsReadOnlyView().Length;

    [Benchmark]
    public string Utf8CString_ToString_UnknownLength() => _cstr.ToString();

    [Benchmark]
    public string NativeUtf8CString_ToString_KnownLength() => _nativeStr.ToString();

    [Benchmark]
    public string? Baseline_Marshal_PtrToStringUTF8()
    {
#if NET6_0_OR_GREATER
        return Marshal.PtrToStringUTF8((IntPtr)_cstr.Pointer);
#else
        byte* p = _cstr.Pointer;
        int len = 0;
        while (p[len] != 0) len++;
        return Encoding.UTF8.GetString(p, len);
#endif
    }

    [Benchmark]
    public string Utf16CString_ToString() => _utf16Cstr.ToString();

    [Benchmark]
    public string? Baseline_Marshal_PtrToStringUni() => Marshal.PtrToStringUni((IntPtr)_utf16Cstr.Pointer);
}
