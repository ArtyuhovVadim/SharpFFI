using SharpFFI.Primitives.Utf16;

namespace SharpFFI.Primitives.Tests;

public unsafe class ReadOnlyUtf16CStringTests
{
    [Fact]
    public void NullState_PropertiesAndConversions()
    {
        // Arrange
        ReadOnlyUtf16CString[] nullRoUtf16s = [
            default,
            ReadOnlyUtf16CString.Null,
            ReadOnlyUtf16CString.FromPointer((char*)null),
            ReadOnlyUtf16CString.FromPointer((nint)0)
        ];

        // Act & Assert
        foreach (var cstr in nullRoUtf16s)
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
        var zero = '\0';

        // Act
        var roCstr = ReadOnlyUtf16CString.FromPointer(&zero);

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
        var zero = '\0';
        var nullRo = ReadOnlyUtf16CString.Null;
        var emptyRo = ReadOnlyUtf16CString.FromPointer(&zero);

        // Act & Assert
        nullRo.ShouldNotBe(emptyRo);
        nullRo.AsReadOnlyView().Equals(emptyRo.AsReadOnlyView()).ShouldBeTrue();
    }

    [Fact]
    public void Equality_DifferentBuffersSameNullTerminatedPrefix_ReturnsTrue()
    {
        // Arrange
        char[] bufA = ['a', 'b', '\0', 'X'];
        char[] bufB = ['a', 'b', '\0', 'Y'];

        // Act & Assert
        fixed (char* pA = bufA, pB = bufB)
        {
            var roA = ReadOnlyUtf16CString.FromPointer(pA);
            var roB = ReadOnlyUtf16CString.FromPointer(pB);

            roA.ShouldBe(roB);
            roA.GetHashCode().ShouldBe(roB.GetHashCode());
            roA.ToString().ShouldBe("ab");
            roB.ToString().ShouldBe("ab");
        }
    }

    [Fact]
    public void Indexer_ReturnsCharAtOffset()
    {
        // Arrange
        char[] buf = ['a', 'b', 'c', '\0'];

        // Act & Assert
        fixed (char* p = buf)
        {
            var ro = ReadOnlyUtf16CString.FromPointer(p);
            ro[0].ShouldBe('a');
            ro[1].ShouldBe('b');
            ro[2].ShouldBe('c');
            ro[3].ShouldBe('\0');
        }
    }

    [Fact]
    public void Equals_AndOperators_CoverAllBranches()
    {
        // Arrange
        char[] buf1 = ['h', 'e', 'l', 'l', 'o', '\0'];
        char[] buf2 = ['h', 'e', 'l', 'l', 'o', '\0'];
        char[] bufDiff = ['w', 'o', 'r', 'l', 'd', '\0'];

        // Act & Assert
        fixed (char* p1 = buf1, p2 = buf2, pDiff = bufDiff)
        {
            var ro1 = ReadOnlyUtf16CString.FromPointer(p1);
            var ro2 = ReadOnlyUtf16CString.FromPointer(p2);
            var roDiff = ReadOnlyUtf16CString.FromPointer(pDiff);
            var roNull = ReadOnlyUtf16CString.Null;

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
