using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;

namespace SharpFFI.Primitives.Utf8;

/// <summary>
/// Represents a non-owning pointer wrapper for a null-terminated UTF-8 string (<c>char*</c>).
/// </summary>
/// <remarks>
/// This struct does not own or manage the lifetime of the underlying memory.
/// When pointing to memory on the managed GC heap, the target object must remain pinned for the entire lifetime of this instance.
/// If the target memory is relocated by the garbage collector or deallocated, accessing this instance causes undefined behavior or memory corruption.
/// </remarks>
[DebuggerDisplay("{ToString(),raw}")]
[StructLayout(LayoutKind.Sequential)]
public readonly unsafe struct Utf8CString : IEquatable<Utf8CString>
{
    /// <summary>
    /// Gets a <see cref="Utf8CString"/> whose pointer is <see langword="null"/>.
    /// </summary>
    public static Utf8CString Null => default;

    /// <summary>
    /// The raw pointer to the null-terminated UTF-8 byte buffer.
    /// </summary>
    public readonly byte* Pointer;

    /// <summary>
    /// Creates a <see cref="Utf8CString"/> from a raw byte pointer.
    /// </summary>
    /// <param name="pointer">The pointer to the null-terminated UTF-8 string.</param>
    /// <returns>A new <see cref="Utf8CString"/> instance.</returns>
    /// <remarks>
    /// The caller is responsible for ensuring that the memory at <paramref name="pointer"/> remains valid, accessible, and pinned (if allocated on the managed GC heap) for the entire lifetime of this instance.
    /// </remarks>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Utf8CString FromPointer(byte* pointer) => new(pointer);

    /// <summary>
    /// Creates a <see cref="Utf8CString"/> from an unmanaged integer pointer.
    /// </summary>
    /// <param name="pointer">The pointer to the null-terminated UTF-8 string.</param>
    /// <returns>A new <see cref="Utf8CString"/> instance.</returns>
    /// <remarks>
    /// The caller is responsible for ensuring that the memory at <paramref name="pointer"/> remains valid, accessible, and pinned (if allocated on the managed GC heap) for the entire lifetime of this instance.
    /// </remarks>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Utf8CString FromPointer(nint pointer) => new((byte*)pointer);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private Utf8CString(byte* pointer) => Pointer = pointer;

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
        get => Pointer is null || *Pointer == 0;
    }

    /// <summary>
    /// Gets a mutable reference to the byte at the specified index.
    /// </summary>
    /// <param name="index">The zero-based offset from the starting pointer.</param>
    public ref byte this[int index]
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => ref Pointer[index];
    }

    /// <summary>
    /// Creates a mutable <see cref="Utf8StringView"/> over the string up to the null terminator.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Utf8StringView AsView() => Utf8StringView.FromPointer(Pointer);

    /// <summary>
    /// Creates a <see cref="ReadOnlyUtf8StringView"/> over the string up to the null terminator.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ReadOnlyUtf8StringView AsReadOnlyView() => ReadOnlyUtf8StringView.FromPointer(Pointer);

    /// <summary>
    /// Determines whether this instance and another <see cref="Utf8CString"/> have the same byte content.
    /// </summary>
    /// <param name="other">The string to compare with this instance.</param>
    /// <returns><see langword="true"/> if both strings have equal byte content; otherwise, <see langword="false"/>.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool Equals(Utf8CString other)
    {
        if (Pointer == other.Pointer) return true;
        if (Pointer is null || other.Pointer is null) return false;
        return AsReadOnlyView().Equals(other.AsReadOnlyView());
    }

    /// <inheritdoc/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override bool Equals(object? obj) => obj is Utf8CString other && Equals(other);

    /// <inheritdoc/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override int GetHashCode() => Utf8StringView.GetHashCode(Pointer);

    /// <summary>
    /// Decodes the null-terminated UTF-8 string into a managed <see cref="string"/>.
    /// </summary>
    /// <returns>A managed string representing the decoded content, or an empty string if <see cref="Pointer"/> is <see langword="null"/>.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override string ToString() => IsEmpty ? string.Empty : Encoding.UTF8.GetString(AsReadOnlyView());

    /// <summary>
    /// Determines whether two <see cref="Utf8CString"/> instances have the same byte content.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool operator ==(Utf8CString left, Utf8CString right) => left.Equals(right);

    /// <summary>
    /// Determines whether two <see cref="Utf8CString"/> instances have different byte content.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool operator !=(Utf8CString left, Utf8CString right) => !left.Equals(right);

    /// <summary>
    /// Implicitly converts a mutable <see cref="Utf8CString"/> to a <see cref="ReadOnlyUtf8CString"/>.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static implicit operator ReadOnlyUtf8CString(Utf8CString str) => ReadOnlyUtf8CString.FromPointer(str.Pointer);
}
