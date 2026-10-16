using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;

namespace SharpFFI.Primitives.Extensions;

internal unsafe static class FrameworkExtensions
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int GetHashCode(ReadOnlySpan<byte> span) => span.IsEmpty ? 0 : Marvin.ComputeHash32(span, Marvin.DefaultSeed);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int GetHashCode(ReadOnlySpan<char> span) => span.IsEmpty ? 0 : Marvin.ComputeHash32(MemoryMarshal.AsBytes(span), Marvin.DefaultSeed);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static string ToString(ReadOnlySpan<byte> span)
#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
        => span.IsEmpty ? string.Empty : Encoding.UTF8.GetString(span);
#elif NETSTANDARD2_0_OR_GREATER
        => span.IsEmpty ? string.Empty : Encoding.UTF8.GetString(span.AsPointer(), span.Length);
#else
#error Unsupported runtime.
#endif

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static string ToString(ReadOnlySpan<char> span) => span.ToString();

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T* NativeAlloc<T>(nuint bytesCount) where T : unmanaged
#if NET6_0_OR_GREATER
        => (T*)NativeMemory.Alloc(bytesCount);
#elif NET5_0_OR_GREATER || NETSTANDARD2_0_OR_GREATER
        => (T*)Marshal.AllocHGlobal((nint)bytesCount);
#else
#error Unsupported runtime.
#endif

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void NativeFree(void* pointer)
#if NET6_0_OR_GREATER
        => NativeMemory.Free(pointer);
#elif NET5_0_OR_GREATER || NETSTANDARD2_0_OR_GREATER
        => Marshal.FreeHGlobal((nint)pointer);
#else
#error Unsupported runtime.
#endif

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int GetUtf8BytesCount(ReadOnlySpan<char> chars)
#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
        => chars.IsEmpty ? 0 : Encoding.UTF8.GetByteCount(chars);
#elif NETSTANDARD2_0_OR_GREATER
        => chars.IsEmpty ? 0 : Encoding.UTF8.GetByteCount(chars.AsPointer(), chars.Length);
#else
#error Unsupported runtime.
#endif

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int GetUtf8Bytes(ReadOnlySpan<char> chars, Span<byte> bytes)
#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
        => chars.IsEmpty ? 0 : Encoding.UTF8.GetBytes(chars, bytes);
#elif NETSTANDARD2_0_OR_GREATER
        => chars.IsEmpty ? 0 : Encoding.UTF8.GetBytes(chars.AsPointer(), chars.Length, bytes.AsPointer(), bytes.Length);
#else
#error Unsupported runtime.
#endif

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Span<byte> CreateSpanFromNullTerminated(byte* pointer)
#if NET6_0_OR_GREATER
        => new(pointer, MemoryMarshal.CreateReadOnlySpanFromNullTerminated(pointer).Length);
#elif NET5_0_OR_GREATER || NETSTANDARD2_0_OR_GREATER
        => pointer is null ? default : new Span<byte>(pointer, new ReadOnlySpan<byte>(pointer, int.MaxValue).IndexOf((byte)0));
#else
#error Unsupported runtime.
#endif

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Span<char> CreateSpanFromNullTerminated(char* pointer)
#if NET6_0_OR_GREATER
        => new(pointer, MemoryMarshal.CreateReadOnlySpanFromNullTerminated(pointer).Length);
#elif NET5_0_OR_GREATER || NETSTANDARD2_0_OR_GREATER
        => pointer is null ? default : new Span<char>(pointer, new ReadOnlySpan<char>(pointer, int.MaxValue).IndexOf('\0'));
#else
#error Unsupported runtime.
#endif
}
