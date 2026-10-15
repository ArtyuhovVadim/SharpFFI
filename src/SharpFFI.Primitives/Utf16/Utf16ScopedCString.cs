using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using SharpFFI.Primitives.Extensions;

#pragma warning disable CS0809

namespace SharpFFI.Primitives.Utf16;

/// <summary>
/// Represents a scoped, null-terminated UTF-16 string designed for interop scenarios.
/// Uses a caller-provided pinned or <c>stackalloc</c> buffer, falling back to unmanaged native memory if the buffer is insufficient.
/// </summary>
[DebuggerDisplay("{ToString(),raw}")]
[StructLayout(LayoutKind.Sequential)]
public unsafe ref struct Utf16ScopedCString
{
    private char* _pointer;
    private int _length;
    private bool _heapAllocated;

    /// <summary>
    /// Creates a new <see cref="Utf16ScopedCString"/> by copying a managed string.
    /// Uses <paramref name="pinnedBuffer"/> if sufficient; otherwise allocates unmanaged native memory.
    /// If the string contains embedded null characters, it is truncated at the first null character.
    /// </summary>
    /// <param name="value">The managed string to copy.</param>
    /// <param name="pinnedBuffer">An optional destination buffer, typically allocated on the stack via <c>stackalloc</c>.</param>
    /// <returns>A new <see cref="Utf16ScopedCString"/> instance.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="value"/> is <see langword="null"/>.</exception>
    public static Utf16ScopedCString FromString(string value, Span<char> pinnedBuffer = default) => value is null ? throw new ArgumentNullException(nameof(value)) : FromUtf16Span(value, pinnedBuffer);

    /// <summary>
    /// Creates a new <see cref="Utf16ScopedCString"/> by copying a UTF-16 character span.
    /// Uses <paramref name="pinnedBuffer"/> if sufficient; otherwise allocates unmanaged native memory.
    /// If the span contains embedded null characters, it is truncated at the first null character.
    /// </summary>
    /// <param name="value">The UTF-16 character span to copy.</param>
    /// <param name="pinnedBuffer">An optional destination buffer, typically allocated on the stack via <c>stackalloc</c>.</param>
    /// <returns>A new <see cref="Utf16ScopedCString"/> instance.</returns>
    public static Utf16ScopedCString FromUtf16Span(ReadOnlySpan<char> value, Span<char> pinnedBuffer = default)
    {
        var indexOfNullTerminator = value.IndexOf('\0');

        if (indexOfNullTerminator >= 0)
            value = value.Slice(0, indexOfNullTerminator);

        var exactCharsCount = value.Length + 1;
        var heapAllocated = exactCharsCount > pinnedBuffer.Length;
        var pointer = heapAllocated ? FrameworkExtensions.NativeAlloc<char>((nuint)exactCharsCount * sizeof(char)) : pinnedBuffer.AsPointer();
        var length = value.Length;

        var result = new Utf16ScopedCString(pointer, length, heapAllocated);

        value.CopyTo(result.Span);
        pointer[length] = '\0';

        return result;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private Utf16ScopedCString(char* pointer, int length, bool heapAllocated)
    {
        _pointer = pointer;
        _length = length;
        _heapAllocated = heapAllocated;
    }

    private readonly Span<char> Span
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => new(_pointer, _length);
    }

    /// <summary>
    /// Gets a read-only reference to the UTF-16 character at the specified index.
    /// </summary>
    /// <param name="index">The zero-based index of the character to retrieve.</param>
    /// <exception cref="IndexOutOfRangeException"><paramref name="index"/> is negative or greater than or equal to <see cref="Length"/>.</exception>
    public readonly ref readonly char this[int index]
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => ref Span[index];
    }

    /// <summary>
    /// Gets the number of UTF-16 characters in this string, excluding the null terminator.
    /// </summary>
    public readonly int Length
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => _length;
    }

    /// <summary>
    /// Gets a value indicating whether this string is empty (<see cref="Length"/> is 0).
    /// </summary>
    public readonly bool IsEmpty
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => Length == 0;
    }

    /// <summary>
    /// Returns a <see cref="ReadOnlyUtf16StringView"/> representing the content of this string (excluding the null terminator).
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public readonly ReadOnlyUtf16StringView AsReadOnlyView() => ReadOnlyUtf16StringView.FromPointerLength(_pointer, _length);

    /// <inheritdoc/>
    [EditorBrowsable(EditorBrowsableState.Never)]
    [Obsolete("Equals(object) on ref struct is not supported.", true)]
    public readonly override bool Equals(object? obj) => throw new NotSupportedException();

    /// <inheritdoc/>
    [EditorBrowsable(EditorBrowsableState.Never)]
    [Obsolete("GetHashCode() on ref struct is not supported.", true)]
    public readonly override int GetHashCode() => throw new NotSupportedException();

    /// <summary>
    /// Determines whether this instance and another <see cref="Utf16ScopedCString"/> have the same character content.
    /// </summary>
    /// <param name="other">The scoped string to compare with this instance.</param>
    /// <returns><see langword="true"/> if both strings have equal character content; otherwise, <see langword="false"/>.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public readonly bool Equals(Utf16ScopedCString other) => AsReadOnlyView().Equals(other.AsReadOnlyView());

    /// <summary>
    /// Converts this UTF-16 string into a managed <see cref="string"/>.
    /// </summary>
    /// <returns>A managed string representing the content.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public readonly override string ToString() => FrameworkExtensions.ToString(Span);

    /// <summary>
    /// Determines whether two <see cref="Utf16ScopedCString"/> instances have the same character content.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool operator ==(Utf16ScopedCString left, Utf16ScopedCString right) => left.Equals(right);

    /// <summary>
    /// Determines whether two <see cref="Utf16ScopedCString"/> instances have different character content.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool operator !=(Utf16ScopedCString left, Utf16ScopedCString right) => !left.Equals(right);

    /// <summary>
    /// Implicitly converts a <see cref="Utf16ScopedCString"/> to an unmanaged <see cref="ReadOnlyUtf16CString"/> pointer wrapper.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static implicit operator ReadOnlyUtf16CString(Utf16ScopedCString str) => ReadOnlyUtf16CString.FromPointer(str._pointer);

    /// <summary>
    /// Releases unmanaged native heap memory if it was allocated.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Dispose()
    {
        if (_heapAllocated)
        {
            FrameworkExtensions.NativeFree(_pointer);
            _heapAllocated = false;
        }

        _pointer = null;
        _length = 0;
    }
}