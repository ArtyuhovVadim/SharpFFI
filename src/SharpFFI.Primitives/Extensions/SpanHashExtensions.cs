using System;
using System.Runtime.CompilerServices;

namespace SharpFFI.Primitives.Extensions;

public static class SpanHashExtensions
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int GetMarvinHash(this ReadOnlySpan<byte> span) => Marvin.ComputeHash32(span, Marvin.DefaultSeed);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int GetMarvinHash(this Span<byte> span) => Marvin.ComputeHash32(span, Marvin.DefaultSeed);
}
