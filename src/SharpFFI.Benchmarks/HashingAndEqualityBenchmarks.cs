using System.Runtime.InteropServices;
using System.Text;
using BenchmarkDotNet.Attributes;
using SharpFFI.Primitives.Utf8;

namespace SharpFFI.Benchmarks;

[MemoryDiagnoser]
public unsafe class HashingAndEqualityBenchmarks
{
    [Params(3, 6, 64, 1024)]
    public int ByteLength;

    private string _str = null!;
    private NativeUtf8CString _nativeUtf8 = null!;
    private byte[] _bytes1 = null!;
    private byte[] _bytes2 = null!;

    private byte* _cstrPtr;
    private byte* _cstrPtr1;
    private byte* _cstrPtr2;
    private ReadOnlyUtf8CString _cstr;
    private ReadOnlyUtf8CString _cstr1;
    private ReadOnlyUtf8CString _cstr2;

    [GlobalSetup]
    public void Setup()
    {
        _str = TestData.GetString(ByteLength, Charset.Ascii);
        _nativeUtf8 = NativeUtf8CString.FromString(_str);

        _bytes1 = Encoding.UTF8.GetBytes(_str);
        _bytes2 = (byte[])_bytes1.Clone();

        _cstrPtr = (byte*)Marshal.AllocHGlobal(_bytes1.Length + 1);
        Marshal.Copy(_bytes1, 0, (IntPtr)_cstrPtr, _bytes1.Length);
        _cstrPtr[_bytes1.Length] = 0;
        _cstr = ReadOnlyUtf8CString.FromPointer(_cstrPtr);

        _cstrPtr1 = (byte*)Marshal.AllocHGlobal(_bytes1.Length + 1);
        Marshal.Copy(_bytes1, 0, (IntPtr)_cstrPtr1, _bytes1.Length);
        _cstrPtr1[_bytes1.Length] = 0;
        _cstr1 = ReadOnlyUtf8CString.FromPointer(_cstrPtr1);

        _cstrPtr2 = (byte*)Marshal.AllocHGlobal(_bytes1.Length + 1);
        Marshal.Copy(_bytes1, 0, (IntPtr)_cstrPtr2, _bytes1.Length);
        _cstrPtr2[_bytes1.Length] = 0;
        _cstr2 = ReadOnlyUtf8CString.FromPointer(_cstrPtr2);
    }

    [GlobalCleanup]
    public void Cleanup()
    {
        _nativeUtf8?.Dispose();
        _nativeUtf8 = null!;

        if (_cstrPtr != null)
        {
            Marshal.FreeHGlobal((IntPtr)_cstrPtr);
            _cstrPtr = null;
        }

        if (_cstrPtr1 != null)
        {
            Marshal.FreeHGlobal((IntPtr)_cstrPtr1);
            _cstrPtr1 = null;
        }

        if (_cstrPtr2 != null)
        {
            Marshal.FreeHGlobal((IntPtr)_cstrPtr2);
            _cstrPtr2 = null;
        }
    }

    [Benchmark]
    public int NativeUtf8_GetHashCode_Marvin() => _nativeUtf8.GetHashCode();

    [Benchmark]
    public int ReadOnlyUtf8CString_GetHashCode() => _cstr.GetHashCode();

    [Benchmark]
    public int Baseline_ManagedString_GetHashCode() => _str.GetHashCode();

    [Benchmark]
    public bool StringView_Equals_SameContentDifferentPointers()
    {
        var view1 = ReadOnlyUtf8StringView.FromUtf8Span(_bytes1);
        var view2 = ReadOnlyUtf8StringView.FromUtf8Span(_bytes2);
        return view1.Equals(view2);
    }

    [Benchmark]
    public bool CString_Equals_SameContentDifferentPointers()
    {
        return _cstr1.Equals(_cstr2);
    }
}
