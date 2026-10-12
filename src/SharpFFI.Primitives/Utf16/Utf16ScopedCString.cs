using System.Buffers;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using SharpFFI.Primitives.Extensions;

#pragma warning disable CS0809

namespace SharpFFI.Primitives.Utf16;

[DebuggerDisplay("{ToString(),raw}")]
[StructLayout(LayoutKind.Sequential)]
public readonly unsafe ref struct Utf16ScopedCString
{
    private readonly Span<char> _span;
    private readonly char[]? _rentedBuffer;
    private readonly GCHandle _gcBufferHandle;

    public Utf16ScopedCString(string? value, Span<char> buffer = default)
    {
        if (value is null)
        {
            _span = default;
            _rentedBuffer = null;
            _gcBufferHandle = default;
            return;
        }

        var length = value.Length + 1;

        if (buffer.Length >= length)
        {
            _rentedBuffer = null;
            _span = buffer[..length];
        }
        else
        {
            _rentedBuffer = ArrayPool<char>.Shared.Rent(length);
            _gcBufferHandle = GCHandle.Alloc(_rentedBuffer, GCHandleType.Pinned);
            _span = _rentedBuffer.AsSpan(0, length);
        }

        value.AsSpan().CopyTo(_span);
        _span[length - 1] = '\0';
    }

    public Utf16ScopedCString(ReadOnlySpan<char> value, Span<char> buffer = default)
    {
        var length = value.Length + 1;

        if (buffer.Length >= length)
        {
            _rentedBuffer = null;
            _span = buffer[..length];
        }
        else
        {
            _rentedBuffer = ArrayPool<char>.Shared.Rent(length);
            _gcBufferHandle = GCHandle.Alloc(_rentedBuffer, GCHandleType.Pinned);
            _span = _rentedBuffer.AsSpan(0, length);
        }

        value.CopyTo(_span);
        _span[length - 1] = '\0';
    }

    public ref readonly char this[int index]
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => ref _span[index];
    }

    public int Length
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => _span.IsEmpty ? 0 : _span.Length - 1;
    }

    public bool IsEmpty
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => _span.IsEmpty;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ReadOnlyUtf16StringView AsReadOnlyView() => new(_span.IsEmpty ? default : _span[..^1]);

    [EditorBrowsable(EditorBrowsableState.Never)]
    [Obsolete("Equals(object) on ref struct is not supported.", true)]
    public override bool Equals(object? obj) => throw new NotSupportedException();

    [EditorBrowsable(EditorBrowsableState.Never)]
    [Obsolete("GetHashCode() on ref struct is not supported.", true)]
    public override int GetHashCode() => throw new NotSupportedException();

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool Equals(Utf16ScopedCString other) => AsReadOnlyView().Equals(other.AsReadOnlyView());

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override string ToString() => _span.ToString();

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool operator ==(Utf16ScopedCString left, Utf16ScopedCString right) => left.Equals(right);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool operator !=(Utf16ScopedCString left, Utf16ScopedCString right) => !left.Equals(right);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static implicit operator ReadOnlyUtf16CString(Utf16ScopedCString str) => new(str._span.IsEmpty ? null : str._span.AsPointer());

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Dispose()
    {
        if (_rentedBuffer is null)
            return;

        ref var handle = ref Unsafe.AsRef(in _gcBufferHandle);

        if (handle.IsAllocated)
            handle.Free();

        ArrayPool<char>.Shared.Return(_rentedBuffer);
    }
}