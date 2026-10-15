using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using SharpFFI.Primitives.Extensions;

#pragma warning disable CS0809

namespace SharpFFI.Primitives.Utf8;

[DebuggerDisplay("{ToString(),raw}")]
[StructLayout(LayoutKind.Sequential)]
public readonly unsafe ref struct ReadOnlyUtf8StringView
{
    public static ReadOnlyUtf8StringView Empty => default;

    public readonly ReadOnlySpan<byte> Span;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ReadOnlyUtf8StringView FromUtf8Span(ReadOnlySpan<byte> value) => new(value);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ReadOnlyUtf8StringView FromPointerLength(byte* pointer, int length) => new(pointer, length);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ReadOnlyUtf8StringView FromPointerLength(nint pointer, int length) => new((byte*)pointer, length);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ReadOnlyUtf8StringView FromPointer(byte* pointer) => new(FrameworkExtensions.CreateSpanFromNullTerminated(pointer));

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ReadOnlyUtf8StringView FromPointer(nint pointer) => new(FrameworkExtensions.CreateSpanFromNullTerminated((byte*)pointer));

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ReadOnlyUtf8StringView FromUtf8CString(Utf8CString value) => new(FrameworkExtensions.CreateSpanFromNullTerminated(value.Pointer));

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ReadOnlyUtf8StringView FromUtf8CString(ReadOnlyUtf8CString value) => new(FrameworkExtensions.CreateSpanFromNullTerminated(value.Pointer));

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private ReadOnlyUtf8StringView(ReadOnlySpan<byte> value) => Span = value;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private ReadOnlyUtf8StringView(byte* pointer, int length) => Span = new ReadOnlySpan<byte>(pointer, length);

    public ref readonly byte this[int index]
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
    public void CopyTo(Utf8StringView destination) => Span.CopyTo(destination.Span);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool TryCopyTo(Utf8StringView destination) => Span.TryCopyTo(destination.Span);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ReadOnlyUtf8StringView Slice(int start) => new(Span.Slice(start));

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ReadOnlyUtf8StringView Slice(int start, int length) => new(Span.Slice(start, length));

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public byte[] ToArray() => Span.ToArray();

    [EditorBrowsable(EditorBrowsableState.Never)]
    [Obsolete("Equals(object) on ref struct is not supported.", true)]
    public override bool Equals(object? obj) => throw new NotSupportedException();

    [EditorBrowsable(EditorBrowsableState.Never)]
    [Obsolete("GetHashCode() on ref struct is not supported.", true)]
    public override int GetHashCode() => throw new NotSupportedException();

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool Equals(ReadOnlyUtf8StringView other) => Span == other.Span || Span.SequenceEqual(other.Span);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Utf8CharEnumerator GetEnumerator() => new(Span);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override string ToString() => FrameworkExtensions.ToString(Span);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool operator ==(ReadOnlyUtf8StringView left, ReadOnlyUtf8StringView right) => left.Equals(right);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool operator !=(ReadOnlyUtf8StringView left, ReadOnlyUtf8StringView right) => !left.Equals(right);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static implicit operator ReadOnlySpan<byte>(ReadOnlyUtf8StringView view) => view.Span;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static implicit operator ReadOnlyUtf8StringView(ReadOnlySpan<byte> span) => new(span);
}