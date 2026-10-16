using SharpFFI.Primitives.Utf8;

namespace SharpFFI.Primitives.Tests;

public unsafe class ReadOnlyUtf8StringViewTests
{
    [Fact]
    public void Creation_FromSpanVsFromPointer_NullByteBehavior()
    {
        // Arrange
        var arr8 = "a\0b\0"u8.ToArray();

        // Act & Assert
        fixed (byte* p8 = arr8)
        {
            var roFromSpan = ReadOnlyUtf8StringView.FromUtf8Span(arr8.AsSpan(0, 3));
            roFromSpan.Length.ShouldBe(3);
            roFromSpan[1].ShouldBe((byte)0);

            var roFromPtrLen = ReadOnlyUtf8StringView.FromPointerLength(p8, 3);
            roFromPtrLen.Length.ShouldBe(3);
            roFromPtrLen[1].ShouldBe((byte)0);

            var roFromPtr = ReadOnlyUtf8StringView.FromPointer(p8);
            roFromPtr.Length.ShouldBe(1);
            roFromPtr[0].ShouldBe((byte)'a');

            var roFromCStr = ReadOnlyUtf8StringView.FromUtf8CString(ReadOnlyUtf8CString.FromPointer(p8));
            roFromCStr.Length.ShouldBe(1);
        }
    }

    [Fact]
    public void FromPointer_NullPointer_ReturnsEmptyView()
    {
        // Arrange & Act
        var roViewNull = ReadOnlyUtf8StringView.FromPointer((byte*)null);
        var roViewNint = ReadOnlyUtf8StringView.FromPointer((nint)0);
        var roViewCStr = ReadOnlyUtf8StringView.FromUtf8CString(ReadOnlyUtf8CString.Null);

        // Assert
        roViewNull.IsEmpty.ShouldBeTrue();
        roViewNull.Length.ShouldBe(0);

        roViewNint.IsEmpty.ShouldBeTrue();
        roViewNint.Length.ShouldBe(0);

        roViewCStr.IsEmpty.ShouldBeTrue();
        roViewCStr.Length.ShouldBe(0);
    }

    [Fact]
    public void CopyAndTryCopy_ToDestinationView_RespectsBounds()
    {
        // Arrange
        byte[] src = [1, 2, 3];
        var roSrc = ReadOnlyUtf8StringView.FromUtf8Span(src);

        // Act & Assert - Destination of equal size
        var dstEqual = new byte[3];
        roSrc.TryCopyTo(Utf8StringView.FromUtf8Span(dstEqual)).ShouldBeTrue();
        dstEqual.ShouldBe(src);

        // Act & Assert - Smaller destination
        var dstSmaller = new byte[2];
        roSrc.TryCopyTo(Utf8StringView.FromUtf8Span(dstSmaller)).ShouldBeFalse();
        Should.Throw<ArgumentException>(() => ReadOnlyUtf8StringView.FromUtf8Span(src).CopyTo(Utf8StringView.FromUtf8Span(dstSmaller)));
    }

    [Fact]
    public void Slice_ValidAndInvalidRanges_BehavesLikeSpan()
    {
        // Arrange
        var arr = "hello"u8.ToArray();
        var roView = ReadOnlyUtf8StringView.FromUtf8Span(arr);

        // Act & Assert - Valid slices
        roView.Slice(1).Length.ShouldBe(4);
        roView.Slice(1, 2).ToString().ShouldBe("el");
        roView.Slice(5, 0).IsEmpty.ShouldBeTrue();

        // Act & Assert - Invalid slices
        Should.Throw<ArgumentOutOfRangeException>(() => ReadOnlyUtf8StringView.FromUtf8Span(arr).Slice(-1));
        Should.Throw<ArgumentOutOfRangeException>(() => ReadOnlyUtf8StringView.FromUtf8Span(arr).Slice(6));
        Should.Throw<ArgumentOutOfRangeException>(() => ReadOnlyUtf8StringView.FromUtf8Span(arr).Slice(2, 4));
    }

