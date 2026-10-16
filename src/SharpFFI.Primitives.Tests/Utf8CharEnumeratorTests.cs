using System.Text;
using SharpFFI.Primitives.Extensions;
using SharpFFI.Primitives.Utf8;

namespace SharpFFI.Primitives.Tests;

public class Utf8CharEnumeratorTests
{
    [Fact]
    public void Enumerate_EmptyView_DoesNotIterate()
    {
        // Arrange
        var view = ReadOnlyUtf8StringView.Empty;
        var enumerator = view.GetEnumerator();

        // Act
        var hasNext = enumerator.MoveNext();
        var loopCount = 0;
        foreach (var _ in view)
        {
            loopCount++;
        }

        // Assert
        hasNext.ShouldBeFalse();
        loopCount.ShouldBe(0);
    }

    [Fact]
    public void Enumerate_BmpCharacters_Decodes1To3ByteSequences()
    {
        // Arrange
        var expectedString = "AЯé中€TestТест";
        var utf8Bytes = Encoding.UTF8.GetBytes(expectedString);
        var view = ReadOnlyUtf8StringView.FromUtf8Span(utf8Bytes);
        var chars = new List<char>();

        // Act
        foreach (var c in view)
        {
            chars.Add(c);
        }

        // Assert
        chars.ToArray().ShouldBe(expectedString.ToCharArray());
    }

    [Fact]
    public void Enumerate_SurrogatePairs_YieldsHighThenLowSurrogate()
    {
        // Arrange
        string[] testStrings = [
            "🔥",
            "🎉",
            "\U0001D11E", // 𝄞
            "A🔥Б🚀C"
        ];

        // Act & Assert - All strings
        foreach (var text in testStrings)
        {
            var utf8Bytes = Encoding.UTF8.GetBytes(text);
            var view = ReadOnlyUtf8StringView.FromUtf8Span(utf8Bytes);
            var enumerator = view.GetEnumerator();
            var decodedChars = new List<char>();

            while (enumerator.MoveNext())
            {
                decodedChars.Add(enumerator.Current);
            }

            decodedChars.ToArray().ShouldBe(text.ToCharArray());
        }

        // Detailed step-by-step verification on a single emoji "🔥"
        // Arrange
        var emoji = "🔥";
        var emojiUtf8 = Encoding.UTF8.GetBytes(emoji);
        var emojiView = ReadOnlyUtf8StringView.FromUtf8Span(emojiUtf8);
        var emojiEnumerator = emojiView.GetEnumerator();

        // Act & Assert - Step 1: High surrogate
        emojiEnumerator.MoveNext().ShouldBeTrue();
        char.IsHighSurrogate(emojiEnumerator.Current).ShouldBeTrue();
        emojiEnumerator.Current.ShouldBe(emoji[0]);

        // Act & Assert - Step 2: Low surrogate from pending
        emojiEnumerator.MoveNext().ShouldBeTrue();
        char.IsLowSurrogate(emojiEnumerator.Current).ShouldBeTrue();
        emojiEnumerator.Current.ShouldBe(emoji[1]);

        // Act & Assert - Step 3: Finished
        emojiEnumerator.MoveNext().ShouldBeFalse();
    }

