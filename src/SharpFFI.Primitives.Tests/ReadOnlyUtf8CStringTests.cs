using SharpFFI.Primitives.Utf8;

namespace SharpFFI.Primitives.Tests;

public unsafe class ReadOnlyUtf8CStringTests
{
    [Fact]
    public void NullState_PropertiesAndConversions()
    {
        // Arrange
        ReadOnlyUtf8CString[] nullRoUtf8S = [
            default,
            ReadOnlyUtf8CString.Null,
            ReadOnlyUtf8CString.FromPointer((byte*)null),
            ReadOnlyUtf8CString.FromPointer((nint)0)
        ];

        // Act & Assert
        foreach (var cstr in nullRoUtf8S)
        {
            cstr.IsNull.ShouldBeTrue();
            cstr.IsEmpty.ShouldBeTrue();
            ((nint)cstr.Pointer).ShouldBe(0);
            cstr.ToString().ShouldBeEmpty();
            cstr.GetHashCode().ShouldBe(0);
            cstr.AsReadOnlyView().IsEmpty.ShouldBeTrue();
        }
    }

    [Fact]
    public void EmptyNonNullState_DistinguishesIsNullAndIsEmpty()
    {
        // Arrange
        byte zero = 0;

        // Act
        var roCstr = ReadOnlyUtf8CString.FromPointer(&zero);

        // Assert
        roCstr.IsNull.ShouldBeFalse();
        roCstr.IsEmpty.ShouldBeTrue();
        roCstr.AsReadOnlyView().Length.ShouldBe(0);
        roCstr.ToString().ShouldBeEmpty();
    }

    [Fact]
    public void Equality_NullVsEmptyNonNull_ReturnsFalse()
    {
        // Arrange
        byte zero = 0;
        var nullRo = ReadOnlyUtf8CString.Null;
        var emptyRo = ReadOnlyUtf8CString.FromPointer(&zero);

        // Act & Assert
        nullRo.ShouldNotBe(emptyRo);
        nullRo.AsReadOnlyView().Equals(emptyRo.AsReadOnlyView()).ShouldBeTrue();
    }

    [Fact]
    public void Equality_DifferentBuffersSameNullTerminatedPrefix_ReturnsTrue()
    {
        // Arrange
        var bufA = "ab\0X"u8.ToArray();
        var bufB = "ab\0Y"u8.ToArray();

        // Act & Assert
        fixed (byte* pA = bufA, pB = bufB)
        {
            var roA = ReadOnlyUtf8CString.FromPointer(pA);
            var roB = ReadOnlyUtf8CString.FromPointer(pB);

            roA.ShouldBe(roB);
            roA.GetHashCode().ShouldBe(roB.GetHashCode());
            roA.ToString().ShouldBe("ab");
            roB.ToString().ShouldBe("ab");
        }
    }

    [Fact]
    public void Indexer_ReturnsByteAtOffset()
    {
        // Arrange
        var buf = "abc\0"u8.ToArray();

        // Act & Assert
        fixed (byte* p = buf)
        {
            var ro = ReadOnlyUtf8CString.FromPointer(p);
            ro[0].ShouldBe((byte)'a');
            ro[1].ShouldBe((byte)'b');
            ro[2].ShouldBe((byte)'c');
            ro[3].ShouldBe((byte)0);
        }
    }

    [Fact]
    public void Equals_AndOperators_CoverAllBranches()
    {
        // Arrange
        var buf1 = "hello\0"u8.ToArray();
        var buf2 = "hello\0"u8.ToArray();
        var bufDiff = "world\0"u8.ToArray();

        // Act & Assert
        fixed (byte* p1 = buf1, p2 = buf2, pDiff = bufDiff)
        {
            var ro1 = ReadOnlyUtf8CString.FromPointer(p1);
            var ro2 = ReadOnlyUtf8CString.FromPointer(p2);
            var roDiff = ReadOnlyUtf8CString.FromPointer(pDiff);
            var roNull = ReadOnlyUtf8CString.Null;

            // Act & Assert - Same pointer
            ro1.Equals(ro1).ShouldBeTrue();
            (ro1 == ro1).ShouldBeTrue();
            (ro1 != ro1).ShouldBeFalse();

            // Act & Assert - Both null
            roNull.Equals(roNull).ShouldBeTrue();
            (roNull == roNull).ShouldBeTrue();
            (roNull != roNull).ShouldBeFalse();

            // Act & Assert - One null, one non-null
            roNull.Equals(ro1).ShouldBeFalse();
            ro1.Equals(roNull).ShouldBeFalse();
            (roNull == ro1).ShouldBeFalse();
            (ro1 == roNull).ShouldBeFalse();
            (roNull != ro1).ShouldBeTrue();
            (ro1 != roNull).ShouldBeTrue();

            // Act & Assert - Different pointers same content
            ro1.Equals(ro2).ShouldBeTrue();
            (ro1 == ro2).ShouldBeTrue();
            (ro1 != ro2).ShouldBeFalse();

            // Act & Assert - Different content
            ro1.Equals(roDiff).ShouldBeFalse();
            (ro1 == roDiff).ShouldBeFalse();
            (ro1 != roDiff).ShouldBeTrue();

            // Act & Assert - Object overload
            ro1.Equals((object)ro2).ShouldBeTrue();
            ro1.Equals((object)roDiff).ShouldBeFalse();
            ro1.Equals((object?)null).ShouldBeFalse();
            ro1.Equals(new object()).ShouldBeFalse();
        }
    }
}
