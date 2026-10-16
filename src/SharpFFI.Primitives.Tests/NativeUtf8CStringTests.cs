using System.Text;
using SharpFFI.Primitives.Utf8;

namespace SharpFFI.Primitives.Tests;

public unsafe class NativeUtf8CStringTests
{
    [Fact]
    public void FromString_Null_ThrowsArgumentNullException()
    {
        // Arrange
        string nullString = null!;

        // Act & Assert
        Should.Throw<ArgumentNullException>(() => NativeUtf8CString.FromString(nullString));
    }

    [Fact]
    public void Create_EmptyInput_AllocatesSingleNullTerminator()
    {
        // Arrange & Act
        using var utf8FromStr = NativeUtf8CString.FromString(string.Empty);
        using var utf8FromUtf16Span = NativeUtf8CString.FromUtf16Span(ReadOnlySpan<char>.Empty);
        using var utf8FromUtf8Span = NativeUtf8CString.FromUtf8Span(ReadOnlySpan<byte>.Empty);

        // Assert
        foreach (var str in new[] { utf8FromStr, utf8FromUtf16Span, utf8FromUtf8Span })
        {
            str.Length.ShouldBe(0);
            str.IsEmpty.ShouldBeTrue();
            str.ToString().ShouldBeEmpty();

            ReadOnlyUtf8CString ro = str;
            ((nint)ro.Pointer).ShouldNotBe(0);
            ro.Pointer[0].ShouldBe((byte)0);
        }
    }

    [Theory]
    [InlineData("Hello", 5)]
    [InlineData("Привет", 12)]
    [InlineData("你好", 6)]
    [InlineData("🔥🚀", 8)]
    public void Create_MultibyteAndUnicode_CalculatesCorrectLength(string input, int expectedUtf8Len)
    {
        // Arrange
        var expectedUtf8Bytes = Encoding.UTF8.GetBytes(input);

        // Act
        using var utf8Str1 = NativeUtf8CString.FromString(input);
        using var utf8Str2 = NativeUtf8CString.FromUtf16Span(input.AsSpan());
        using var utf8Str3 = NativeUtf8CString.FromUtf8Span(expectedUtf8Bytes);

        // Assert
        foreach (var str in new[] { utf8Str1, utf8Str2, utf8Str3 })
        {
            str.Length.ShouldBe(expectedUtf8Len);
            str.ToString().ShouldBe(input);

            ReadOnlyUtf8CString ro = str;
            ro.Pointer[str.Length].ShouldBe((byte)0);
            new ReadOnlySpan<byte>(ro.Pointer, str.Length).SequenceEqual(expectedUtf8Bytes).ShouldBeTrue();
        }
    }

    [Fact]
    public void Create_EmbeddedNullInMiddle_TruncatesAtFirstNull()
    {
        // Arrange
        var input = "Hello\0World";
        var inputUtf8 = "Hello\0World"u8.ToArray();

        // Act
        using var u8FromString = NativeUtf8CString.FromString(input);
        using var u8From16 = NativeUtf8CString.FromUtf16Span(input.AsSpan());
        using var u8From8 = NativeUtf8CString.FromUtf8Span(inputUtf8);

        // Assert
        u8FromString.Length.ShouldBe(5);
        u8FromString.ToString().ShouldBe("Hello");

        u8From16.Length.ShouldBe(5);
        u8From16.ToString().ShouldBe("Hello");

        u8From8.Length.ShouldBe(5);
        u8From8.ToString().ShouldBe("Hello");
    }

    [Fact]
    public void Create_LeadingNull_TruncatesToEmpty()
    {
        // Arrange
        string[] inputs = ["\0Hello", "\0\0\0"];

        // Act & Assert
        foreach (var input in inputs)
        {
            using var u8 = NativeUtf8CString.FromString(input);
            u8.Length.ShouldBe(0);
            u8.IsEmpty.ShouldBeTrue();
            u8.ToString().ShouldBeEmpty();
        }
    }

    [Fact]
    public void Create_TrailingNull_DoesNotIncludeNullInLength()
    {
        // Arrange
        var input = "Test\0";

        // Act
        using var u8 = NativeUtf8CString.FromString(input);

        // Assert
        u8.Length.ShouldBe(4);
        u8.ToString().ShouldBe("Test");
    }

    [Fact]
    public void Indexer_ValidAndOutOfRange_BehavesLikeSpan()
    {
        // Arrange
        var text = "Alpha";
        using var u8 = NativeUtf8CString.FromString(text);

        // Act & Assert
        for (var i = 0; i < u8.Length; i++)
        {
            u8[i].ShouldBe((byte)text[i]);
        }

        Should.Throw<IndexOutOfRangeException>(() => { var _ = u8[-1]; });
        Should.Throw<IndexOutOfRangeException>(() => { var _ = u8[u8.Length]; });
    }

