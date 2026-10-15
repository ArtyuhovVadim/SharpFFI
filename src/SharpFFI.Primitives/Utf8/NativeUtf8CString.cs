using System.Diagnostics;
using System.Runtime.CompilerServices;
using SharpFFI.Primitives.Extensions;

namespace SharpFFI.Primitives.Utf8;

/// <summary>
/// Represents an unmanaged null-terminated UTF-8 string allocated on the native heap.
/// Owns the underlying native memory and frees it when disposed or finalized.
/// </summary>
[DebuggerDisplay("{ToString(),raw}")]
public sealed unsafe class NativeUtf8CString : IEquatable<NativeUtf8CString>, IDisposable
{
    private byte* _pointer;
    private int _length;

    /// <summary>
    /// Creates a new <see cref="NativeUtf8CString"/> by encoding a managed string into UTF-8.
    /// If the string contains embedded null characters, it is truncated at the first null character.
    /// </summary>
    /// <param name="value">The managed string to encode.</param>
    /// <returns>A new <see cref="NativeUtf8CString"/> containing the null-terminated UTF-8 bytes.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="value"/> is <see langword="null"/>.</exception>
    public static NativeUtf8CString FromString(string value) => value is null ? throw new ArgumentNullException(nameof(value)) : FromUtf16Span(value);

    /// <summary>
    /// Creates a new <see cref="NativeUtf8CString"/> by encoding a UTF-16 character span into UTF-8.
    /// If the span contains embedded null characters, it is truncated at the first null character.
    /// </summary>
    /// <param name="value">The UTF-16 character span to encode.</param>
    /// <returns>A new <see cref="NativeUtf8CString"/> containing the null-terminated UTF-8 bytes.</returns>
    public static NativeUtf8CString FromUtf16Span(ReadOnlySpan<char> value)
    {
        var indexOfNullTerminator = value.IndexOf('\0');

        if (indexOfNullTerminator >= 0)
            value = value.Slice(0, indexOfNullTerminator);

        var exactBytesCount = FrameworkExtensions.GetUtf8BytesCount(value) + 1;
        var pointer = FrameworkExtensions.NativeAlloc<byte>((nuint)exactBytesCount);
        var length = exactBytesCount - 1;

        var result = new NativeUtf8CString(pointer, length);

        FrameworkExtensions.GetUtf8Bytes(value, result.Span);
        pointer[length] = 0;

        return result;
    }

    /// <summary>
    /// Creates a new <see cref="NativeUtf8CString"/> by copying a UTF-8 byte span into native memory.
    /// If the span contains embedded null characters, it is truncated at the first null character.
    /// </summary>
    /// <param name="value">The UTF-8 byte span to copy.</param>
    /// <returns>A new <see cref="NativeUtf8CString"/> containing the null-terminated UTF-8 bytes.</returns>
    public static NativeUtf8CString FromUtf8Span(ReadOnlySpan<byte> value)
    {
        var indexOfNullTerminator = value.IndexOf((byte)0);

        if (indexOfNullTerminator >= 0)
            value = value.Slice(0, indexOfNullTerminator);

        var exactBytesCount = value.Length + 1;
        var pointer = FrameworkExtensions.NativeAlloc<byte>((nuint)exactBytesCount);
        var length = value.Length;

        var result = new NativeUtf8CString(pointer, length);

        value.CopyTo(result.Span);
        pointer[length] = 0;

        return result;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private NativeUtf8CString(byte* pointer, int length)
    {
        _pointer = pointer;
        _length = length;
    }

    private Span<byte> Span
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => new(_pointer, _length);
    }

    /// <summary>
    /// Gets a read-only reference to the UTF-8 byte at the specified index.
    /// </summary>
    /// <param name="index">The zero-based index of the byte to retrieve.</param>
    /// <exception cref="IndexOutOfRangeException"><paramref name="index"/> is negative or greater than or equal to <see cref="Length"/>.</exception>
    public ref readonly byte this[int index]
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => ref Span[index];
    }

    /// <summary>
    /// Gets the number of UTF-8 bytes in this string, excluding the null terminator.
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
    /// Returns a <see cref="ReadOnlyUtf8StringView"/> representing the content of this string (excluding the null terminator).
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ReadOnlyUtf8StringView AsReadOnlyView() => ReadOnlyUtf8StringView.FromPointerLength(_pointer, _length);

    /// <inheritdoc/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override bool Equals(object? obj) => obj is NativeUtf8CString other && Equals(other);

    /// <inheritdoc/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override int GetHashCode() => FrameworkExtensions.GetHashCode(Span);

    /// <summary>
    /// Determines whether this instance and another <see cref="NativeUtf8CString"/> have the same byte content.
    /// </summary>
    /// <param name="other">The string to compare with this instance.</param>
    /// <returns><see langword="true"/> if both strings have equal byte content; otherwise, <see langword="false"/>.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool Equals(NativeUtf8CString? other)
    {
        if (ReferenceEquals(this, other)) return true;
        if (other is null) return false;
        if (_pointer == other._pointer) return true;
        if (_pointer is null || other._pointer is null) return false;
        return AsReadOnlyView().Equals(other.AsReadOnlyView());
    }

    /// <summary>
    /// Decodes this UTF-8 string into a managed <see cref="string"/>.
    /// </summary>
    /// <returns>A managed string representing the decoded content.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override string ToString() => FrameworkExtensions.ToString(Span);

    /// <summary>
    /// Determines whether two <see cref="NativeUtf8CString"/> instances have the same byte content.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool operator ==(NativeUtf8CString? left, NativeUtf8CString? right)
    {
        if (ReferenceEquals(left, right)) return true;
        if (left is null) return false;
        return left.Equals(right);
    }

    /// <summary>
    /// Determines whether two <see cref="NativeUtf8CString"/> instances have different byte content.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool operator !=(NativeUtf8CString? left, NativeUtf8CString? right) => !(left == right);

    /// <summary>
    /// Implicitly converts a <see cref="NativeUtf8CString"/> to an unmanaged <see cref="ReadOnlyUtf8CString"/> pointer wrapper.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static implicit operator ReadOnlyUtf8CString(NativeUtf8CString? str) => str is null ? ReadOnlyUtf8CString.Null : ReadOnlyUtf8CString.FromPointer(str._pointer);

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
    /// Finalizes an instance of the <see cref="NativeUtf8CString"/> class.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    ~NativeUtf8CString() => ReleaseUnmanagedResources();
}