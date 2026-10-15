using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using SharpFFI.Primitives.Extensions;

#pragma warning disable CS0809

namespace SharpFFI.Primitives.Utf8;

[DebuggerDisplay("{ToString(),raw}")]
[StructLayout(LayoutKind.Sequential)]
public readonly unsafe ref struct Utf8StringView
{
    public static Utf8StringView Empty => default;

    public readonly Span<byte> Span;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Utf8StringView FromUtf8Span(Span<byte> value) => new(value);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Utf8StringView FromPointerLength(byte* pointer, int length) => new(pointer, length);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Utf8StringView FromPointerLength(nint pointer, int length) => new((byte*)pointer, length);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Utf8StringView FromPointer(byte* pointer) => new(FrameworkExtensions.CreateSpanFromNullTerminated(pointer));

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Utf8StringView FromPointer(nint pointer) => new(FrameworkExtensions.CreateSpanFromNullTerminated((byte*)pointer));

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Utf8StringView FromUtf8CString(Utf8CString value) => new(FrameworkExtensions.CreateSpanFromNullTerminated(value.Pointer));

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private Utf8StringView(Span<byte> value) => Span = value;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private Utf8StringView(byte* pointer, int length) => Span = new Span<byte>(pointer, length);

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
    public Utf8StringView Slice(int start) => new(Span.Slice(start));

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
    public override string ToString() => FrameworkExtensions.ToString(Span);

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
    public static implicit operator ReadOnlyUtf8StringView(Utf8StringView view) => ReadOnlyUtf8StringView.FromUtf8Span(view.Span);
}