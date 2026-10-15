using System.Diagnostics;
using System.Runtime.CompilerServices;
using SharpFFI.Primitives.Extensions;

namespace SharpFFI.Primitives.Utf16;

/// <summary>
/// Represents an unmanaged null-terminated UTF-16 string allocated on the native heap.
/// Owns the underlying native memory and frees it when disposed or finalized.
/// </summary>
[DebuggerDisplay("{ToString(),raw}")]
public sealed unsafe class NativeUtf16CString : IEquatable<NativeUtf16CString>, IDisposable
{
    private char* _pointer;
    private int _length;

    /// <summary>
    /// Creates a new <see cref="NativeUtf16CString"/> by copying a managed string into native memory.
    /// If the string contains embedded null characters, it is truncated at the first null character.
    /// </summary>
    /// <param name="value">The managed string to copy.</param>
    /// <returns>A new <see cref="NativeUtf16CString"/> containing the null-terminated UTF-16 characters.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="value"/> is <see langword="null"/>.</exception>
    public static NativeUtf16CString FromString(string value) => value is null ? throw new ArgumentNullException(nameof(value)) : FromUtf16Span(value);

    /// <summary>
    /// Creates a new <see cref="NativeUtf16CString"/> by copying a UTF-16 character span into native memory.
    /// If the span contains embedded null characters, it is truncated at the first null character.
    /// </summary>
    /// <param name="value">The UTF-16 character span to copy.</param>
    /// <returns>A new <see cref="NativeUtf16CString"/> containing the null-terminated UTF-16 characters.</returns>
    public static NativeUtf16CString FromUtf16Span(ReadOnlySpan<char> value)
    {
        var indexOfNullTerminator = value.IndexOf('\0');

        if (indexOfNullTerminator >= 0)
            value = value.Slice(0, indexOfNullTerminator);

        var exactBytesCount = (value.Length + 1) * sizeof(char);
        var pointer = FrameworkExtensions.NativeAlloc<char>((nuint)exactBytesCount);
        var length = value.Length;

        var result = new NativeUtf16CString(pointer, length);

        value.CopyTo(result.Span);
        pointer[length] = '\0';

        return result;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private NativeUtf16CString(char* pointer, int length)
    {
        _pointer = pointer;
        _length = length;
    }

    private Span<char> Span
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => new(_pointer, _length);
    }

    /// <summary>
    /// Gets a read-only reference to the UTF-16 character at the specified index.
    /// </summary>
    /// <param name="index">The zero-based index of the character to retrieve.</param>
    /// <exception cref="IndexOutOfRangeException"><paramref name="index"/> is negative or greater than or equal to <see cref="Length"/>.</exception>
    public ref readonly char this[int index]
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => ref Span[index];
    }

    /// <summary>
    /// Gets the number of UTF-16 characters in this string, excluding the null terminator.
    /// </summary>
    public int Length
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => _length;
    }

    /// <summary>
    /// Gets a value indicating whether this string is empty (<see cref="Length"/> is 0).
    /// </summary>
    public bool IsEmpty
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => Length == 0;
    }

    /// <summary>
    /// Returns a <see cref="ReadOnlyUtf16StringView"/> representing the content of this string (excluding the null terminator).
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ReadOnlyUtf16StringView AsReadOnlyView() => ReadOnlyUtf16StringView.FromPointerLength(_pointer, _length);

    /// <inheritdoc/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override bool Equals(object? obj) => obj is NativeUtf16CString other && Equals(other);

    /// <inheritdoc/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override int GetHashCode() => FrameworkExtensions.GetHashCode(Span);

    /// <summary>
    /// Determines whether this instance and another <see cref="NativeUtf16CString"/> have the same character content.
    /// </summary>
    /// <param name="other">The string to compare with this instance.</param>
    /// <returns><see langword="true"/> if both strings have equal character content; otherwise, <see langword="false"/>.</returns>
    public bool Equals(NativeUtf16CString? other)
    {
        if (ReferenceEquals(this, other)) return true;
        if (other is null) return false;
        if (_pointer == other._pointer) return true;
        if (_pointer is null || other._pointer is null) return false;
        return AsReadOnlyView().Equals(other.AsReadOnlyView());
    }

    /// <summary>
    /// Converts this UTF-16 string into a managed <see cref="string"/>.
    /// </summary>
    /// <returns>A managed string representing the content.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override string ToString() => FrameworkExtensions.ToString(Span);

    /// <summary>
    /// Determines whether two <see cref="NativeUtf16CString"/> instances have the same character content.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool operator ==(NativeUtf16CString? left, NativeUtf16CString? right)
    {
        if (ReferenceEquals(left, right)) return true;
        if (left is null) return false;
        return left.Equals(right);
    }

    /// <summary>
    /// Determines whether two <see cref="NativeUtf16CString"/> instances have different character content.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool operator !=(NativeUtf16CString? left, NativeUtf16CString? right) => !(left == right);

    /// <summary>
    /// Implicitly converts a <see cref="NativeUtf16CString"/> to an unmanaged <see cref="ReadOnlyUtf16CString"/> pointer wrapper.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static implicit operator ReadOnlyUtf16CString(NativeUtf16CString? str) => str is null ? ReadOnlyUtf16CString.Null : ReadOnlyUtf16CString.FromPointer(str._pointer);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void ReleaseUnmanagedResources()
    {
        FrameworkExtensions.NativeFree(_pointer);
        _pointer = null;
        _length = 0;
    }

    /// <summary>
    /// Releases the unmanaged memory allocated for this string.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Dispose()
    {
        ReleaseUnmanagedResources();
        GC.SuppressFinalize(this);
    }

    /// <summary>
    /// Finalizes an instance of the <see cref="NativeUtf16CString"/> class.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    ~NativeUtf16CString() => ReleaseUnmanagedResources();
}