using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using SharpFFI.Primitives.Extensions;

namespace SharpFFI.Primitives.Utf8;

[DebuggerDisplay("{ToString(),raw}")]
[StructLayout(LayoutKind.Sequential)]
public readonly unsafe struct Utf8CString : IEquatable<Utf8CString>
{
    public static Utf8CString Null => default;

    public readonly byte* Pointer;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Utf8CString FromPointer(byte* pointer) => new(pointer);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Utf8CString FromPointer(nint pointer) => new((byte*)pointer);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private Utf8CString(byte* pointer) => Pointer = pointer;

    public bool IsNull
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => Pointer is null;
    }

    public bool IsEmpty
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => Pointer is null || *Pointer == 0;
    }

    public ref byte this[int index]
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => ref Pointer[index];
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Utf8StringView AsView() => Utf8StringView.FromPointer(Pointer);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ReadOnlyUtf8StringView AsReadOnlyView() => ReadOnlyUtf8StringView.FromPointer(Pointer);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool Equals(Utf8CString other)
    {
        if (Pointer == other.Pointer) return true;
        if (Pointer is null || other.Pointer is null) return false;
        return AsReadOnlyView().Equals(other.AsReadOnlyView());
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override bool Equals(object? obj) => obj is Utf8CString other && Equals(other);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override int GetHashCode() => FrameworkExtensions.GetHashCode(AsReadOnlyView());

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override string ToString() => FrameworkExtensions.ToString(AsReadOnlyView());

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool operator ==(Utf8CString left, Utf8CString right) => left.Equals(right);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool operator !=(Utf8CString left, Utf8CString right) => !left.Equals(right);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static implicit operator ReadOnlyUtf8CString(Utf8CString str) => ReadOnlyUtf8CString.FromPointer(str.Pointer);
}