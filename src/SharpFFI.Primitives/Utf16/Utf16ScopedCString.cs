using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using SharpFFI.Primitives.Extensions;

#pragma warning disable CS0809

namespace SharpFFI.Primitives.Utf16;

[DebuggerDisplay("{ToString(),raw}")]
[StructLayout(LayoutKind.Sequential)]
public unsafe ref struct Utf16ScopedCString
{
    private char* _pointer;
    private int _length;
    private bool _heapAllocated;

    public static Utf16ScopedCString FromString(string value, Span<char> pinnedBuffer = default) => value is null ? throw new ArgumentNullException(nameof(value)) : FromUtf16Span(value, pinnedBuffer);

    public static Utf16ScopedCString FromUtf16Span(ReadOnlySpan<char> value, Span<char> pinnedBuffer = default)
    {
        var indexOfNullTerminator = value.IndexOf('\0');

        if (indexOfNullTerminator >= 0)
            value = value.Slice(0, indexOfNullTerminator);

        var exactCharsCount = value.Length + 1;
        var heapAllocated = exactCharsCount > pinnedBuffer.Length;
        var pointer = heapAllocated ? FrameworkExtensions.NativeAlloc<char>((nuint)exactCharsCount * sizeof(char)) : pinnedBuffer.AsPointer();
        var length = value.Length;

        var result = new Utf16ScopedCString(pointer, length, heapAllocated);

        value.CopyTo(result.Span);
        pointer[length] = '\0';

        return result;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private Utf16ScopedCString(char* pointer, int length, bool heapAllocated)
    {
        _pointer = pointer;
        _length = length;
        _heapAllocated = heapAllocated;
    }

    private readonly Span<char> Span
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => new(_pointer, _length);
    }

    public readonly ref readonly char this[int index]
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
    public readonly ReadOnlyUtf16StringView AsReadOnlyView() => ReadOnlyUtf16StringView.FromPointerLength(_pointer, _length);

    [EditorBrowsable(EditorBrowsableState.Never)]
    [Obsolete("Equals(object) on ref struct is not supported.", true)]
    public readonly override bool Equals(object? obj) => throw new NotSupportedException();

    [EditorBrowsable(EditorBrowsableState.Never)]
    [Obsolete("GetHashCode() on ref struct is not supported.", true)]
    public readonly override int GetHashCode() => throw new NotSupportedException();

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public readonly bool Equals(Utf16ScopedCString other) => AsReadOnlyView().Equals(other.AsReadOnlyView());

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public readonly override string ToString() => FrameworkExtensions.ToString(Span);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool operator ==(Utf16ScopedCString left, Utf16ScopedCString right) => left.Equals(right);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool operator !=(Utf16ScopedCString left, Utf16ScopedCString right) => !left.Equals(right);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static implicit operator ReadOnlyUtf16CString(Utf16ScopedCString str) => ReadOnlyUtf16CString.FromPointer(str._pointer);

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