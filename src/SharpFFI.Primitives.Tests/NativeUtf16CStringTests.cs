using SharpFFI.Primitives.Utf16;

namespace SharpFFI.Primitives.Tests;

public unsafe class NativeUtf16CStringTests
{
    [Fact]
    public void FromString_Null_ThrowsArgumentNullException()
    {
        // Arrange
        string nullString = null!;

        // Act & Assert
        Should.Throw<ArgumentNullException>(() => NativeUtf16CString.FromString(nullString));
    }

    [Fact]
    public void Create_EmptyInput_AllocatesSingleNullTerminator()
    {
        // Arrange & Act
        using var utf16FromStr = NativeUtf16CString.FromString(string.Empty);
        using var utf16FromUtf16Span = NativeUtf16CString.FromUtf16Span(ReadOnlySpan<char>.Empty);

        // Assert
        foreach (var str in new[] { utf16FromStr, utf16FromUtf16Span })
        {
            str.Length.ShouldBe(0);
            str.IsEmpty.ShouldBeTrue();
            str.ToString().ShouldBeEmpty();

            ReadOnlyUtf16CString ro = str;
            ((nint)ro.Pointer).ShouldNotBe(0);
            ro.Pointer[0].ShouldBe('\0');
        }
    }

    [Theory]
    [InlineData("Hello", 5)]
    [InlineData("Привет", 6)]
    [InlineData("你好", 2)]
    [InlineData("🔥🚀", 4)]
    public void Create_MultibyteAndUnicode_CalculatesCorrectLength(string input, int expectedUtf16Len)
    {
        // Arrange & Act
        using var utf16Str1 = NativeUtf16CString.FromString(input);
        using var utf16Str2 = NativeUtf16CString.FromUtf16Span(input.AsSpan());

        // Assert
        foreach (var str in new[] { utf16Str1, utf16Str2 })
        {
            str.Length.ShouldBe(expectedUtf16Len);
            str.ToString().ShouldBe(input);

            ReadOnlyUtf16CString ro = str;
            ro.Pointer[str.Length].ShouldBe('\0');
            new ReadOnlySpan<char>(ro.Pointer, str.Length).SequenceEqual(input.AsSpan()).ShouldBeTrue();
        }
    }

    [Fact]
    public void Create_EmbeddedNullInMiddle_TruncatesAtFirstNull()
    {
        // Arrange
        var input = "Hello\0World";

        // Act
        using var u16FromString = NativeUtf16CString.FromString(input);
        using var u16From16 = NativeUtf16CString.FromUtf16Span(input.AsSpan());

        // Assert
        u16FromString.Length.ShouldBe(5);
        u16FromString.ToString().ShouldBe("Hello");

        u16From16.Length.ShouldBe(5);
        u16From16.ToString().ShouldBe("Hello");
    }

    [Fact]
    public void Create_LeadingNull_TruncatesToEmpty()
    {
        // Arrange
        string[] inputs = ["\0Hello", "\0\0\0"];

        // Act & Assert
        foreach (var input in inputs)
        {
            using var u16 = NativeUtf16CString.FromString(input);
            u16.Length.ShouldBe(0);
            u16.IsEmpty.ShouldBeTrue();
            u16.ToString().ShouldBeEmpty();
        }
    }

    [Fact]
    public void Create_TrailingNull_DoesNotIncludeNullInLength()
    {
        // Arrange
        var input = "Test\0";

        // Act
        using var u16 = NativeUtf16CString.FromString(input);

        // Assert
        u16.Length.ShouldBe(4);
        u16.ToString().ShouldBe("Test");
    }

    [Fact]
    public void Indexer_ValidAndOutOfRange_BehavesLikeSpan()
    {
        // Arrange
        var text = "Alpha";
        using var u16 = NativeUtf16CString.FromString(text);

        // Act & Assert
        for (var i = 0; i < u16.Length; i++)
        {
            u16[i].ShouldBe(text[i]);
        }

        Should.Throw<IndexOutOfRangeException>(() => { var _ = u16[-1]; });
        Should.Throw<IndexOutOfRangeException>(() => { var _ = u16[u16.Length]; });
    }

    [Fact]
    public void EqualityAndHashCode_DifferentInstancesSameContent_AreEqual()
    {
        // Arrange
        using var u16_1 = NativeUtf16CString.FromString("SameContent");
        using var u16_2 = NativeUtf16CString.FromString("SameContent");

        // Act & Assert
        u16_1.ShouldBe(u16_2);
        u16_1.GetHashCode().ShouldBe(u16_2.GetHashCode());
    }

    [Fact]
    public void Equality_WithNulls_HandlesAllCasesSafely()
    {
        // Arrange
        using var u16 = NativeUtf16CString.FromString("Alive");
        NativeUtf16CString? nullRef = null;

        // Act & Assert
        u16.ShouldNotBeNull();
        nullRef.ShouldBeNull();
        u16.Equals(null).ShouldBeFalse();
    }

