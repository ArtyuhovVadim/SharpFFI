using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace SharpFFI.Primitives;

/// <summary>
/// Represents a 32-bit boolean value compatible with Win32 <c>BOOL</c> and C/C++ 4-byte boolean ABIs.
/// </summary>
[StructLayout(LayoutKind.Sequential)]
public readonly struct Bool32 : IEquatable<Bool32>, IComparable<Bool32>, IComparable
{
    /// <summary>
    /// Gets the underlying 32-bit unsigned integer value (1 for <see langword="true"/>, 0 for <see langword="false"/>).
    /// </summary>
    public readonly uint Value;

    /// <summary>
    /// Initializes a new instance of the <see cref="Bool32"/> struct from a managed <see cref="bool"/>.
    /// </summary>
    /// <param name="value">The boolean value.</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Bool32(bool value) => Value = value ? 1u : 0u;

    /// <summary>
    /// Initializes a new instance of the <see cref="Bool32"/> struct from a raw 32-bit unsigned integer value.
    /// Non-zero values are normalized to 1 (<see langword="true"/>).
    /// </summary>
    /// <param name="value">The integer value to convert.</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Bool32(uint value) => Value = value != 0 ? 1u : 0u;

    /// <summary>
    /// Initializes a new instance of the <see cref="Bool32"/> struct from a <see cref="Bool8"/> value.
    /// </summary>
    /// <param name="value">The 8-bit boolean value to convert.</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Bool32(Bool8 value) => Value = value.Value != 0 ? 1u : 0u;

    /// <summary>
    /// Indicates whether the current instance is equal to another <see cref="Bool32"/>.
    /// </summary>
    /// <param name="other">A <see cref="Bool32"/> to compare with this instance.</param>
    /// <returns><see langword="true"/> if both instances represent the same truth value; otherwise, <see langword="false"/>.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool Equals(Bool32 other) => Value != 0 == (other.Value != 0);

    /// <inheritdoc/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override bool Equals(object? obj) => obj is Bool32 other && Equals(other);

    /// <inheritdoc/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override int GetHashCode() => Value != 0 ? 1 : 0;

    /// <inheritdoc/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override string ToString() => Value != 0 ? "True" : "False";

    /// <summary>
    /// Compares this instance to another <see cref="Bool32"/>.
    /// </summary>
    /// <param name="other">A <see cref="Bool32"/> to compare with this instance.</param>
    /// <returns>A signed number indicating the relative order (<c>false &lt; true</c>).</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public int CompareTo(Bool32 other) => (Value != 0 ? 1 : 0).CompareTo(other.Value != 0 ? 1 : 0);

    /// <inheritdoc/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public int CompareTo(object? obj)
    {
        if (obj is null) return 1;
        return obj is Bool32 other ? CompareTo(other) : throw new ArgumentException($"Object must be of type {nameof(Bool32)}");
    }

    /// <summary>
    /// Implicitly converts a <see cref="Bool8"/> to a <see cref="Bool32"/>.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static implicit operator Bool32(Bool8 value) => new(value);

    /// <summary>
    /// Implicitly converts a <see cref="Bool32"/> to a managed <see cref="bool"/>.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static implicit operator bool(Bool32 value) => value.Value != 0;

    /// <summary>
    /// Implicitly converts a managed <see cref="bool"/> to a <see cref="Bool32"/>.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static implicit operator Bool32(bool value) => new(value);

    /// <summary>
    /// Computes the logical NOT of the specified <see cref="Bool32"/>.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool operator !(Bool32 value) => value.Value == 0;

    /// <summary>
    /// Determines whether two <see cref="Bool32"/> instances have the same value.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool operator ==(Bool32 left, Bool32 right) => left.Equals(right);

    /// <summary>
    /// Determines whether two <see cref="Bool32"/> instances have different values.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool operator !=(Bool32 left, Bool32 right) => !left.Equals(right);

    /// <summary>
    /// Determines whether one <see cref="Bool32"/> is strictly less than another (<c>false &lt; true</c>).
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool operator <(Bool32 left, Bool32 right) => (left.Value != 0 ? 1 : 0) < (right.Value != 0 ? 1 : 0);

    /// <summary>
    /// Determines whether one <see cref="Bool32"/> is strictly greater than another.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool operator >(Bool32 left, Bool32 right) => (left.Value != 0 ? 1 : 0) > (right.Value != 0 ? 1 : 0);

    /// <summary>
    /// Determines whether one <see cref="Bool32"/> is less than or equal to another.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool operator <=(Bool32 left, Bool32 right) => (left.Value != 0 ? 1 : 0) <= (right.Value != 0 ? 1 : 0);

    /// <summary>
    /// Determines whether one <see cref="Bool32"/> is greater than or equal to another.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool operator >=(Bool32 left, Bool32 right) => (left.Value != 0 ? 1 : 0) >= (right.Value != 0 ? 1 : 0);

    /// <summary>
    /// Determines whether the <see cref="Bool32"/> evaluates to <see langword="true"/>.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool operator true(Bool32 value) => value.Value != 0;

    /// <summary>
    /// Determines whether the <see cref="Bool32"/> evaluates to <see langword="false"/>.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool operator false(Bool32 value) => value.Value == 0;
}