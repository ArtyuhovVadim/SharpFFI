using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using SharpFFI.Primitives.Extensions;

namespace SharpFFI.Primitives.Utf16;

[DebuggerDisplay("{ToString(),raw}")]
public unsafe class NativeUtf16CString : IEquatable<NativeUtf16CString>, IDisposable
{
    private readonly char* _pointer = null;
    private readonly int _length;

    public NativeUtf16CString(string? value)
    {
        if (value is null)
            return;

        _pointer = (char*)Marshal.AllocHGlobal(value.Length + 1);
        _length = value.Length - 1;
        value.AsSpan().CopyTo(new Span<char>(_pointer, value.Length));
        _pointer[value.Length - 1] = '\0';
    }

    private Span<char> Span
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => new(_pointer, _length);
    }

    public ref readonly char this[int index]
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
    public ReadOnlyUtf16StringView AsReadOnlyView() => new(_pointer, _length);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override bool Equals(object? obj) => obj is NativeUtf16CString other && Equals(other);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override int GetHashCode() => _pointer is null ? 0 : AsReadOnlyView().Span.GetMarvinHash();

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool Equals(NativeUtf16CString? other)
    {
        if (ReferenceEquals(this, other)) return true;
        if (other is null) return false;
        if (_pointer == other._pointer) return true;
        if (_pointer == null || other._pointer == null) return false;
        return AsReadOnlyView().Equals(other.AsReadOnlyView());
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override string ToString() => Span.ToString();

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool operator ==(NativeUtf16CString? left, NativeUtf16CString? right)
    {
        if (ReferenceEquals(left, right)) return true;
        if (left is null) return false;
        return left.Equals(right);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool operator !=(NativeUtf16CString? left, NativeUtf16CString? right) => !(left == right);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static implicit operator ReadOnlyUtf16CString(NativeUtf16CString str) => new(str._pointer);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void ReleaseUnmanagedResources() => Marshal.FreeHGlobal((nint)_pointer);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Dispose()
    {
        ReleaseUnmanagedResources();
        GC.SuppressFinalize(this);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    ~NativeUtf16CString() => ReleaseUnmanagedResources();
}