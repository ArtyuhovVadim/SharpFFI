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
public readonly unsafe ref struct Utf16StringView(Span<char> span)
{
    public static Utf16StringView Null => default;

    public readonly Span<char> Span = span;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Utf16StringView(char* pointer, int length) : this(pointer is null ? Span<char>.Empty : new Span<char>(pointer, length)) { }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Utf16StringView(char* pointer) : this(pointer, pointer is null ? 0 : SpanHelpers.IndexOfNullCharacter(pointer)) { }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Utf16StringView(nint pointer, int length) : this((char*)pointer, length) { }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Utf16StringView(nint pointer) : this((char*)pointer) { }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Utf16StringView(Utf16CString str) : this(str.Pointer) { }

    public ref char this[int index]
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
    public void Clear() => Span.Clear();

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Fill(char value) => Span.Fill(value);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void CopyTo(Utf16StringView destination) => Span.CopyTo(destination.Span);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool TryCopyTo(Utf16StringView destination) => Span.TryCopyTo(destination.Span);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Utf16StringView Slice(int start) => new(Span[start..]);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Utf16StringView Slice(int start, int length) => new(Span.Slice(start, length));

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public char[] ToArray() => Span.ToArray();

    [EditorBrowsable(EditorBrowsableState.Never)]
    [Obsolete("Equals(object) on ref struct is not supported.", true)]
    public override bool Equals(object? obj) => throw new NotSupportedException();

    [EditorBrowsable(EditorBrowsableState.Never)]
    [Obsolete("GetHashCode() on ref struct is not supported.", true)]
    public override int GetHashCode() => throw new NotSupportedException();

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool Equals(Utf16StringView other) => Span == other.Span || Span.SequenceEqual(other.Span);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Utf16CharEnumerator GetEnumerator() => new(Span);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override string ToString() => Span.ToString();

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool operator ==(Utf16StringView left, Utf16StringView right) => left.Equals(right);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool operator !=(Utf16StringView left, Utf16StringView right) => !left.Equals(right);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static implicit operator Span<char>(Utf16StringView view) => view.Span;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static implicit operator ReadOnlySpan<char>(Utf16StringView view) => view.Span;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static implicit operator Utf16StringView(Span<char> span) => new(span);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static implicit operator ReadOnlyUtf16StringView(Utf16StringView view) => new(view.Span);
}