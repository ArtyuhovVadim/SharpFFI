using SharpFFI.Primitives.Extensions;

namespace SharpFFI.Primitives.Tests;

public unsafe class FrameworkExtensionsTests
{
    [Fact]
    public void MarvinHash_AllTailLengthsAndUnalignedMemory_ProduceConsistentHashes()
    {
        // Arrange
        var seed = 0x123456789ABCDEF0UL;
        var pattern = new byte[64];
        for (var i = 0; i < pattern.Length; i++)
        {
            pattern[i] = (byte)(i * 17 + 1);
        }

        var bufferAligned = new byte[64];
        var bufferUnaligned = new byte[65];

        // Act & Assert - Test all tail lengths from 0 to 40 bytes across alignments
        for (var len = 0; len <= 40; len++)
        {
            Array.Copy(pattern, 0, bufferAligned, 0, len);
            // Put copy at offset 1 to simulate unaligned memory access
            Array.Copy(pattern, 0, bufferUnaligned, 1, len);

            ReadOnlySpan<byte> alignedSpan = bufferAligned.AsSpan(0, len);
            ReadOnlySpan<byte> unalignedSpan = bufferUnaligned.AsSpan(1, len);

            var hashAligned = Marvin.ComputeHash32(alignedSpan, seed);
            var hashUnaligned = Marvin.ComputeHash32(unalignedSpan, seed);

            hashAligned.ShouldBe(hashUnaligned);
        }
    }

    [Fact]
    public void SpanExtensions_AsPointer_SpanAndReadOnlySpan_ReturnsValidPointer()
    {
        // Arrange
        byte[] data = [10, 20, 30];
        var span = data.AsSpan();
        ReadOnlySpan<byte> roSpan = data.AsSpan();

        // Act & Assert
        fixed (byte* pExpected = data)
        {
            var pSpan = span.AsPointer();
            var pRoSpan = roSpan.AsPointer();

            ((nint)pSpan).ShouldBe((nint)pExpected);
            ((nint)pRoSpan).ShouldBe((nint)pExpected);
        }
    }

    [Theory]
    [InlineData(0)]
    [InlineData(1)]
    [InlineData(15)]
    [InlineData(16)]
    [InlineData(31)]
    [InlineData(32)]
    [InlineData(64)]
    [InlineData(4096)]
    public void CreateSpanFromNullTerminated_Utf8_FindsCorrectLength(int length)
    {
        // Arrange
        var bytes = new byte[length + 1];
        for (var i = 0; i < length; i++)
        {
            bytes[i] = (byte)'A';
        }
        bytes[length] = 0;

        // Act & Assert
        fixed (byte* pBytes = bytes)
        {
            var span8 = FrameworkExtensions.CreateSpanFromNullTerminated(pBytes);
            span8.Length.ShouldBe(length);
        }
    }

    [Theory]
    [InlineData(0)]
    [InlineData(1)]
    [InlineData(15)]
    [InlineData(16)]
    [InlineData(31)]
    [InlineData(32)]
    [InlineData(64)]
    [InlineData(4096)]
    public void CreateSpanFromNullTerminated_Utf16_FindsCorrectLength(int length)
    {
        // Arrange
        var chars = new char[length + 1];
        for (var i = 0; i < length; i++)
        {
            chars[i] = 'A';
        }
        chars[length] = '\0';

        // Act & Assert
        fixed (char* pChars = chars)
        {
            var span16 = FrameworkExtensions.CreateSpanFromNullTerminated(pChars);
            span16.Length.ShouldBe(length);
        }
    }
}
