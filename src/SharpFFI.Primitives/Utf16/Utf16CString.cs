using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using SharpFFI.Primitives.Extensions;

namespace SharpFFI.Primitives.Utf16;

/// <summary>
/// Represents a non-owning pointer wrapper for a null-terminated UTF-16 wide string (<c>wchar_t*</c> / <c>char16_t*</c>).
/// </summary>
[DebuggerDisplay("{ToString(),raw}")]
[StructLayout(LayoutKind.Sequential)]
public readonly unsafe struct Utf16CString : IEquatable<Utf16CString>
{
    /// <summary>
    /// Gets a <see cref="Utf16CString"/> whose pointer is <see langword="null"/>.
    /// </summary>
    public static Utf16CString Null => default;

    /// <summary>
    /// The raw pointer to the null-terminated UTF-16 character buffer.
    /// </summary>
    public readonly char* Pointer;

    /// <summary>
    /// Creates a <see cref="Utf16CString"/> from a raw character pointer.
    /// </summary>
    /// <param name="pointer">The pointer to the null-terminated UTF-16 string.</param>
    /// <returns>A new <see cref="Utf16CString"/> instance.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Utf16CString FromPointer(char* pointer) => new(pointer);

    /// <summary>
    /// Creates a <see cref="Utf16CString"/> from an unmanaged integer pointer.
    /// </summary>
    /// <param name="pointer">The pointer to the null-terminated UTF-16 string.</param>
    /// <returns>A new <see cref="Utf16CString"/> instance.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Utf16CString FromPointer(nint pointer) => new((char*)pointer);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private Utf16CString(char* pointer) => Pointer = pointer;

    /// <summary>
    /// Gets a value indicating whether the pointer is <see langword="null"/>.
    /// </summary>
    public bool IsNull
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => Pointer is null;
    }

    /// <summary>
    /// Gets a value indicating whether the pointer is <see langword="null"/> or points to a null terminator (empty string).
    /// </summary>
    public bool IsEmpty
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => Pointer is null || *Pointer == '\0';
    }

    /// <summary>
    /// Gets a mutable reference to the character at the specified index.
    /// </summary>
    /// <param name="index">The zero-based offset from the starting pointer.</param>
    public ref char this[int index]
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => ref Pointer[index];
    }

    /// <summary>
    /// Creates a mutable <see cref="Utf16StringView"/> over the string up to the null terminator.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Utf16StringView AsView() => Utf16StringView.FromPointer(Pointer);

    /// <summary>
    /// Creates a <see cref="ReadOnlyUtf16StringView"/> over the string up to the null terminator.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ReadOnlyUtf16StringView AsReadOnlyView() => ReadOnlyUtf16StringView.FromPointer(Pointer);

    /// <summary>
    /// Determines whether this instance and another <see cref="Utf16CString"/> have the same character content.
    /// </summary>
    /// <param name="other">The string to compare with this instance.</param>
    /// <returns><see langword="true"/> if both strings have equal character content; otherwise, <see langword="false"/>.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool Equals(Utf16CString other)
    {
        if (Pointer == other.Pointer) return true;
        if (Pointer is null || other.Pointer is null) return false;
        return AsReadOnlyView().Equals(other.AsReadOnlyView());
    }

    /// <inheritdoc/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override bool Equals(object? obj) => obj is Utf16CString other && Equals(other);

    /// <inheritdoc/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override int GetHashCode() => FrameworkExtensions.GetHashCode(AsReadOnlyView());

    /// <summary>
    /// Converts the null-terminated UTF-16 string into a managed <see cref="string"/>.
    /// </summary>
    /// <returns>A managed string representing the content, or an empty string if <see cref="Pointer"/> is <see langword="null"/>.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override string ToString() => FrameworkExtensions.ToString(AsReadOnlyView());

    /// <summary>
    /// Determines whether two <see cref="Utf16CString"/> instances have the same character content.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool operator ==(Utf16CString left, Utf16CString right) => left.Equals(right);

    /// <summary>
    /// Determines whether two <see cref="Utf16CString"/> instances have different character content.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool operator !=(Utf16CString left, Utf16CString right) => !left.Equals(right);

    /// <summary>
    /// Implicitly converts a mutable <see cref="Utf16CString"/> to a <see cref="ReadOnlyUtf16CString"/>.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static implicit operator ReadOnlyUtf16CString(Utf16CString str) => ReadOnlyUtf16CString.FromPointer(str.Pointer);
}