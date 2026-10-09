using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using SharpFFI.Primitives.Extensions;

namespace SharpFFI.Primitives.Utf8;

[DebuggerDisplay("{ToString(),raw}")]
[StructLayout(LayoutKind.Sequential)]
public readonly unsafe struct Utf8CString : IEquatable<Utf8CString>
{
    public static readonly Utf8CString Null;

    public readonly byte* Pointer;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Utf8CString(byte* pointer) => Pointer = pointer;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Utf8CString(nint pointer) => Pointer = (byte*)pointer;

    public bool IsEmpty
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => Pointer is null;
    }

    public ref byte this[int index]
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => ref Pointer[index];
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Utf8StringView AsView() => new(Pointer);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ReadOnlyUtf8StringView AsReadOnlyView() => new(Pointer);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool Equals(Utf8CString other)
    {
        if (Pointer == other.Pointer) return true;
        if (Pointer == null || other.Pointer == null) return false;
        return AsView().Equals(other.AsView());
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override bool Equals(object? obj) => obj is Utf8CString other && Equals(other);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override int GetHashCode() => Pointer is null ? 0 : AsReadOnlyView().Span.GetMarvinHash();

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override string ToString() => Encoding.UTF8.GetString(AsView().Span);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool operator ==(Utf8CString left, Utf8CString right) => left.Equals(right);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool operator !=(Utf8CString left, Utf8CString right) => !left.Equals(right);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static implicit operator ReadOnlyUtf8CString(Utf8CString str) => new(str.Pointer);
}