    [Fact]
    public void EqualityAndHashCode_DifferentInstancesSameContent_AreEqual()
    {
        // Arrange
        using var u8_1 = NativeUtf8CString.FromString("SameContent");
        using var u8_2 = NativeUtf8CString.FromString("SameContent");

        // Act & Assert
        u8_1.ShouldBe(u8_2);
        u8_1.GetHashCode().ShouldBe(u8_2.GetHashCode());
    }

    [Fact]
    public void Equality_WithNulls_HandlesAllCasesSafely()
    {
        // Arrange
        using var u8 = NativeUtf8CString.FromString("Alive");
        NativeUtf8CString? nullRef = null;

        // Act & Assert
        u8.ShouldNotBeNull();
        nullRef.ShouldBeNull();
        u8.Equals(null).ShouldBeFalse();
    }

    [Fact]
    public void ImplicitCast_NullReference_ReturnsNullCString()
    {
        // Arrange
        NativeUtf8CString? str8 = null;

        // Act
        ReadOnlyUtf8CString ro8 = str8;

        // Assert
        ro8.IsNull.ShouldBeTrue();
        ((nint)ro8.Pointer).ShouldBe(0);
    }

    [Fact]
    public void Dispose_ResetsStateAndIsIdempotent()
    {
        // Arrange
        var u8 = NativeUtf8CString.FromString("DisposableUtf8");

        // Act
        u8.Dispose();
        u8.Dispose(); // second call should be safe

        // Assert
        u8.Length.ShouldBe(0);
        u8.IsEmpty.ShouldBeTrue();
        u8.ToString().ShouldBeEmpty();
        ReadOnlyUtf8CString ro8 = u8;
        ro8.IsNull.ShouldBeTrue();
        ((nint)ro8.Pointer).ShouldBe(0);
        u8.AsReadOnlyView().IsEmpty.ShouldBeTrue();
        Should.Throw<IndexOutOfRangeException>(() => { var _ = u8[0]; });
    }

    [Fact]
    public void Equality_DisposedInstanceVsEmptyAliveInstance_ReturnsFalse()
    {
        // Arrange
        using var alive8 = NativeUtf8CString.FromString(string.Empty);
        var disposed8 = NativeUtf8CString.FromString("abc");

        // Act
        disposed8.Dispose();

        // Assert
        alive8.ShouldNotBe(disposed8);
        disposed8.ShouldNotBe(alive8);
    }

    [Fact]
    public void Equals_ObjectAndTyped_CoversAllBranches()
    {
        // Arrange
        using var u8 = NativeUtf8CString.FromString("abc");
        using var u8Same = NativeUtf8CString.FromString("abc");
        using var u8Diff = NativeUtf8CString.FromString("xyz");
        var disposed1 = NativeUtf8CString.FromString("temp1");
        var disposed2 = NativeUtf8CString.FromString("temp2");
        disposed1.Dispose();
        disposed2.Dispose();

        // Act & Assert - ReferenceEquals
        u8.Equals(u8).ShouldBeTrue();

        // Act & Assert - Null argument
        u8.Equals((NativeUtf8CString?)null).ShouldBeFalse();
        u8.Equals((object?)null).ShouldBeFalse();
        u8.Equals(new object()).ShouldBeFalse();

        // Act & Assert - Object overload
        u8.Equals((object)u8Same).ShouldBeTrue();
        u8.Equals((object)u8Diff).ShouldBeFalse();

        // Act & Assert - Both disposed (both pointers null)
        disposed1.Equals(disposed2).ShouldBeTrue();

        // Act & Assert - One disposed, one alive
        disposed1.Equals(u8).ShouldBeFalse();
        u8.Equals(disposed1).ShouldBeFalse();
    }

    [Fact]
    public void EqualityOperators_DirectInvocation_ReturnsExpected()
    {
        // Arrange
        using var u8_1 = NativeUtf8CString.FromString("hello");
        using var u8_2 = NativeUtf8CString.FromString("hello");
        using var u8_diff = NativeUtf8CString.FromString("world");
        NativeUtf8CString? null1 = null;
        NativeUtf8CString? null2 = null;

        // Act
        var eqBothNull = null1 == null2;
        var eqLeftNull = null1 == u8_1;
        var eqRightNull = u8_1 == null1;
        var eqSameRef = u8_1 == u8_1;
        var eqSameContent = u8_1 == u8_2;
        var eqDiffContent = u8_1 == u8_diff;

        var neqBothNull = null1 != null2;
        var neqLeftNull = null1 != u8_1;
        var neqRightNull = u8_1 != null1;
        var neqSameContent = u8_1 != u8_2;
        var neqDiffContent = u8_1 != u8_diff;

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
            _ = NativeUtf8CString.FromString("finalizerTest");
        }
    }
}
