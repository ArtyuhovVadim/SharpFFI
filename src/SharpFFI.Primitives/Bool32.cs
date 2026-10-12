using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace SharpFFI.Primitives;

[StructLayout(LayoutKind.Sequential)]
public readonly struct Bool32 : IEquatable<Bool32>, IComparable<Bool32>, IComparable
{
    public readonly uint Value;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Bool32(bool value) => Value = value ? 1u : 0u;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Bool32(uint value) => Value = value != 0 ? 1u : 0u;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Bool32(Bool8 value) => Value = value.Value != 0 ? 1u : 0u;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool Equals(Bool32 other) => Value != 0 == (other.Value != 0);

    public override bool Equals(object? obj) => obj is Bool32 other && Equals(other);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override int GetHashCode() => Value != 0 ? 1 : 0;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override string ToString() => Value != 0 ? "True" : "False";

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public int CompareTo(Bool32 other) => (Value != 0 ? 1 : 0).CompareTo(other.Value != 0 ? 1 : 0);

    public int CompareTo(object? obj)
    {
        if (obj is null) return 1;
        return obj is Bool32 other ? CompareTo(other) : throw new ArgumentException($"Object must be of type {nameof(Bool32)}");
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static implicit operator Bool32(Bool8 value) => new(value);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static implicit operator bool(Bool32 value) => value.Value != 0;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static implicit operator Bool32(bool value) => new(value);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool operator !(Bool32 value) => value.Value == 0;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool operator ==(Bool32 left, Bool32 right) => left.Equals(right);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool operator !=(Bool32 left, Bool32 right) => !left.Equals(right);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool operator <(Bool32 left, Bool32 right) => (left.Value != 0 ? 1 : 0) < (right.Value != 0 ? 1 : 0);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool operator >(Bool32 left, Bool32 right) => (left.Value != 0 ? 1 : 0) > (right.Value != 0 ? 1 : 0);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool operator <=(Bool32 left, Bool32 right) => (left.Value != 0 ? 1 : 0) <= (right.Value != 0 ? 1 : 0);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool operator >=(Bool32 left, Bool32 right) => (left.Value != 0 ? 1 : 0) >= (right.Value != 0 ? 1 : 0);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool operator true(Bool32 value) => value.Value != 0;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool operator false(Bool32 value) => value.Value == 0;
}