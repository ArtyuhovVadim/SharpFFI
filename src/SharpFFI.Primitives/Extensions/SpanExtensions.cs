using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace SharpFFI.Primitives.Extensions;

internal static unsafe class SpanExtensions
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T* AsPointer<T>(this ReadOnlySpan<T> span) where T : unmanaged => (T*)Unsafe.AsPointer(ref MemoryMarshal.GetReference(span));

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T* AsPointer<T>(this Span<T> span) where T : unmanaged => (T*)Unsafe.AsPointer(ref MemoryMarshal.GetReference(span));
}