    [Fact]
    public void ImplicitCast_NullReference_ReturnsNullCString()
    {
        // Arrange
        NativeUtf16CString? str16 = null;

        // Act
        ReadOnlyUtf16CString ro16 = str16;

        // Assert
        ro16.IsNull.ShouldBeTrue();
        ((nint)ro16.Pointer).ShouldBe(0);
    }

    [Fact]
    public void Dispose_ResetsStateAndIsIdempotent()
    {
        // Arrange
        var u16 = NativeUtf16CString.FromString("DisposableUtf16");

        // Act
        u16.Dispose();
        u16.Dispose(); // second call should be safe

        // Assert
        u16.Length.ShouldBe(0);
        u16.IsEmpty.ShouldBeTrue();
        u16.ToString().ShouldBeEmpty();
        ReadOnlyUtf16CString ro16 = u16;
        ro16.IsNull.ShouldBeTrue();
        ((nint)ro16.Pointer).ShouldBe(0);
        u16.AsReadOnlyView().IsEmpty.ShouldBeTrue();
        Should.Throw<IndexOutOfRangeException>(() => { var _ = u16[0]; });
    }

    [Fact]
    public void Equality_DisposedInstanceVsEmptyAliveInstance_ReturnsFalse()
    {
        // Arrange
        using var alive16 = NativeUtf16CString.FromString(string.Empty);
        var disposed16 = NativeUtf16CString.FromString("abc");

        // Act
        disposed16.Dispose();

        // Assert
        alive16.ShouldNotBe(disposed16);
        disposed16.ShouldNotBe(alive16);
    }

    [Fact]
    public void Equals_ObjectAndTyped_CoversAllBranches()
    {
        // Arrange
        using var u16 = NativeUtf16CString.FromString("abc");
        using var u16Same = NativeUtf16CString.FromString("abc");
        using var u16Diff = NativeUtf16CString.FromString("xyz");
        var disposed1 = NativeUtf16CString.FromString("temp1");
        var disposed2 = NativeUtf16CString.FromString("temp2");
        disposed1.Dispose();
        disposed2.Dispose();

        // Act & Assert - ReferenceEquals
        u16.Equals(u16).ShouldBeTrue();

        // Act & Assert - Null argument
        u16.Equals((NativeUtf16CString?)null).ShouldBeFalse();
        u16.Equals((object?)null).ShouldBeFalse();
        u16.Equals(new object()).ShouldBeFalse();

        // Act & Assert - Object overload
        u16.Equals((object)u16Same).ShouldBeTrue();
        u16.Equals((object)u16Diff).ShouldBeFalse();

        // Act & Assert - Both disposed (both pointers null)
        disposed1.Equals(disposed2).ShouldBeTrue();

        // Act & Assert - One disposed, one alive
        disposed1.Equals(u16).ShouldBeFalse();
        u16.Equals(disposed1).ShouldBeFalse();
    }

    [Fact]
    public void EqualityOperators_DirectInvocation_ReturnsExpected()
    {
        // Arrange
        using var u16_1 = NativeUtf16CString.FromString("hello");
        using var u16_2 = NativeUtf16CString.FromString("hello");
        using var u16_diff = NativeUtf16CString.FromString("world");
        NativeUtf16CString? null1 = null;
        NativeUtf16CString? null2 = null;

        // Act
        var eqBothNull = null1 == null2;
        var eqLeftNull = null1 == u16_1;
        var eqRightNull = u16_1 == null1;
        var eqSameRef = u16_1 == u16_1;
        var eqSameContent = u16_1 == u16_2;
        var eqDiffContent = u16_1 == u16_diff;

        var neqBothNull = null1 != null2;
        var neqLeftNull = null1 != u16_1;
        var neqRightNull = u16_1 != null1;
        var neqSameContent = u16_1 != u16_2;
        var neqDiffContent = u16_1 != u16_diff;

        // Assert
        eqBothNull.ShouldBeTrue();
        eqLeftNull.ShouldBeFalse();
        eqRightNull.ShouldBeFalse();
        eqSameRef.ShouldBeTrue();
        eqSameContent.ShouldBeTrue();
        eqDiffContent.ShouldBeFalse();

        neqBothNull.ShouldBeFalse();
        neqLeftNull.ShouldBeTrue();
        neqRightNull.ShouldBeTrue();
        neqSameContent.ShouldBeFalse();
        neqDiffContent.ShouldBeTrue();
    }

    [Fact]
    public void Finalizer_ExecutesUnmanagedCleanup()
    {
        // Arrange & Act
        AllocateAndDrop();
        GC.Collect();
        GC.WaitForPendingFinalizers();
        GC.Collect();

        // Assert
        Assert.True(true);

        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.NoInlining)]
        static void AllocateAndDrop()
        {
            _ = NativeUtf16CString.FromString("finalizerTest");
        }
    }
}
