using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using SharpFFI.Primitives.Extensions;

namespace SharpFFI.Primitives.Utf16;

[DebuggerDisplay("{ToString(),raw}")]
[StructLayout(LayoutKind.Sequential)]
public readonly unsafe struct ReadOnlyUtf16CString : IEquatable<ReadOnlyUtf16CString>
{
    public static ReadOnlyUtf16CString Null => default;

    public readonly char* Pointer;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ReadOnlyUtf16CString FromPointer(char* pointer) => new(pointer);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ReadOnlyUtf16CString FromPointer(nint pointer) => new((char*)pointer);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private ReadOnlyUtf16CString(char* pointer) => Pointer = pointer;

    public bool IsNull
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => Pointer is null;
    }

    public bool IsEmpty
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => Pointer is null || *Pointer == '\0';
    }

    public ref readonly char this[int index]
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => ref Pointer[index];
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ReadOnlyUtf16StringView AsReadOnlyView() => ReadOnlyUtf16StringView.FromPointer(Pointer);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool Equals(ReadOnlyUtf16CString other)
    {
        if (Pointer == other.Pointer) return true;
        if (Pointer is null || other.Pointer is null) return false;
        return AsReadOnlyView().Equals(other.AsReadOnlyView());
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override bool Equals(object? obj) => obj is ReadOnlyUtf16CString other && Equals(other);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override int GetHashCode() => FrameworkExtensions.GetHashCode(AsReadOnlyView());

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override string ToString() => FrameworkExtensions.ToString(AsReadOnlyView());

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool operator ==(ReadOnlyUtf16CString left, ReadOnlyUtf16CString right) => left.Equals(right);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool operator !=(ReadOnlyUtf16CString left, ReadOnlyUtf16CString right) => !left.Equals(right);
}