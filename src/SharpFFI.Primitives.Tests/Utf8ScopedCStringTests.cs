using SharpFFI.Primitives.Utf8;

#pragma warning disable CS0809

namespace SharpFFI.Primitives.Tests;

public unsafe class Utf8ScopedCStringTests
{
    [Fact]
    public void Utf8Scoped_FastPath_UsesPinnedBufferWithoutByteCount()
    {
        // Arrange
        Span<byte> buf = stackalloc byte[16];

        // Act
        fixed (byte* pBuf = buf)
        {
            var scoped = Utf8ScopedCString.FromString("abc", buf);
            ReadOnlyUtf8CString ro = scoped;

            // Assert
            ((nint)ro.Pointer).ShouldBe((nint)pBuf);
            pBuf[3].ShouldBe((byte)0);
            scoped.Length.ShouldBe(3);
            scoped.ToString().ShouldBe("abc");

            scoped.Dispose();
        }
    }

    [Fact]
    public void Utf8Scoped_SlowPathFittingInBuffer_UsesPinnedBufferAfterExactCount()
    {
        // Arrange
        Span<byte> buf = stackalloc byte[6];

        // Act
        fixed (byte* pBuf = buf)
        {
            var scoped = Utf8ScopedCString.FromString("hello", buf);
            ReadOnlyUtf8CString ro = scoped;

            // Assert
            ((nint)ro.Pointer).ShouldBe((nint)pBuf);
            pBuf[5].ShouldBe((byte)0);
            scoped.Length.ShouldBe(5);
            scoped.ToString().ShouldBe("hello");

            scoped.Dispose();
        }
    }

    [Fact]
    public void Utf8Scoped_BufferTooSmallByOneByte_AllocatesOnHeap()
    {
        // Arrange
        Span<byte> buf = stackalloc byte[5];

        // Act
        fixed (byte* pBuf = buf)
        {
            var scoped = Utf8ScopedCString.FromString("hello", buf);
            ReadOnlyUtf8CString ro = scoped;

            // Assert
            ((nint)ro.Pointer).ShouldNotBe((nint)pBuf);
            ((nint)ro.Pointer).ShouldNotBe(0);
            scoped.Length.ShouldBe(5);
            scoped.ToString().ShouldBe("hello");

            scoped.Dispose();
        }
    }

    [Fact]
    public void Utf8Scoped_TruncatedLongString_TakesFastPath()
    {
        // Arrange
        var longStringWithNull = "ab\0" + new string('x', 1000);
        Span<byte> buf = stackalloc byte[8];

        // Act
        fixed (byte* pBuf = buf)
        {
            var scoped = Utf8ScopedCString.FromString(longStringWithNull, buf);
            ReadOnlyUtf8CString ro = scoped;

            // Assert
            ((nint)ro.Pointer).ShouldBe((nint)pBuf);
            scoped.Length.ShouldBe(2);
            scoped.ToString().ShouldBe("ab");

            scoped.Dispose();
        }
    }

    [Fact]
    public void DefaultInstance_IsSafeAndEmpty()
    {
        // Arrange & Act
        var defaultUtf8 = default(Utf8ScopedCString);

        // Assert
        defaultUtf8.Length.ShouldBe(0);
        defaultUtf8.IsEmpty.ShouldBeTrue();
        defaultUtf8.ToString().ShouldBeEmpty();

        ReadOnlyUtf8CString ro8 = defaultUtf8;
        ro8.IsNull.ShouldBeTrue();
        ((nint)ro8.Pointer).ShouldBe(0);

        defaultUtf8.Dispose();
    }

    [Fact]
    public void Dispose_HeapAllocated_ClearsStateAndPreventsDoubleFree()
    {
        // Arrange
        var scoped8 = Utf8ScopedCString.FromString("heapAllocatedString", default);

        // Act
        scoped8.Dispose();

        // Assert
        scoped8.Length.ShouldBe(0);
        scoped8.IsEmpty.ShouldBeTrue();
        ReadOnlyUtf8CString ro8 = scoped8;
        ((nint)ro8.Pointer).ShouldBe(0);

        scoped8.Dispose(); // second call should not throw or double free
    }

