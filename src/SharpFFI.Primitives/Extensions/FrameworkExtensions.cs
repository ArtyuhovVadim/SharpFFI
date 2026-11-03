using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace SharpFFI.Primitives.Extensions;

internal unsafe static class FrameworkExtensions
{
#if NET6_0_OR_GREATER

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T* NativeAlloc<T>(nuint bytesCount) where T : unmanaged => (T*)NativeMemory.Alloc(bytesCount);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void NativeFree(void* pointer) => NativeMemory.Free(pointer);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Span<byte> CreateSpanFromNullTerminated(byte* pointer) => pointer is null ? default : MemoryMarshal.CreateSpan(ref *pointer, MemoryMarshal.CreateReadOnlySpanFromNullTerminated(pointer).Length);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Span<char> CreateSpanFromNullTerminated(char* pointer) => pointer is null ? default : MemoryMarshal.CreateSpan(ref *pointer, MemoryMarshal.CreateReadOnlySpanFromNullTerminated(pointer).Length);

#else

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T* NativeAlloc<T>(nuint bytesCount) where T : unmanaged => (T*)Marshal.AllocHGlobal((nint)bytesCount);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void NativeFree(void* pointer) => Marshal.FreeHGlobal((nint)pointer);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Span<byte> CreateSpanFromNullTerminated(byte* pointer) => pointer is null ? default : MemoryMarshal.CreateSpan(ref *pointer, new ReadOnlySpan<byte>(pointer, int.MaxValue).IndexOf((byte)0));

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Span<char> CreateSpanFromNullTerminated(char* pointer) => pointer is null ? default : MemoryMarshal.CreateSpan(ref *pointer, new ReadOnlySpan<char>(pointer, int.MaxValue).IndexOf('\0'));

#endif
}
