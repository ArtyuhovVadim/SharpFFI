using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using SharpFFI.Primitives.Extensions;

#pragma warning disable CS0809

namespace SharpFFI.Primitives.Utf8;

[DebuggerDisplay("{ToString(),raw}")]
[StructLayout(LayoutKind.Sequential)]
[method: MethodImpl(MethodImplOptions.AggressiveInlining)]
public readonly unsafe ref struct Utf8StringView(Span<byte> span)
{
    public static Utf8StringView Null => default;

    public readonly Span<byte> Span = span;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Utf8StringView(byte* pointer, int length) : this(pointer is null ? Span<byte>.Empty : new Span<byte>(pointer, length)) { }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Utf8StringView(byte* pointer) : this(pointer, pointer is null ? 0 : SpanHelpers.IndexOfNullByte(pointer)) { }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Utf8StringView(nint pointer, int length) : this((byte*)pointer, length) { }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Utf8StringView(nint pointer) : this((byte*)pointer) { }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Utf8StringView(Utf8CString str) : this(str.Pointer) { }

    public ref byte this[int index]
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
    public void Fill(byte value) => Span.Fill(value);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void CopyTo(Utf8StringView destination) => Span.CopyTo(destination.Span);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool TryCopyTo(Utf8StringView destination) => Span.TryCopyTo(destination.Span);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Utf8StringView Slice(int start) => new(Span[start..]);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Utf8StringView Slice(int start, int length) => new(Span.Slice(start, length));

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public byte[] ToArray() => Span.ToArray();

    [EditorBrowsable(EditorBrowsableState.Never)]
    [Obsolete("Equals(object) on ref struct is not supported.", true)]
    public override bool Equals(object? obj) => throw new NotSupportedException();

    [EditorBrowsable(EditorBrowsableState.Never)]
    [Obsolete("GetHashCode() on ref struct is not supported.", true)]
    public override int GetHashCode() => throw new NotSupportedException();

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool Equals(Utf8StringView other) => Span == other.Span || Span.SequenceEqual(other.Span);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Utf8CharEnumerator GetEnumerator() => new(Span);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override string ToString() => Encoding.UTF8.GetString(Span);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool operator ==(Utf8StringView left, Utf8StringView right) => left.Equals(right);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool operator !=(Utf8StringView left, Utf8StringView right) => !left.Equals(right);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static implicit operator Span<byte>(Utf8StringView view) => view.Span;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static implicit operator ReadOnlySpan<byte>(Utf8StringView view) => view.Span;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static implicit operator Utf8StringView(Span<byte> span) => new(span);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static implicit operator ReadOnlyUtf8StringView(Utf8StringView view) => new(view.Span);
}