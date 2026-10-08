using System;
using System.Buffers;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;

#pragma warning disable CS0809

namespace SharpFFI.Primitives.Utf8;

public static unsafe class SpanExtensions
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T* AsPointer<T>(this ReadOnlySpan<T> span) where T : unmanaged => (T*)Unsafe.AsPointer(ref MemoryMarshal.GetReference(span));

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T* AsPointer<T>(this Span<T> span) where T : unmanaged => (T*)Unsafe.AsPointer(ref MemoryMarshal.GetReference(span));
}

public static unsafe class EncodingExtensions
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static string GetString(this Encoding encoding, ReadOnlySpan<byte> bytes) => encoding.GetString(bytes.AsPointer(), bytes.Length);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int GetBytes(this Encoding encoding, ReadOnlySpan<char> chars, Span<byte> bytes) => encoding.GetBytes(chars.AsPointer(), chars.Length, bytes.AsPointer(), bytes.Length);
}

[DebuggerDisplay("{ToString(),raw}")]
[StructLayout(LayoutKind.Sequential)]
public readonly unsafe ref struct Utf8ScopedCString
{
    private readonly Span<byte> _span;
    private readonly byte[]? _rentedBuffer;
    private readonly GCHandle _gcBufferHandle;

    public Utf8ScopedCString(string? value, Span<byte> buffer = default)
    {
        if (value is null)
        {
            _span = default;
            _rentedBuffer = null;
            _gcBufferHandle = default;
            return;
        }

        var valueBytesCount = Encoding.UTF8.GetByteCount(value) + 1;

        if (buffer.Length >= valueBytesCount)
        {
            _rentedBuffer = null;
            _span = buffer[..valueBytesCount];
        }
        else
        {
            _rentedBuffer = ArrayPool<byte>.Shared.Rent(valueBytesCount);
            _gcBufferHandle = GCHandle.Alloc(_rentedBuffer, GCHandleType.Pinned);
            _span = _rentedBuffer.AsSpan(0, valueBytesCount);
        }

        Encoding.UTF8.GetBytes(value, _span);
        _span[valueBytesCount - 1] = (byte)'\0';
    }

    public Utf8ScopedCString(ReadOnlySpan<byte> value, Span<byte> buffer = default)
    {
        var valueBytesCount = value.Length + 1;

        if (buffer.Length >= valueBytesCount)
        {
            _rentedBuffer = null;
            _span = buffer[..valueBytesCount];
        }
        else
        {
            _rentedBuffer = ArrayPool<byte>.Shared.Rent(valueBytesCount);
            _gcBufferHandle = GCHandle.Alloc(_rentedBuffer, GCHandleType.Pinned);
            _span = _rentedBuffer.AsSpan(0, valueBytesCount);
        }

        value.CopyTo(_span);
        _span[valueBytesCount - 1] = (byte)'\0';
    }

    public ref readonly byte this[int index]
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
    public ReadOnlyUtf8StringView AsReadOnlyView() => new(_span.IsEmpty ? default : _span[..^1]);

    [EditorBrowsable(EditorBrowsableState.Never)]
    [Obsolete("Equals(object) on ref struct is not supported.", true)]
    public override bool Equals(object? obj) => throw new NotSupportedException();

    [EditorBrowsable(EditorBrowsableState.Never)]
    [Obsolete("GetHashCode() on ref struct is not supported.", true)]
    public override int GetHashCode() => throw new NotSupportedException();

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool Equals(Utf8ScopedCString other) => AsReadOnlyView().Equals(other.AsReadOnlyView());

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override string ToString() => Encoding.UTF8.GetString(AsReadOnlyView().Span);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool operator ==(Utf8ScopedCString left, Utf8ScopedCString right) => left.Equals(right);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool operator !=(Utf8ScopedCString left, Utf8ScopedCString right) => !left.Equals(right);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static implicit operator ReadOnlyUtf8CString(Utf8ScopedCString str) => new(str._span.IsEmpty ? null : str._span.AsPointer());

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public readonly void Dispose()
    {
        if (_rentedBuffer is null)
            return;

        ref var handle = ref Unsafe.AsRef(in _gcBufferHandle);

        if (handle.IsAllocated)
            handle.Free();

        ArrayPool<byte>.Shared.Return(_rentedBuffer);
    }
}