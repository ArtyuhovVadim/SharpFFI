using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace SharpFFI.Primitives;

[StructLayout(LayoutKind.Sequential)]
public readonly struct Bool8 : IEquatable<Bool8>, IComparable<Bool8>, IComparable
{
    public readonly byte Value;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Bool8(bool value) => Value = value ? (byte)1 : (byte)0;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Bool8(byte value) => Value = value != 0 ? (byte)1 : (byte)0;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Bool8(Bool32 value) => Value = value.Value != 0 ? (byte)1 : (byte)0;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool Equals(Bool8 other) => Value != 0 == (other.Value != 0);

    public override bool Equals(object? obj) => obj is Bool8 other && Equals(other);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override int GetHashCode() => Value != 0 ? 1 : 0;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override string ToString() => Value != 0 ? "True" : "False";

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public int CompareTo(Bool8 other) => (Value != 0 ? 1 : 0).CompareTo(other.Value != 0 ? 1 : 0);

    public int CompareTo(object? obj)
    {
        if (obj is null) return 1;
        return obj is Bool8 other ? CompareTo(other) : throw new ArgumentException($"Object must be of type {nameof(Bool8)}");
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static implicit operator Bool8(Bool32 value) => new(value.Value != 0);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static implicit operator bool(Bool8 value) => value.Value != 0;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static implicit operator Bool8(bool value) => new(value);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool operator !(Bool8 value) => value.Value == 0;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool operator ==(Bool8 left, Bool8 right) => left.Equals(right);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool operator !=(Bool8 left, Bool8 right) => !left.Equals(right);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool operator <(Bool8 left, Bool8 right) => (left.Value != 0 ? 1 : 0) < (right.Value != 0 ? 1 : 0);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool operator >(Bool8 left, Bool8 right) => (left.Value != 0 ? 1 : 0) > (right.Value != 0 ? 1 : 0);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool operator <=(Bool8 left, Bool8 right) => (left.Value != 0 ? 1 : 0) <= (right.Value != 0 ? 1 : 0);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool operator >=(Bool8 left, Bool8 right) => (left.Value != 0 ? 1 : 0) >= (right.Value != 0 ? 1 : 0);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool operator true(Bool8 value) => value.Value != 0;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool operator false(Bool8 value) => value.Value == 0;
}