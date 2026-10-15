using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using SharpFFI.Primitives.Extensions;

#pragma warning disable CS0809

namespace SharpFFI.Primitives.Utf16;

[DebuggerDisplay("{ToString(),raw}")]
[StructLayout(LayoutKind.Sequential)]
public readonly unsafe ref struct ReadOnlyUtf16StringView
{
    public static ReadOnlyUtf16StringView Empty => default;

    public readonly ReadOnlySpan<char> Span;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ReadOnlyUtf16StringView FromUtf16Span(ReadOnlySpan<char> value) => new(value);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ReadOnlyUtf16StringView FromPointerLength(char* pointer, int length) => new(pointer, length);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ReadOnlyUtf16StringView FromPointerLength(nint pointer, int length) => new((char*)pointer, length);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ReadOnlyUtf16StringView FromPointer(char* pointer) => new(FrameworkExtensions.CreateSpanFromNullTerminated(pointer));

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ReadOnlyUtf16StringView FromPointer(nint pointer) => new(FrameworkExtensions.CreateSpanFromNullTerminated((char*)pointer));

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ReadOnlyUtf16StringView FromUtf16CString(Utf16CString value) => new(FrameworkExtensions.CreateSpanFromNullTerminated(value.Pointer));

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ReadOnlyUtf16StringView FromUtf16CString(ReadOnlyUtf16CString value) => new(FrameworkExtensions.CreateSpanFromNullTerminated(value.Pointer));

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private ReadOnlyUtf16StringView(ReadOnlySpan<char> value) => Span = value;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private ReadOnlyUtf16StringView(char* pointer, int length) => Span = new ReadOnlySpan<char>(pointer, length);

    public ref readonly char this[int index]
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => ref Span[index];
    }

    public int Length
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => Span.Length;
    }

    public bool IsEmpty
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => Span.IsEmpty;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void CopyTo(Utf16StringView destination) => Span.CopyTo(destination.Span);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool TryCopyTo(Utf16StringView destination) => Span.TryCopyTo(destination.Span);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ReadOnlyUtf16StringView Slice(int start) => new(Span.Slice(start));

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ReadOnlyUtf16StringView Slice(int start, int length) => new(Span.Slice(start, length));

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public char[] ToArray() => Span.ToArray();

    [EditorBrowsable(EditorBrowsableState.Never)]
    [Obsolete("Equals(object) on ref struct is not supported.", true)]
    public override bool Equals(object? obj) => throw new NotSupportedException();

    [EditorBrowsable(EditorBrowsableState.Never)]
    [Obsolete("GetHashCode() on ref struct is not supported.", true)]
    public override int GetHashCode() => throw new NotSupportedException();

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool Equals(ReadOnlyUtf16StringView other) => Span == other.Span || Span.SequenceEqual(other.Span);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ReadOnlySpan<char>.Enumerator GetEnumerator() => Span.GetEnumerator();

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override string ToString() => FrameworkExtensions.ToString(Span);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool operator ==(ReadOnlyUtf16StringView left, ReadOnlyUtf16StringView right) => left.Equals(right);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool operator !=(ReadOnlyUtf16StringView left, ReadOnlyUtf16StringView right) => !left.Equals(right);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static implicit operator ReadOnlySpan<char>(ReadOnlyUtf16StringView view) => view.Span;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static implicit operator ReadOnlyUtf16StringView(ReadOnlySpan<char> span) => new(span);
}