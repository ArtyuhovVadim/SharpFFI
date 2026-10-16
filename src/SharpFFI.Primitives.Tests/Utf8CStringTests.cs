using SharpFFI.Primitives.Utf8;

namespace SharpFFI.Primitives.Tests;

public unsafe class Utf8CStringTests
{
    [Fact]
    public void NullState_PropertiesAndConversions()
    {
        // Arrange
        Utf8CString[] nullUtf8S = [
            default,
            Utf8CString.Null,
            Utf8CString.FromPointer((byte*)null),
            Utf8CString.FromPointer((nint)0)
        ];

        // Act & Assert
        foreach (var cstr in nullUtf8S)
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
        byte zero = 0;

        // Act
        var cstr = Utf8CString.FromPointer(&zero);

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
        byte zero = 0;
        var nullCstr = Utf8CString.Null;
        var emptyCstr = Utf8CString.FromPointer(&zero);

        // Act & Assert
        nullCstr.ShouldNotBe(emptyCstr);
        nullCstr.AsReadOnlyView().Equals(emptyCstr.AsReadOnlyView()).ShouldBeTrue();
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
            var cstrA = Utf8CString.FromPointer(pA);
            var cstrB = Utf8CString.FromPointer(pB);

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
        var buf = "test\0"u8.ToArray();

        // Act & Assert
        fixed (byte* p = buf)
        {
            var cstr = Utf8CString.FromPointer(p);
            cstr[1] = (byte)'o';
            cstr.ToString().ShouldBe("tost");

            // Write null terminator at index 2
            cstr[2] = 0;
            cstr.AsView().Length.ShouldBe(2);
            cstr.ToString().ShouldBe("to");
            buf[0].ShouldBe((byte)'t');
            buf[1].ShouldBe((byte)'o');
            buf[2].ShouldBe((byte)0);
        }
    }

    [Fact]
    public void ImplicitCast_MutableToReadOnly_PreservesPointer()
    {
        // Arrange
        byte zero = 0;
        var mutable = Utf8CString.FromPointer(&zero);

        // Act
        ReadOnlyUtf8CString ro = mutable;

        // Assert
        ((nint)ro.Pointer).ShouldBe((nint)mutable.Pointer);
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
            var cstr1 = Utf8CString.FromPointer(p1);
            var cstr2 = Utf8CString.FromPointer(p2);
            var cstrDiff = Utf8CString.FromPointer(pDiff);
            var cstrNull = Utf8CString.Null;

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
