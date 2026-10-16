using SharpFFI.Primitives.Utf16;

namespace SharpFFI.Primitives.Tests;

public unsafe class Utf16CStringTests
{
    [Fact]
    public void NullState_PropertiesAndConversions()
    {
        // Arrange
        Utf16CString[] nullUtf16S = [
            default,
            Utf16CString.Null,
            Utf16CString.FromPointer((char*)null),
            Utf16CString.FromPointer((nint)0)
        ];

        // Act & Assert
        foreach (var cstr in nullUtf16S)
        {
            cstr.IsNull.ShouldBeTrue();
            cstr.IsEmpty.ShouldBeTrue();
            ((nint)cstr.Pointer).ShouldBe(0);
            cstr.ToString().ShouldBeEmpty();
            cstr.GetHashCode().ShouldBe(0);
            cstr.AsReadOnlyView().IsEmpty.ShouldBeTrue();
            cstr.AsView().IsEmpty.ShouldBeTrue();
        }
    }

    [Fact]
    public void EmptyNonNullState_DistinguishesIsNullAndIsEmpty()
    {
        // Arrange
        var zero = '\0';

        // Act
        var cstr = Utf16CString.FromPointer(&zero);

        // Assert
        cstr.IsNull.ShouldBeFalse();
        cstr.IsEmpty.ShouldBeTrue();
        cstr.AsReadOnlyView().Length.ShouldBe(0);
        cstr.ToString().ShouldBeEmpty();
    }

    [Fact]
    public void Equality_NullVsEmptyNonNull_ReturnsFalse()
    {
        // Arrange
        var zero = '\0';
        var nullCstr = Utf16CString.Null;
        var emptyCstr = Utf16CString.FromPointer(&zero);

        // Act & Assert
        nullCstr.ShouldNotBe(emptyCstr);
        nullCstr.AsReadOnlyView().Equals(emptyCstr.AsReadOnlyView()).ShouldBeTrue();
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
            var cstrA = Utf16CString.FromPointer(pA);
            var cstrB = Utf16CString.FromPointer(pB);

            cstrA.ShouldBe(cstrB);
            cstrA.GetHashCode().ShouldBe(cstrB.GetHashCode());
            cstrA.ToString().ShouldBe("ab");
            cstrB.ToString().ShouldBe("ab");
        }
    }

    [Fact]
    public void MutableCString_RefIndexer_ModifiesUnderlyingMemory()
    {
        // Arrange
        char[] buf = ['t', 'e', 's', 't', '\0'];

        // Act & Assert
        fixed (char* p = buf)
        {
            var cstr = Utf16CString.FromPointer(p);
            cstr[1] = 'o';
            cstr.ToString().ShouldBe("tost");

            // Write null terminator at index 2
            cstr[2] = '\0';
            cstr.AsView().Length.ShouldBe(2);
            cstr.ToString().ShouldBe("to");
            buf[0].ShouldBe('t');
            buf[1].ShouldBe('o');
            buf[2].ShouldBe('\0');
        }
    }

    [Fact]
    public void ImplicitCast_MutableToReadOnly_PreservesPointer()
    {
        // Arrange
        var zero = '\0';
        var mutable = Utf16CString.FromPointer(&zero);

        // Act
        ReadOnlyUtf16CString ro = mutable;

        // Assert
        ((nint)ro.Pointer).ShouldBe((nint)mutable.Pointer);
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
            var cstr1 = Utf16CString.FromPointer(p1);
            var cstr2 = Utf16CString.FromPointer(p2);
            var cstrDiff = Utf16CString.FromPointer(pDiff);
            var cstrNull = Utf16CString.Null;

            // Act & Assert - Same pointer
            cstr1.Equals(cstr1).ShouldBeTrue();
            (cstr1 == cstr1).ShouldBeTrue();
            (cstr1 != cstr1).ShouldBeFalse();

            // Act & Assert - Both null
            cstrNull.Equals(cstrNull).ShouldBeTrue();
            (cstrNull == cstrNull).ShouldBeTrue();
            (cstrNull != cstrNull).ShouldBeFalse();

            // Act & Assert - One null, one non-null
            cstrNull.Equals(cstr1).ShouldBeFalse();
            cstr1.Equals(cstrNull).ShouldBeFalse();
            (cstrNull == cstr1).ShouldBeFalse();
            (cstr1 == cstrNull).ShouldBeFalse();
            (cstrNull != cstr1).ShouldBeTrue();
            (cstr1 != cstrNull).ShouldBeTrue();

            // Act & Assert - Different pointers same content
            cstr1.Equals(cstr2).ShouldBeTrue();
            (cstr1 == cstr2).ShouldBeTrue();
            (cstr1 != cstr2).ShouldBeFalse();

            // Act & Assert - Different content
            cstr1.Equals(cstrDiff).ShouldBeFalse();
            (cstr1 == cstrDiff).ShouldBeFalse();
            (cstr1 != cstrDiff).ShouldBeTrue();

            // Act & Assert - Object overload
            cstr1.Equals((object)cstr2).ShouldBeTrue();
            cstr1.Equals((object)cstrDiff).ShouldBeFalse();
            cstr1.Equals((object?)null).ShouldBeFalse();
            cstr1.Equals(new object()).ShouldBeFalse();
        }
    }
}
