using System.Diagnostics;
using System.Runtime.CompilerServices;
using SharpFFI.Primitives.Extensions;

namespace SharpFFI.Primitives.Utf8;

[DebuggerDisplay("{ToString(),raw}")]
public sealed unsafe class NativeUtf8CString : IEquatable<NativeUtf8CString>, IDisposable
{
    private byte* _pointer;
    private int _length;

    public static NativeUtf8CString FromString(string value) => value is null ? throw new ArgumentNullException(nameof(value)) : FromUtf16Span(value);

    public static NativeUtf8CString FromUtf16Span(ReadOnlySpan<char> value)
    {
        var indexOfNullTerminator = value.IndexOf('\0');

        if (indexOfNullTerminator >= 0)
            value = value.Slice(0, indexOfNullTerminator);

        var exactBytesCount = FrameworkExtensions.GetUtf8BytesCount(value) + 1;
        var pointer = FrameworkExtensions.NativeAlloc<byte>((nuint)exactBytesCount);
        var length = exactBytesCount - 1;

        var result = new NativeUtf8CString(pointer, length);

        FrameworkExtensions.GetUtf8Bytes(value, result.Span);
        pointer[length] = 0;

        return result;
    }

    public static NativeUtf8CString FromUtf8Span(ReadOnlySpan<byte> value)
    {
        var indexOfNullTerminator = value.IndexOf((byte)0);

        if (indexOfNullTerminator >= 0)
            value = value.Slice(0, indexOfNullTerminator);

        var exactBytesCount = value.Length + 1;
        var pointer = FrameworkExtensions.NativeAlloc<byte>((nuint)exactBytesCount);
        var length = value.Length;

        var result = new NativeUtf8CString(pointer, length);

        value.CopyTo(result.Span);
        pointer[length] = 0;

        return result;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private NativeUtf8CString(byte* pointer, int length)
    {
        _pointer = pointer;
        _length = length;
    }

    private Span<byte> Span
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => new(_pointer, _length);
    }

    public ref readonly byte this[int index]
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => ref Span[index];
    }

    public int Length
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => _length;
    }

    public bool IsEmpty
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => Length == 0;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ReadOnlyUtf8StringView AsReadOnlyView() => ReadOnlyUtf8StringView.FromPointerLength(_pointer, _length);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override bool Equals(object? obj) => obj is NativeUtf8CString other && Equals(other);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override int GetHashCode() => FrameworkExtensions.GetHashCode(Span);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool Equals(NativeUtf8CString? other)
    {
        if (ReferenceEquals(this, other)) return true;
        if (other is null) return false;
        if (_pointer == other._pointer) return true;
        if (_pointer is null || other._pointer is null) return false;
        return AsReadOnlyView().Equals(other.AsReadOnlyView());
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override string ToString() => FrameworkExtensions.ToString(Span);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool operator ==(NativeUtf8CString? left, NativeUtf8CString? right)
    {
        if (ReferenceEquals(left, right)) return true;
        if (left is null) return false;
        return left.Equals(right);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool operator !=(NativeUtf8CString? left, NativeUtf8CString? right) => !(left == right);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static implicit operator ReadOnlyUtf8CString(NativeUtf8CString? str) => str is null ? ReadOnlyUtf8CString.Null : ReadOnlyUtf8CString.FromPointer(str._pointer);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void ReleaseUnmanagedResources()
    {
        FrameworkExtensions.NativeFree(_pointer);
        _pointer = null;
        _length = 0;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Dispose()
    {
        ReleaseUnmanagedResources();
        GC.SuppressFinalize(this);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    ~NativeUtf8CString() => ReleaseUnmanagedResources();
}