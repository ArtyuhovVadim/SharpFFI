using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using SharpFFI.Primitives.Extensions;

#pragma warning disable CS0809

namespace SharpFFI.Primitives.Utf16;

/// <summary>
/// Represents a mutable, stack-only view over a UTF-16 character buffer.
/// </summary>
[DebuggerDisplay("{ToString(),raw}")]
[StructLayout(LayoutKind.Sequential)]
public readonly unsafe ref struct Utf16StringView
{
    /// <summary>
    /// Gets an empty <see cref="Utf16StringView"/>.
    /// </summary>
    public static Utf16StringView Empty => default;

    /// <summary>
    /// The underlying character span represented by this view.
    /// </summary>
    public readonly Span<char> Span;

    /// <summary>
    /// Creates a <see cref="Utf16StringView"/> wrapping the specified character span.
    /// </summary>
    /// <param name="value">The character span to wrap.</param>
    /// <returns>A new <see cref="Utf16StringView"/> instance.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Utf16StringView FromUtf16Span(Span<char> value) => new(value);

    /// <summary>
    /// Creates a <see cref="Utf16StringView"/> from a pointer and explicit character length.
    /// </summary>
    /// <param name="pointer">The pointer to the start of the UTF-16 characters.</param>
    /// <param name="length">The number of characters in the view.</param>
    /// <returns>A new <see cref="Utf16StringView"/> instance.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Utf16StringView FromPointerLength(char* pointer, int length) => new(pointer, length);

    /// <summary>
    /// Creates a <see cref="Utf16StringView"/> from an unmanaged integer pointer and explicit character length.
    /// </summary>
    /// <param name="pointer">The pointer to the start of the UTF-16 characters.</param>
    /// <param name="length">The number of characters in the view.</param>
    /// <returns>A new <see cref="Utf16StringView"/> instance.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Utf16StringView FromPointerLength(nint pointer, int length) => new((char*)pointer, length);

    /// <summary>
    /// Creates a <see cref="Utf16StringView"/> from a null-terminated character pointer by scanning for the null terminator.
    /// </summary>
    /// <param name="pointer">The pointer to the null-terminated UTF-16 string.</param>
    /// <returns>A new <see cref="Utf16StringView"/> instance.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Utf16StringView FromPointer(char* pointer) => new(FrameworkExtensions.CreateSpanFromNullTerminated(pointer));

    /// <summary>
    /// Creates a <see cref="Utf16StringView"/> from a null-terminated unmanaged integer pointer by scanning for the null terminator.
    /// </summary>
    /// <param name="pointer">The pointer to the null-terminated UTF-16 string.</param>
    /// <returns>A new <see cref="Utf16StringView"/> instance.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Utf16StringView FromPointer(nint pointer) => new(FrameworkExtensions.CreateSpanFromNullTerminated((char*)pointer));

    /// <summary>
    /// Creates a <see cref="Utf16StringView"/> from a <see cref="Utf16CString"/> by scanning for the null terminator.
    /// </summary>
    /// <param name="value">The null-terminated UTF-16 C string.</param>
    /// <returns>A new <see cref="Utf16StringView"/> instance.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Utf16StringView FromUtf16CString(Utf16CString value) => new(FrameworkExtensions.CreateSpanFromNullTerminated(value.Pointer));

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private Utf16StringView(Span<char> value) => Span = value;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private Utf16StringView(char* pointer, int length) => Span = new Span<char>(pointer, length);

    /// <summary>
    /// Gets a mutable reference to the character at the specified index.
    /// </summary>
    /// <param name="index">The zero-based index of the character to retrieve.</param>
    /// <exception cref="IndexOutOfRangeException"><paramref name="index"/> is out of bounds.</exception>
    public ref char this[int index]
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => ref Span[index];
    }

    /// <summary>
    /// Gets the number of characters in this view.
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
    /// Clears the contents of this view by filling it with zeroes (<c>'\0'</c>).
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Clear() => Span.Clear();

    /// <summary>
    /// Fills the view with the specified character value.
    /// </summary>
    /// <param name="value">The character value to fill with.</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Fill(char value) => Span.Fill(value);

    /// <summary>
    /// Copies the contents of this view into a destination view.
    /// </summary>
    /// <param name="destination">The target view.</param>
    /// <exception cref="ArgumentException"><paramref name="destination"/> is shorter than this view.</exception>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void CopyTo(Utf16StringView destination) => Span.CopyTo(destination.Span);

    /// <summary>
    /// Attempts to copy the contents of this view into a destination view.
    /// </summary>
    /// <param name="destination">The target view.</param>
    /// <returns><see langword="true"/> if the copy succeeded; <see langword="false"/> if <paramref name="destination"/> is too short.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool TryCopyTo(Utf16StringView destination) => Span.TryCopyTo(destination.Span);

    /// <summary>
    /// Forms a slice out of the current view starting at a specified index.
    /// </summary>
    /// <param name="start">The zero-based start index.</param>
    /// <returns>A sliced view.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Utf16StringView Slice(int start) => new(Span.Slice(start));

    /// <summary>
    /// Forms a slice out of the current view starting at a specified index for a specified length.
    /// </summary>
    /// <param name="start">The zero-based start index.</param>
    /// <param name="length">The number of characters in the slice.</param>
    /// <returns>A sliced view.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Utf16StringView Slice(int start, int length) => new(Span.Slice(start, length));

    /// <summary>
    /// Copies the contents of this view to a new character array.
    /// </summary>
    /// <returns>A character array containing the characters of this view.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public char[] ToArray() => Span.ToArray();

    /// <inheritdoc/>
    [EditorBrowsable(EditorBrowsableState.Never)]
    [Obsolete("Equals(object) on ref struct is not supported.", true)]
    public override bool Equals(object? obj) => throw new NotSupportedException();

    /// <inheritdoc/>
    [EditorBrowsable(EditorBrowsableState.Never)]
    [Obsolete("GetHashCode() on ref struct is not supported.", true)]
    public override int GetHashCode() => throw new NotSupportedException();

    /// <summary>
    /// Determines whether this view and another view contain the same character sequence.
    /// </summary>
    /// <param name="other">The view to compare with this instance.</param>
    /// <returns><see langword="true"/> if both views have equal sequences; otherwise, <see langword="false"/>.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool Equals(Utf16StringView other) => Span == other.Span || Span.SequenceEqual(other.Span);

    /// <summary>
    /// Returns an enumerator that iterates through the characters in this view.
    /// </summary>
    /// <returns>A <see cref="ReadOnlySpan{Char}.Enumerator"/> for this view.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ReadOnlySpan<char>.Enumerator GetEnumerator() => ((ReadOnlySpan<char>)Span).GetEnumerator();

    /// <summary>
    /// Converts the characters in this view into a managed <see cref="string"/>.
    /// </summary>
    /// <returns>A managed string representing the content.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override string ToString() => FrameworkExtensions.ToString(Span);

    /// <summary>
    /// Determines whether two <see cref="Utf16StringView"/> instances contain the same character sequence.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool operator ==(Utf16StringView left, Utf16StringView right) => left.Equals(right);

    /// <summary>
    /// Determines whether two <see cref="Utf16StringView"/> instances contain different character sequences.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool operator !=(Utf16StringView left, Utf16StringView right) => !left.Equals(right);

    /// <summary>
    /// Implicitly converts a <see cref="Utf16StringView"/> to a <see cref="Span{Char}"/>.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static implicit operator Span<char>(Utf16StringView view) => view.Span;

    /// <summary>
    /// Implicitly converts a <see cref="Utf16StringView"/> to a <see cref="ReadOnlySpan{Char}"/>.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static implicit operator ReadOnlySpan<char>(Utf16StringView view) => view.Span;

    /// <summary>
    /// Implicitly converts a <see cref="Span{Char}"/> to a <see cref="Utf16StringView"/>.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static implicit operator Utf16StringView(Span<char> span) => new(span);

    /// <summary>
    /// Implicitly converts a <see cref="Utf16StringView"/> to a <see cref="ReadOnlyUtf16StringView"/>.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static implicit operator ReadOnlyUtf16StringView(Utf16StringView view) => ReadOnlyUtf16StringView.FromUtf16Span(view.Span);
}