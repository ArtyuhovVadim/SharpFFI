using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

#pragma warning disable CS0809

namespace SharpFFI.Primitives.Utf16;

[DebuggerDisplay("{ToString(),raw}")]
[StructLayout(LayoutKind.Sequential)]
[method: MethodImpl(MethodImplOptions.AggressiveInlining)]
public readonly unsafe ref struct ReadOnlyUtf16StringView(ReadOnlySpan<char> span)
{
    public static Utf16StringView Null => default;

    public readonly ReadOnlySpan<char> Span = span;

    public ReadOnlyUtf16StringView(char* pointer, int length) : this(pointer is null ? ReadOnlySpan<char>.Empty : new ReadOnlySpan<char>(pointer, length)) { }

    public ReadOnlyUtf16StringView(char* pointer) : this(pointer, pointer is null ? 0 : SpanHelpers.IndexOfNullCharacter(pointer)) { }

    public ReadOnlyUtf16StringView(nint pointer, int length) : this((char*)pointer, length) { }

    public ReadOnlyUtf16StringView(nint pointer) : this((char*)pointer) { }

    public ReadOnlyUtf16StringView(Utf16CString str) : this(str.Pointer) { }

    public ReadOnlyUtf16StringView(ReadOnlyUtf16CString str) : this(str.Pointer) { }

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
    public ReadOnlyUtf16StringView Slice(int start) => new(Span[start..]);

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
    public Utf16CharEnumerator GetEnumerator() => new(Span);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override string ToString() => Span.ToString();

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool operator ==(ReadOnlyUtf16StringView left, ReadOnlyUtf16StringView right) => left.Equals(right);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool operator !=(ReadOnlyUtf16StringView left, ReadOnlyUtf16StringView right) => !left.Equals(right);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static implicit operator ReadOnlySpan<char>(ReadOnlyUtf16StringView view) => view.Span;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static implicit operator ReadOnlyUtf16StringView(ReadOnlySpan<char> span) => new(span);
}