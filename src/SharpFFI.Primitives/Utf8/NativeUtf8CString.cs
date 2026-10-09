using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using SharpFFI.Primitives.Extensions;

namespace SharpFFI.Primitives.Utf8;

[DebuggerDisplay("{ToString(),raw}")]
public unsafe class NativeUtf8CString : IEquatable<NativeUtf8CString>, IDisposable
{
    private readonly byte* _pointer = null;
    private readonly int _length;

    public NativeUtf8CString(string? value)
    {
        if (value is null)
            return;

        var valueBytesCount = Encoding.UTF8.GetByteCount(value) + 1;
        _pointer = (byte*)Marshal.AllocHGlobal(valueBytesCount);
        _length = valueBytesCount - 1;
        Encoding.UTF8.GetBytes(value, new Span<byte>(_pointer, valueBytesCount));
        _pointer[valueBytesCount - 1] = (byte)'\0';
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
        get => Span.IsEmpty ? 0 : Span.Length - 1;
    }

    public bool IsEmpty
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => Span.IsEmpty;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ReadOnlyUtf8StringView AsReadOnlyView() => new(_pointer, _length);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override bool Equals(object? obj) => obj is NativeUtf8CString other && Equals(other);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override int GetHashCode() => _pointer is null ? 0 : Span.GetMarvinHash();

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool Equals(NativeUtf8CString? other)
    {
        if (ReferenceEquals(this, other)) return true;
        if (other is null) return false;
        if (_pointer == other._pointer) return true;
        if (_pointer == null || other._pointer == null) return false;
        return AsReadOnlyView().Equals(other.AsReadOnlyView());
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override string ToString() => Encoding.UTF8.GetString(Span);

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
    public static implicit operator ReadOnlyUtf8CString(NativeUtf8CString str) => new(str._pointer);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void ReleaseUnmanagedResources() => Marshal.FreeHGlobal((nint)_pointer);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Dispose()
    {
        ReleaseUnmanagedResources();
        GC.SuppressFinalize(this);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    ~NativeUtf8CString() => ReleaseUnmanagedResources();
}