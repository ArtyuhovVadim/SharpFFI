using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;

namespace SharpFFI.Primitives.Utf8;

[DebuggerDisplay("{ToString(),raw}")]
[StructLayout(LayoutKind.Sequential)]
public readonly unsafe struct ReadOnlyUtf8CString : IEquatable<ReadOnlyUtf8CString>
{
    public static readonly ReadOnlyUtf8CString Null = default;

    public readonly byte* Pointer;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ReadOnlyUtf8CString(byte* pointer) => Pointer = pointer;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ReadOnlyUtf8CString(nint pointer) => Pointer = (byte*)pointer;

    public bool IsEmpty
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => Pointer is null;
    }

    public ref readonly byte this[int index]
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => ref Pointer[index];
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ReadOnlyUtf8StringView AsReadOnlyView() => new(Pointer);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool Equals(ReadOnlyUtf8CString other)
    {
        if (Pointer == other.Pointer) return true;
        if (Pointer == null || other.Pointer == null) return false;
        return AsReadOnlyView().Equals(other.AsReadOnlyView());
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override bool Equals(object? obj) => obj is ReadOnlyUtf8CString other && Equals(other);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override int GetHashCode() => Pointer is null ? 0 : Marvin.ComputeHash32(AsReadOnlyView(), Marvin.DefaultSeed);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override string ToString() => Encoding.UTF8.GetString(AsReadOnlyView().Span);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool operator ==(ReadOnlyUtf8CString left, ReadOnlyUtf8CString right) => left.Equals(right);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool operator !=(ReadOnlyUtf8CString left, ReadOnlyUtf8CString right) => !left.Equals(right);
}