    [Fact]
    public void ToArray_CreatesIndependentCopy()
    {
        // Arrange
        byte[] orig = [1, 2, 3];
        var roView = ReadOnlyUtf8StringView.FromUtf8Span(orig);

        // Act
        var copy = roView.ToArray();
        copy[0] = 99;

        // Assert
        roView[0].ShouldBe((byte)1);
    }

    [Fact]
    public void ImplicitConversions_BetweenViewsAndSpans_WorkSeamlessly()
    {
        // Arrange
        ReadOnlySpan<byte> roSpan = [1, 2, 3, 4];

        // Act
        ReadOnlyUtf8StringView roView = roSpan;
        ReadOnlySpan<byte> roundtripRoSpan = roView;

        // Assert
        roView.Length.ShouldBe(4);
        roView[0].ShouldBe((byte)1);
        roundtripRoSpan.Length.ShouldBe(4);
    }

    [Fact]
    public void EmptyProperty_ReturnsDefaultEmptyView()
    {
        // Arrange & Act
        var empty = ReadOnlyUtf8StringView.Empty;

        // Assert
        empty.IsEmpty.ShouldBeTrue();
        empty.Length.ShouldBe(0);
    }

    [Fact]
    public void FromPointerLength_Nint_CreatesExpectedView()
    {
        // Arrange
        var arr = "abcdef"u8.ToArray();

        // Act & Assert
        fixed (byte* p = arr)
        {
            var view = ReadOnlyUtf8StringView.FromPointerLength((nint)p, 4);
            view.Length.ShouldBe(4);
            view.ToString().ShouldBe("abcd");
        }
    }

    [Fact]
    public void FromUtf8CString_MutableCString_CreatesExpectedView()
    {
        // Arrange
        var arr = "hello\0"u8.ToArray();

        // Act & Assert
        fixed (byte* p = arr)
        {
            var mutableCStr = Utf8CString.FromPointer(p);
            var view = ReadOnlyUtf8StringView.FromUtf8CString(mutableCStr);

            view.Length.ShouldBe(5);
            view.ToString().ShouldBe("hello");
        }
    }

    [Fact]
    public void Equals_AndOperators_CompareContent()
    {
        // Arrange
        var arr1 = "test"u8.ToArray();
        var arr2 = "test"u8.ToArray();
        var arrDiff = "diff"u8.ToArray();

        var v1 = ReadOnlyUtf8StringView.FromUtf8Span(arr1);
        var v2 = ReadOnlyUtf8StringView.FromUtf8Span(arr2);
        var vDiff = ReadOnlyUtf8StringView.FromUtf8Span(arrDiff);

        // Act
        var eqSameSpan = v1.Equals(v1);
        var eqDiffSpan = v1.Equals(v2);
        var eqDiffContent = v1.Equals(vDiff);

        var opEqSame = v1 == v2;
        var opEqDiff = v1 == vDiff;
        var opNeqSame = v1 != v2;
        var opNeqDiff = v1 != vDiff;

        // Assert
        eqSameSpan.ShouldBeTrue();
        eqDiffSpan.ShouldBeTrue();
        eqDiffContent.ShouldBeFalse();

        opEqSame.ShouldBeTrue();
        opEqDiff.ShouldBeFalse();
        opNeqSame.ShouldBeFalse();
        opNeqDiff.ShouldBeTrue();
    }

    [Fact]
    public void BoxedMethods_EqualsObjectAndGetHashCode_ThrowNotSupported()
    {
        // Arrange
        var view = ReadOnlyUtf8StringView.Empty;

        // Act & Assert
        var eqThrew = false;
        try
        {
            view.Equals((object?)null);
        }
        catch (NotSupportedException)
        {
            eqThrew = true;
        }
        eqThrew.ShouldBeTrue();

        var hashThrew = false;
        try
        {
            view.GetHashCode();
        }
        catch (NotSupportedException)
        {
            hashThrew = true;
        }
        hashThrew.ShouldBeTrue();
    }
}
