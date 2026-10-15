using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace SharpFFI.Primitives;

/// <summary>
/// Represents an 8-bit boolean value compatible with C <c>bool</c> / <c>_Bool</c> and C++ <c>bool</c> ABIs.
/// </summary>
[StructLayout(LayoutKind.Sequential)]
public readonly struct Bool8 : IEquatable<Bool8>, IComparable<Bool8>, IComparable
{
    /// <summary>
    /// Gets the underlying 8-bit value (1 for <see langword="true"/>, 0 for <see langword="false"/>).
    /// </summary>
    public readonly byte Value;

    /// <summary>
    /// Initializes a new instance of the <see cref="Bool8"/> struct from a managed <see cref="bool"/>.
    /// </summary>
    /// <param name="value">The boolean value.</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Bool8(bool value) => Value = value ? (byte)1 : (byte)0;

    /// <summary>
    /// Initializes a new instance of the <see cref="Bool8"/> struct from a raw byte value.
    /// Non-zero values are normalized to 1 (<see langword="true"/>).
    /// </summary>
    /// <param name="value">The byte value to convert.</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Bool8(byte value) => Value = value != 0 ? (byte)1 : (byte)0;

    /// <summary>
    /// Initializes a new instance of the <see cref="Bool8"/> struct from a <see cref="Bool32"/> value.
    /// </summary>
    /// <param name="value">The 32-bit boolean value to convert.</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Bool8(Bool32 value) => Value = value.Value != 0 ? (byte)1 : (byte)0;

    /// <summary>
    /// Indicates whether the current instance is equal to another <see cref="Bool8"/>.
    /// </summary>
    /// <param name="other">A <see cref="Bool8"/> to compare with this instance.</param>
    /// <returns><see langword="true"/> if both instances represent the same truth value; otherwise, <see langword="false"/>.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool Equals(Bool8 other) => Value != 0 == (other.Value != 0);
    
    /// <inheritdoc/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override bool Equals(object? obj) => obj is Bool8 other && Equals(other);

    /// <inheritdoc/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override int GetHashCode() => Value != 0 ? 1 : 0;

    /// <inheritdoc/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override string ToString() => Value != 0 ? "True" : "False";

    /// <summary>
    /// Compares this instance to another <see cref="Bool8"/>.
    /// </summary>
    /// <param name="other">A <see cref="Bool8"/> to compare with this instance.</param>
    /// <returns>A signed number indicating the relative order (<c>false &lt; true</c>).</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public int CompareTo(Bool8 other) => (Value != 0 ? 1 : 0).CompareTo(other.Value != 0 ? 1 : 0);

    /// <inheritdoc/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public int CompareTo(object? obj)
    {
        if (obj is null) return 1;
        return obj is Bool8 other ? CompareTo(other) : throw new ArgumentException($"Object must be of type {nameof(Bool8)}");
    }

    /// <summary>
    /// Implicitly converts a <see cref="Bool32"/> to a <see cref="Bool8"/>.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static implicit operator Bool8(Bool32 value) => new(value.Value != 0);

    /// <summary>
    /// Implicitly converts a <see cref="Bool8"/> to a managed <see cref="bool"/>.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static implicit operator bool(Bool8 value) => value.Value != 0;

    /// <summary>
    /// Implicitly converts a managed <see cref="bool"/> to a <see cref="Bool8"/>.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static implicit operator Bool8(bool value) => new(value);

    /// <summary>
    /// Computes the logical NOT of the specified <see cref="Bool8"/>.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool operator !(Bool8 value) => value.Value == 0;

    /// <summary>
    /// Determines whether two <see cref="Bool8"/> instances have the same value.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool operator ==(Bool8 left, Bool8 right) => left.Equals(right);

    /// <summary>
    /// Determines whether two <see cref="Bool8"/> instances have different values.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool operator !=(Bool8 left, Bool8 right) => !left.Equals(right);

    /// <summary>
    /// Determines whether one <see cref="Bool8"/> is strictly less than another (<c>false &lt; true</c>).
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool operator <(Bool8 left, Bool8 right) => (left.Value != 0 ? 1 : 0) < (right.Value != 0 ? 1 : 0);

    /// <summary>
    /// Determines whether one <see cref="Bool8"/> is strictly greater than another.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool operator >(Bool8 left, Bool8 right) => (left.Value != 0 ? 1 : 0) > (right.Value != 0 ? 1 : 0);

    /// <summary>
    /// Determines whether one <see cref="Bool8"/> is less than or equal to another.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool operator <=(Bool8 left, Bool8 right) => (left.Value != 0 ? 1 : 0) <= (right.Value != 0 ? 1 : 0);

    /// <summary>
    /// Determines whether one <see cref="Bool8"/> is greater than or equal to another.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool operator >=(Bool8 left, Bool8 right) => (left.Value != 0 ? 1 : 0) >= (right.Value != 0 ? 1 : 0);

    /// <summary>
    /// Determines whether the <see cref="Bool8"/> evaluates to <see langword="true"/>.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool operator true(Bool8 value) => value.Value != 0;

    /// <summary>
    /// Determines whether the <see cref="Bool8"/> evaluates to <see langword="false"/>.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool operator false(Bool8 value) => value.Value == 0;
}