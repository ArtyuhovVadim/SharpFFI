using SharpFFI.Primitives.Utf16;

#pragma warning disable CS0809

namespace SharpFFI.Primitives.Tests;

public unsafe class Utf16ScopedCStringTests
{
    [Fact]
    public void Utf16Scoped_ExactFitAndHeapFallback_SelectsCorrectMemory()
    {
        // Arrange
        var input = "abcd"; // 4 chars

        // Act & Assert 1: Buffer of 5 chars fits with null terminator
        Span<char> buf5 = stackalloc char[5];
        fixed (char* pBuf5 = buf5)
        {
            var scoped1 = Utf16ScopedCString.FromUtf16Span(input.AsSpan(), buf5);
            ReadOnlyUtf16CString ro1 = scoped1;

            ((nint)ro1.Pointer).ShouldBe((nint)pBuf5);
            pBuf5[4].ShouldBe('\0');
            scoped1.Length.ShouldBe(4);
            scoped1.ToString().ShouldBe("abcd");
            scoped1.Dispose();
        }

        // Act & Assert 2: Buffer of 4 chars does not fit null terminator -> heap fallback
        Span<char> buf4 = stackalloc char[4];
        fixed (char* pBuf4 = buf4)
        {
            var scoped2 = Utf16ScopedCString.FromUtf16Span(input.AsSpan(), buf4);
            ReadOnlyUtf16CString ro2 = scoped2;

            ((nint)ro2.Pointer).ShouldNotBe((nint)pBuf4);
            ((nint)ro2.Pointer).ShouldNotBe(0);
            scoped2.Length.ShouldBe(4);
            scoped2.ToString().ShouldBe("abcd");
            scoped2.Dispose();
        }

        // Act & Assert 3: Default buffer -> heap fallback
        var scoped3 = Utf16ScopedCString.FromUtf16Span(input.AsSpan(), default);
        ReadOnlyUtf16CString ro3 = scoped3;

        ((nint)ro3.Pointer).ShouldNotBe(0);
        scoped3.Length.ShouldBe(4);
        scoped3.ToString().ShouldBe("abcd");
        scoped3.Dispose();
    }

    [Fact]
    public void DefaultInstance_IsSafeAndEmpty()
    {
        // Arrange & Act
        var defaultUtf16 = default(Utf16ScopedCString);

        // Assert
        defaultUtf16.Length.ShouldBe(0);
        defaultUtf16.IsEmpty.ShouldBeTrue();
        defaultUtf16.ToString().ShouldBeEmpty();

        ReadOnlyUtf16CString ro16 = defaultUtf16;
        ro16.IsNull.ShouldBeTrue();
        ((nint)ro16.Pointer).ShouldBe(0);

        defaultUtf16.Dispose();
    }

    [Fact]
    public void Dispose_HeapAllocated_ClearsStateAndPreventsDoubleFree()
    {
        // Arrange
        var scoped16 = Utf16ScopedCString.FromString("heapAllocatedString", default);

        // Act
        scoped16.Dispose();

        // Assert
        scoped16.Length.ShouldBe(0);
        scoped16.IsEmpty.ShouldBeTrue();
        ReadOnlyUtf16CString ro16 = scoped16;
        ((nint)ro16.Pointer).ShouldBe(0);

        scoped16.Dispose(); // second call should not throw or double free
    }

    [Fact]
    public void BoxedMethods_EqualsObjectAndGetHashCode_ThrowNotSupported()
    {
        // Arrange
        var scoped16 = Utf16ScopedCString.FromString("test", stackalloc char[16]);

        // Act & Assert
        var equalsThrew = false;
        try
        {
            scoped16.Equals((object?)null);
        }
        catch (NotSupportedException)
        {
            equalsThrew = true;
        }
        equalsThrew.ShouldBeTrue();

        var hashThrew = false;
        try
        {
            scoped16.GetHashCode();
        }
        catch (NotSupportedException)
        {
            hashThrew = true;
        }
        hashThrew.ShouldBeTrue();

        scoped16.Dispose();
    }

    [Fact]
    public void FromUtf16Span_WithEmbeddedNull_TruncatesAtFirstNull()
    {
        // Arrange
        var input = "hello\0world";
        Span<char> buf = stackalloc char[10];

        // Act
        var scoped = Utf16ScopedCString.FromUtf16Span(input.AsSpan(), buf);

        // Assert
        scoped.Length.ShouldBe(5);
        scoped.ToString().ShouldBe("hello");

        scoped.Dispose();
    }

    [Fact]
    public void Indexer_ReturnsCharAtOffset()
    {
        // Arrange
        var scoped = Utf16ScopedCString.FromString("abc", stackalloc char[8]);

        // Act & Assert
        scoped[0].ShouldBe('a');
        scoped[1].ShouldBe('b');
        scoped[2].ShouldBe('c');

        scoped.Dispose();
    }

    [Fact]
    public void Equals_AndEqualityOperators_CompareContent()
    {
        // Arrange
        var scoped1 = Utf16ScopedCString.FromString("test", stackalloc char[8]);
        var scoped2 = Utf16ScopedCString.FromString("test", stackalloc char[8]);
        var scopedDiff = Utf16ScopedCString.FromString("other", stackalloc char[8]);

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
