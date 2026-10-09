using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using SharpFFI.Primitives.Extensions;

namespace SharpFFI.Primitives.Utf16;

[DebuggerDisplay("{ToString(),raw}")]
[StructLayout(LayoutKind.Sequential)]
public readonly unsafe struct Utf16CString : IEquatable<Utf16CString>
{
    public static readonly Utf16CString Null;

    public readonly char* Pointer;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Utf16CString(char* pointer) => Pointer = pointer;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Utf16CString(nint pointer) => Pointer = (char*)pointer;

    public bool IsEmpty
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => Pointer is null;
    }

    public ref char this[int index]
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => ref Pointer[index];
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Utf16StringView AsView() => new(Pointer);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ReadOnlyUtf16StringView AsReadOnlyView() => new(Pointer);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool Equals(Utf16CString other)
    {
        if (Pointer == other.Pointer) return true;
        if (Pointer == null || other.Pointer == null) return false;
        return AsReadOnlyView().Equals(other.AsReadOnlyView());
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override bool Equals(object? obj) => obj is Utf16CString other && Equals(other);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override int GetHashCode() => Pointer is null ? 0 : AsReadOnlyView().Span.GetMarvinHash();

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override string ToString() => AsReadOnlyView().ToString();

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool operator ==(Utf16CString left, Utf16CString right) => left.Equals(right);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool operator !=(Utf16CString left, Utf16CString right) => !left.Equals(right);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static implicit operator ReadOnlyUtf16CString(Utf16CString str) => new(str.Pointer);
}