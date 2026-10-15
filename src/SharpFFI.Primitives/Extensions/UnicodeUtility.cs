using System.Buffers;
using System.Runtime.CompilerServices;

namespace SharpFFI.Primitives.Extensions;

internal static class UnicodeUtility
{
    /// <summary>
    /// The Unicode replacement character U+FFFD.
    /// </summary>
    public const uint ReplacementChar = 0xFFFD;

    /// <summary>
    /// Returns <see langword="true"/> iff <paramref name="value"/> is an ASCII
    /// character ([ U+0000..U+007F ]).
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsAsciiCodePoint(uint value) => value <= 0x7Fu;

    /// <summary>
    /// Returns <see langword="true"/> iff <paramref name="value"/> is in the
    /// Basic Multilingual Plane (BMP).
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsBmpCodePoint(uint value) => value <= 0xFFFFu;

    /// <summary>
    /// Returns <see langword="true"/> iff <paramref name="value"/> is between
    /// <paramref name="lowerBound"/> and <paramref name="upperBound"/>, inclusive.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsInRangeInclusive(uint value, uint lowerBound, uint upperBound) => (value - lowerBound) <= (upperBound - lowerBound);

    /// <summary>
    /// Decomposes an astral Unicode scalar into UTF-16 high and low surrogate code units.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void GetUtf16SurrogatesFromSupplementaryPlaneScalar(uint value, out char highSurrogateCodePoint, out char lowSurrogateCodePoint)
    {
        highSurrogateCodePoint = (char)((value + ((0xD800u - 0x40u) << 10)) >> 10);
        lowSurrogateCodePoint = (char)((value & 0x3FFu) + 0xDC00u);
    }

    /// <summary>
    /// Decodes the first Unicode scalar value from the provided UTF-8 source buffer.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static OperationStatus DecodeScalarFromUtf8(ReadOnlySpan<byte> source, out uint result, out int bytesConsumed)
    {
        int index = 0;
        if (source.IsEmpty)
        {
            goto NeedsMoreData;
        }

        uint tempValue = source[0];
        if (IsAsciiCodePoint(tempValue))
        {
            bytesConsumed = 1;
            result = tempValue;
            return OperationStatus.Done;
        }

        index = 1;
        if (!IsInRangeInclusive(tempValue, 0xC2, 0xF4))
        {
            goto Invalid;
        }

        tempValue = (tempValue - 0xC2) << 6;

        if (source.Length <= 1)
        {
            goto NeedsMoreData;
        }

        int thisByteSignExtended = (sbyte)source[1];
        if (thisByteSignExtended >= -64)
        {
            goto Invalid;
        }

        tempValue += (uint)thisByteSignExtended;
        tempValue += 0x80;
        tempValue += (0xC2 - 0xC0) << 6;

        if (tempValue < 0x0800)
        {
            goto Finish;
        }

        if (!IsInRangeInclusive(tempValue, ((0xE0 - 0xC0) << 6) + (0xA0 - 0x80), ((0xF4 - 0xC0) << 6) + (0x8F - 0x80)))
        {
            goto Invalid;
        }

        if (IsInRangeInclusive(tempValue, ((0xED - 0xC0) << 6) + (0xA0 - 0x80), ((0xED - 0xC0) << 6) + (0xBF - 0x80)))
        {
            goto Invalid;
        }

        if (IsInRangeInclusive(tempValue, ((0xF0 - 0xC0) << 6) + (0x80 - 0x80), ((0xF0 - 0xC0) << 6) + (0x8F - 0x80)))
        {
            goto Invalid;
        }

        index = 2;
        if (source.Length <= 2)
        {
            goto NeedsMoreData;
        }

        thisByteSignExtended = (sbyte)source[2];
        if (thisByteSignExtended >= -64)
        {
            goto Invalid;
        }

        tempValue <<= 6;
        tempValue += (uint)thisByteSignExtended;
        tempValue += 0x80;
        tempValue -= (0xE0 - 0xC0) << 12;

        if (tempValue <= 0xFFFF)
        {
            goto Finish;
        }

        index = 3;
        if (source.Length <= 3)
        {
            goto NeedsMoreData;
        }

        thisByteSignExtended = (sbyte)source[3];
        if (thisByteSignExtended >= -64)
        {
            goto Invalid;
        }

        tempValue <<= 6;
        tempValue += (uint)thisByteSignExtended;
        tempValue += 0x80;
        tempValue -= (0xF0 - 0xE0) << 18;

    Finish:
        bytesConsumed = index + 1;
        result = tempValue;
        return OperationStatus.Done;

    NeedsMoreData:
        bytesConsumed = index;
        result = ReplacementChar;
        return OperationStatus.NeedMoreData;

    Invalid:
        bytesConsumed = index;
        result = ReplacementChar;
        return OperationStatus.InvalidData;
    }
}