    [Fact]
    public void BoxedMethods_EqualsObjectAndGetHashCode_ThrowNotSupported()
    {
        // Arrange
        var scoped8 = Utf8ScopedCString.FromString("test", stackalloc byte[16]);

        // Act & Assert
        var equalsThrew = false;
        try
        {
            scoped8.Equals((object?)null);
        }
        catch (NotSupportedException)
        {
            equalsThrew = true;
        }
        equalsThrew.ShouldBeTrue();

        var hashThrew = false;
        try
        {
            scoped8.GetHashCode();
        }
        catch (NotSupportedException)
        {
            hashThrew = true;
        }
        hashThrew.ShouldBeTrue();

        scoped8.Dispose();
    }

    [Fact]
    public void FromUtf8Span_FittingAndFallbackAndEmbeddedNull_WorksCorrectly()
    {
        // Arrange
        var input = "hello\0world"u8;
        Span<byte> bufFit = stackalloc byte[10];
        Span<byte> bufSmall = stackalloc byte[4];

        // Act & Assert 1: Fits in pinned buffer with embedded null truncation
        fixed (byte* pFit = bufFit)
        {
            var scopedFit = Utf8ScopedCString.FromUtf8Span(input, bufFit);
            ReadOnlyUtf8CString roFit = scopedFit;

            ((nint)roFit.Pointer).ShouldBe((nint)pFit);
            scopedFit.Length.ShouldBe(5);
            scopedFit.ToString().ShouldBe("hello");
            pFit[5].ShouldBe((byte)0);

            scopedFit.Dispose();
        }

        // Act & Assert 2: Too small buffer -> falls back to unmanaged heap
        fixed (byte* pSmall = bufSmall)
        {
            var scopedHeap = Utf8ScopedCString.FromUtf8Span(input, bufSmall);
            ReadOnlyUtf8CString roHeap = scopedHeap;

            ((nint)roHeap.Pointer).ShouldNotBe((nint)pSmall);
            ((nint)roHeap.Pointer).ShouldNotBe(0);
            scopedHeap.Length.ShouldBe(5);
            scopedHeap.ToString().ShouldBe("hello");

            scopedHeap.Dispose();
        }

        // Act & Assert 3: No embedded null
        var scopedNoNull = Utf8ScopedCString.FromUtf8Span("abc"u8, default);
        scopedNoNull.Length.ShouldBe(3);
        scopedNoNull.ToString().ShouldBe("abc");
        scopedNoNull.Dispose();
    }

    [Fact]
    public void Indexer_ReturnsByteAtOffset()
    {
        // Arrange
        var scoped = Utf8ScopedCString.FromString("abc", stackalloc byte[8]);

        // Act & Assert
        scoped[0].ShouldBe((byte)'a');
        scoped[1].ShouldBe((byte)'b');
        scoped[2].ShouldBe((byte)'c');

        scoped.Dispose();
    }

    [Fact]
    public void Equals_AndEqualityOperators_CompareContent()
    {
        // Arrange
        var scoped1 = Utf8ScopedCString.FromString("test", stackalloc byte[8]);
        var scoped2 = Utf8ScopedCString.FromString("test", stackalloc byte[8]);
        var scopedDiff = Utf8ScopedCString.FromString("other", stackalloc byte[8]);

        // Act
        var eqMethod = scoped1.Equals(scoped2);
        var eqDiffMethod = scoped1.Equals(scopedDiff);
        var eqOp = scoped1 == scoped2;
        var eqOpDiff = scoped1 == scopedDiff;
        var neqOp = scoped1 != scopedDiff;
        var neqOpSame = scoped1 != scoped2;

        // Assert
        eqMethod.ShouldBeTrue();
        eqDiffMethod.ShouldBeFalse();
        eqOp.ShouldBeTrue();
        eqOpDiff.ShouldBeFalse();
        neqOp.ShouldBeTrue();
        neqOpSame.ShouldBeFalse();

        scoped1.Dispose();
        scoped2.Dispose();
        scopedDiff.Dispose();
    }
}
