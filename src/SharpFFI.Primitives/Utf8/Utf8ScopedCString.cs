using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using SharpFFI.Primitives.Extensions;

#pragma warning disable CS0809

namespace SharpFFI.Primitives.Utf8;

[DebuggerDisplay("{ToString(),raw}")]
[StructLayout(LayoutKind.Sequential)]
public unsafe ref struct Utf8ScopedCString
{
    private const long MaxUtf8BytesPerChar = 3;

    private byte* _pointer;
    private int _length;
    private bool _heapAllocated;

    public static Utf8ScopedCString FromString(string value, Span<byte> pinnedBuffer = default) => value is null ? throw new ArgumentNullException(nameof(value)) : FromUtf16Span(value, pinnedBuffer);

    public static Utf8ScopedCString FromUtf16Span(ReadOnlySpan<char> value, Span<byte> pinnedBuffer = default)
    {
        var indexOfNullTerminator = value.IndexOf('\0');

        if (indexOfNullTerminator >= 0)
            value = value.Slice(0, indexOfNullTerminator);

        if (MaxUtf8BytesPerChar * value.Length < pinnedBuffer.Length)
        {
            var pointer = pinnedBuffer.AsPointer();
            var length = FrameworkExtensions.GetUtf8Bytes(value, pinnedBuffer);
            pointer[length] = 0;

            return new Utf8ScopedCString(pointer, length, heapAllocated: false);
        }
        else
        {
            var exactBytesCount = FrameworkExtensions.GetUtf8BytesCount(value) + 1;
            var heapAllocated = exactBytesCount > pinnedBuffer.Length;
            var pointer = heapAllocated ? FrameworkExtensions.NativeAlloc<byte>((nuint)exactBytesCount) : pinnedBuffer.AsPointer();
            var length = exactBytesCount - 1;

            var result = new Utf8ScopedCString(pointer, length, heapAllocated);

            FrameworkExtensions.GetUtf8Bytes(value, result.Span);
            pointer[length] = 0;

            return result;
        }
    }

    public static Utf8ScopedCString FromUtf8Span(ReadOnlySpan<byte> value, Span<byte> pinnedBuffer = default)
    {
        var indexOfNullTerminator = value.IndexOf((byte)0);

        if (indexOfNullTerminator >= 0)
            value = value.Slice(0, indexOfNullTerminator);

        var exactBytesCount = value.Length + 1;
        var heapAllocated = exactBytesCount > pinnedBuffer.Length;

        var pointer = heapAllocated ? FrameworkExtensions.NativeAlloc<byte>((nuint)exactBytesCount) : pinnedBuffer.AsPointer();
        var length = value.Length;

        var result = new Utf8ScopedCString(pointer, length, heapAllocated);

        value.CopyTo(result.Span);
        pointer[length] = 0;

        return result;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private Utf8ScopedCString(byte* pointer, int length, bool heapAllocated)
    {
        _pointer = pointer;
        _length = length;
        _heapAllocated = heapAllocated;
    }

    private readonly Span<byte> Span
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => new(_pointer, _length);
    }

    public readonly ref readonly byte this[int index]
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => ref Span[index];
    }

    public readonly int Length
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => _length;
    }

    public readonly bool IsEmpty
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => Length == 0;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public readonly ReadOnlyUtf8StringView AsReadOnlyView() => ReadOnlyUtf8StringView.FromPointerLength(_pointer, _length);

    [EditorBrowsable(EditorBrowsableState.Never)]
    [Obsolete("Equals(object) on ref struct is not supported.", true)]
    public readonly override bool Equals(object? obj) => throw new NotSupportedException();

    [EditorBrowsable(EditorBrowsableState.Never)]
    [Obsolete("GetHashCode() on ref struct is not supported.", true)]
    public readonly override int GetHashCode() => throw new NotSupportedException();

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public readonly bool Equals(Utf8ScopedCString other) => AsReadOnlyView().Equals(other.AsReadOnlyView());

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public readonly override string ToString() => FrameworkExtensions.ToString(Span);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool operator ==(Utf8ScopedCString left, Utf8ScopedCString right) => left.Equals(right);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool operator !=(Utf8ScopedCString left, Utf8ScopedCString right) => !left.Equals(right);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static implicit operator ReadOnlyUtf8CString(Utf8ScopedCString str) => ReadOnlyUtf8CString.FromPointer(str._pointer);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Dispose()
    {
        if (_heapAllocated)
        {
            FrameworkExtensions.NativeFree(_pointer);
            _heapAllocated = false;
        }

        _pointer = null;
        _length = 0;
    }
}