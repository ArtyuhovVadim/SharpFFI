using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using SharpFFI.Primitives.Extensions;

#pragma warning disable CS0809

namespace SharpFFI.Primitives.Utf8;

/// <summary>
/// Represents a read-only, stack-only view over a UTF-8 byte buffer.
/// </summary>
[DebuggerDisplay("{ToString(),raw}")]
[StructLayout(LayoutKind.Sequential)]
public readonly unsafe ref struct ReadOnlyUtf8StringView
{
    /// <summary>
    /// Gets an empty <see cref="ReadOnlyUtf8StringView"/>.
    /// </summary>
    public static ReadOnlyUtf8StringView Empty => default;

    /// <summary>
    /// The underlying read-only byte span represented by this view.
    /// </summary>
    public readonly ReadOnlySpan<byte> Span;

    /// <summary>
    /// Creates a <see cref="ReadOnlyUtf8StringView"/> wrapping the specified read-only byte span.
    /// </summary>
    /// <param name="value">The byte span to wrap.</param>
    /// <returns>A new <see cref="ReadOnlyUtf8StringView"/> instance.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ReadOnlyUtf8StringView FromUtf8Span(ReadOnlySpan<byte> value) => new(value);

    /// <summary>
    /// Creates a <see cref="ReadOnlyUtf8StringView"/> from a pointer and explicit byte length.
    /// </summary>
    /// <param name="pointer">The pointer to the start of the UTF-8 bytes.</param>
    /// <param name="length">The number of bytes in the view.</param>
    /// <returns>A new <see cref="ReadOnlyUtf8StringView"/> instance.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ReadOnlyUtf8StringView FromPointerLength(byte* pointer, int length) => new(pointer, length);

    /// <summary>
    /// Creates a <see cref="ReadOnlyUtf8StringView"/> from an unmanaged integer pointer and explicit byte length.
    /// </summary>
    /// <param name="pointer">The pointer to the start of the UTF-8 bytes.</param>
    /// <param name="length">The number of bytes in the view.</param>
    /// <returns>A new <see cref="ReadOnlyUtf8StringView"/> instance.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ReadOnlyUtf8StringView FromPointerLength(nint pointer, int length) => new((byte*)pointer, length);

    /// <summary>
    /// Creates a <see cref="ReadOnlyUtf8StringView"/> from a null-terminated byte pointer by scanning for the null terminator.
    /// </summary>
    /// <param name="pointer">The pointer to the null-terminated UTF-8 string.</param>
    /// <returns>A new <see cref="ReadOnlyUtf8StringView"/> instance.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ReadOnlyUtf8StringView FromPointer(byte* pointer) => new(FrameworkExtensions.CreateSpanFromNullTerminated(pointer));

    /// <summary>
    /// Creates a <see cref="ReadOnlyUtf8StringView"/> from a null-terminated unmanaged integer pointer by scanning for the null terminator.
    /// </summary>
    /// <param name="pointer">The pointer to the null-terminated UTF-8 string.</param>
    /// <returns>A new <see cref="ReadOnlyUtf8StringView"/> instance.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ReadOnlyUtf8StringView FromPointer(nint pointer) => new(FrameworkExtensions.CreateSpanFromNullTerminated((byte*)pointer));

    /// <summary>
    /// Creates a <see cref="ReadOnlyUtf8StringView"/> from a <see cref="Utf8CString"/> by scanning for the null terminator.
    /// </summary>
    /// <param name="value">The null-terminated UTF-8 C string.</param>
    /// <returns>A new <see cref="ReadOnlyUtf8StringView"/> instance.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ReadOnlyUtf8StringView FromUtf8CString(Utf8CString value) => new(FrameworkExtensions.CreateSpanFromNullTerminated(value.Pointer));

    /// <summary>
    /// Creates a <see cref="ReadOnlyUtf8StringView"/> from a <see cref="ReadOnlyUtf8CString"/> by scanning for the null terminator.
    /// </summary>
    /// <param name="value">The read-only null-terminated UTF-8 C string.</param>
    /// <returns>A new <see cref="ReadOnlyUtf8StringView"/> instance.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ReadOnlyUtf8StringView FromUtf8CString(ReadOnlyUtf8CString value) => new(FrameworkExtensions.CreateSpanFromNullTerminated(value.Pointer));

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private ReadOnlyUtf8StringView(ReadOnlySpan<byte> value) => Span = value;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private ReadOnlyUtf8StringView(byte* pointer, int length) => Span = new ReadOnlySpan<byte>(pointer, length);

    /// <summary>
    /// Gets a read-only reference to the byte at the specified index.
    /// </summary>
    /// <param name="index">The zero-based index of the byte to retrieve.</param>
    /// <exception cref="IndexOutOfRangeException"><paramref name="index"/> is out of bounds.</exception>
    public ref readonly byte this[int index]
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => ref Span[index];
    }

    /// <summary>
    /// Gets the number of bytes in this view.
    /// </summary>
    public int Length
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => Span.Length;
    }

    /// <summary>
    /// Gets a value indicating whether this view is empty.
    /// </summary>
    public bool IsEmpty
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => Span.IsEmpty;
    }

    /// <summary>
    /// Copies the contents of this view into a destination mutable view.
    /// </summary>
    /// <param name="destination">The target view.</param>
    /// <exception cref="ArgumentException"><paramref name="destination"/> is shorter than this view.</exception>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void CopyTo(Utf8StringView destination) => Span.CopyTo(destination.Span);

    /// <summary>
    /// Attempts to copy the contents of this view into a destination mutable view.
    /// </summary>
    /// <param name="destination">The target view.</param>
    /// <returns><see langword="true"/> if the copy succeeded; <see langword="false"/> if <paramref name="destination"/> is too short.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool TryCopyTo(Utf8StringView destination) => Span.TryCopyTo(destination.Span);

    /// <summary>
    /// Forms a slice out of the current view starting at a specified index.
    /// </summary>
    /// <param name="start">The zero-based start index.</param>
    /// <returns>A sliced view.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ReadOnlyUtf8StringView Slice(int start) => new(Span.Slice(start));

    /// <summary>
    /// Forms a slice out of the current view starting at a specified index for a specified length.
    /// </summary>
    /// <param name="start">The zero-based start index.</param>
    /// <param name="length">The number of bytes in the slice.</param>
    /// <returns>A sliced view.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ReadOnlyUtf8StringView Slice(int start, int length) => new(Span.Slice(start, length));

    /// <summary>
    /// Copies the contents of this view to a new byte array.
    /// </summary>
    /// <returns>A byte array containing the bytes of this view.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public byte[] ToArray() => Span.ToArray();

    /// <inheritdoc/>
    [EditorBrowsable(EditorBrowsableState.Never)]
    [Obsolete("Equals(object) on ref struct is not supported.", true)]
    public override bool Equals(object? obj) => throw new NotSupportedException();

    /// <inheritdoc/>
    [EditorBrowsable(EditorBrowsableState.Never)]
    [Obsolete("GetHashCode() on ref struct is not supported.", true)]
    public override int GetHashCode() => throw new NotSupportedException();

    /// <summary>
    /// Determines whether this view and another view contain the same byte sequence.
    /// </summary>
    /// <param name="other">The view to compare with this instance.</param>
    /// <returns><see langword="true"/> if both views have equal sequences; otherwise, <see langword="false"/>.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool Equals(ReadOnlyUtf8StringView other) => Span == other.Span || Span.SequenceEqual(other.Span);

    /// <summary>
    /// Returns an enumerator that iterates through the UTF-8 bytes as decoded UTF-16 characters.
    /// </summary>
    /// <returns>A <see cref="Utf8CharEnumerator"/> for this view.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Utf8CharEnumerator GetEnumerator() => new(Span);

    /// <summary>
    /// Decodes the UTF-8 bytes in this view into a managed <see cref="string"/>.
    /// </summary>
    /// <returns>A managed string representing the decoded content.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override string ToString() => FrameworkExtensions.ToString(Span);

    /// <summary>
    /// Determines whether two <see cref="ReadOnlyUtf8StringView"/> instances contain the same byte sequence.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool operator ==(ReadOnlyUtf8StringView left, ReadOnlyUtf8StringView right) => left.Equals(right);

    /// <summary>
    /// Determines whether two <see cref="ReadOnlyUtf8StringView"/> instances contain different byte sequences.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool operator !=(ReadOnlyUtf8StringView left, ReadOnlyUtf8StringView right) => !left.Equals(right);

    /// <summary>
    /// Implicitly converts a <see cref="ReadOnlyUtf8StringView"/> to a <see cref="ReadOnlySpan{Byte}"/>.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static implicit operator ReadOnlySpan<byte>(ReadOnlyUtf8StringView view) => view.Span;

    /// <summary>
    /// Implicitly converts a <see cref="ReadOnlySpan{Byte}"/> to a <see cref="ReadOnlyUtf8StringView"/>.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static implicit operator ReadOnlyUtf8StringView(ReadOnlySpan<byte> span) => new(span);
}