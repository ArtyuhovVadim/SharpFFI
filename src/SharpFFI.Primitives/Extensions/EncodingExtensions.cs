using System;
using System.Runtime.CompilerServices;
using System.Text;

namespace SharpFFI.Primitives.Extensions;

internal static unsafe class EncodingExtensions
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static string GetString(this Encoding encoding, ReadOnlySpan<byte> bytes) => encoding.GetString(bytes.AsPointer(), bytes.Length);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int GetBytes(this Encoding encoding, ReadOnlySpan<char> chars, Span<byte> bytes) => encoding.GetBytes(chars.AsPointer(), chars.Length, bytes.AsPointer(), bytes.Length);
}