    [Fact]
    public void Enumerate_MalformedUtf8_ReplacesWithReplacementCharAndProgresses()
    {
        // 1. Standalone continuation byte [0x80]
        // Arrange
        byte[] continuationByte = [0x80];
        var view1 = ReadOnlyUtf8StringView.FromUtf8Span(continuationByte);
        var enumerator1 = view1.GetEnumerator();

        // Act & Assert
        enumerator1.MoveNext().ShouldBeTrue();
        enumerator1.Current.ShouldBe((char)UnicodeUtility.ReplacementChar);
        enumerator1.MoveNext().ShouldBeFalse();

        // 2. Forbidden byte [0xFF]
        // Arrange
        byte[] forbiddenByte = [0xFF];
        var view2 = ReadOnlyUtf8StringView.FromUtf8Span(forbiddenByte);
        var enumerator2 = view2.GetEnumerator();

        // Act & Assert
        enumerator2.MoveNext().ShouldBeTrue();
        enumerator2.Current.ShouldBe((char)UnicodeUtility.ReplacementChar);
        enumerator2.MoveNext().ShouldBeFalse();

        // 3. Truncated 3-byte sequence [0xE2, 0x82]
        // Arrange
        byte[] truncated3 = [0xE2, 0x82];
        var view3 = ReadOnlyUtf8StringView.FromUtf8Span(truncated3);
        var enumerator3 = view3.GetEnumerator();

        // Act & Assert
        enumerator3.MoveNext().ShouldBeTrue();
        enumerator3.Current.ShouldBe((char)UnicodeUtility.ReplacementChar);
        enumerator3.MoveNext().ShouldBeFalse();

        // 4. Truncated 4-byte sequence [0xF0, 0x9F, 0x94]
        // Arrange
        byte[] truncated4 = [0xF0, 0x9F, 0x94];
        var view4 = ReadOnlyUtf8StringView.FromUtf8Span(truncated4);
        var enumerator4 = view4.GetEnumerator();

        // Act & Assert
        enumerator4.MoveNext().ShouldBeTrue();
        enumerator4.Current.ShouldBe((char)UnicodeUtility.ReplacementChar);
        enumerator4.MoveNext().ShouldBeFalse();

        // 5. Invalid byte followed by valid text: [0xFF, (byte)'A', 0x80, (byte)'B']
        // Arrange
        byte[] mixed = [0xFF, (byte)'A', 0x80, (byte)'B'];
        var view5 = ReadOnlyUtf8StringView.FromUtf8Span(mixed);
        var chars = new List<char>();

        // Act
        foreach (var c in view5)
        {
            chars.Add(c);
        }

        // Assert
        chars.Count.ShouldBe(4);
        chars[0].ShouldBe((char)UnicodeUtility.ReplacementChar);
        chars[1].ShouldBe('A');
        chars[2].ShouldBe((char)UnicodeUtility.ReplacementChar);
        chars[3].ShouldBe('B');
    }

    [Fact]
    public void Enumerate_InvalidUtf8EdgeCases_ReplacesWithReplacementChar()
    {
        // Arrange
        byte[][] edgeCases = [
            [0xC2],                     // 1 byte when 2 needed
            [0xC2, 0x20],               // 2nd byte not continuation
            [0xE0, 0x80, 0x80],         // Overlong 3-byte
            [0xED, 0xA0, 0x80],         // UTF-16 surrogate in UTF-8
            [0xF0, 0x80, 0x80, 0x80],   // Overlong 4-byte
            [0xE2, 0x82, 0x20],         // 3rd byte not continuation
            [0xF0, 0x9F, 0x94, 0x20]    // 4th byte not continuation
        ];

        // Act & Assert
        foreach (var invalidSeq in edgeCases)
        {
            var view = ReadOnlyUtf8StringView.FromUtf8Span(invalidSeq);
            var enumerator = view.GetEnumerator();
            enumerator.MoveNext().ShouldBeTrue();
            enumerator.Current.ShouldBe((char)UnicodeUtility.ReplacementChar);
        }
    }

    [Fact]
    public void DecodeScalarFromUtf8_EmptySource_ReturnsNeedMoreData()
    {
        // Arrange
        var empty = ReadOnlySpan<byte>.Empty;

        // Act
        var status = UnicodeUtility.DecodeScalarFromUtf8(empty, out var result, out var bytesConsumed);

        // Assert
        status.ShouldBe(System.Buffers.OperationStatus.NeedMoreData);
        result.ShouldBe(UnicodeUtility.ReplacementChar);
        bytesConsumed.ShouldBe(0);
    }
}
