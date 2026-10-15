using System.Runtime.CompilerServices;

namespace SharpFFI.Primitives.Extensions;

internal unsafe static class FrameworkExtensions
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int GetHashCode(ReadOnlySpan<byte> span) => throw new NotImplementedException();

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int GetHashCode(ReadOnlySpan<char> span) => throw new NotImplementedException();

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static string ToString(ReadOnlySpan<byte> span) => throw new NotImplementedException();

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static string ToString(ReadOnlySpan<char> span) => throw new NotImplementedException();

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T* NativeAlloc<T>(nuint bytesCount) where T : unmanaged => throw new NotImplementedException();

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void NativeFree(void* pointer) => throw new NotImplementedException();

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int GetUtf8BytesCount(ReadOnlySpan<char> str) => throw new NotImplementedException();

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int GetUtf8Bytes(ReadOnlySpan<char> chars, Span<byte> bytes) => throw new NotImplementedException();

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Span<byte> CreateSpanFromNullTerminated(byte* pointer) => throw new NotImplementedException();

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Span<char> CreateSpanFromNullTerminated(char* pointer) => throw new NotImplementedException();
}
