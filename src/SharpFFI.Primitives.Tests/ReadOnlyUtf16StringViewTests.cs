using SharpFFI.Primitives.Utf16;

namespace SharpFFI.Primitives.Tests;

public unsafe class ReadOnlyUtf16StringViewTests
{
    [Fact]
    public void Creation_FromSpanVsFromPointer_NullByteBehavior()
    {
        // Arrange
        char[] arr16 = ['a', '\0', 'b', '\0'];

        // Act & Assert
        fixed (char* p16 = arr16)
        {
            var roFromSpan16 = ReadOnlyUtf16StringView.FromUtf16Span(arr16.AsSpan(0, 3));
            roFromSpan16.Length.ShouldBe(3);
            roFromSpan16[1].ShouldBe('\0');

            var viewFromPtrLen16 = ReadOnlyUtf16StringView.FromPointerLength(p16, 3);
            viewFromPtrLen16.Length.ShouldBe(3);
            viewFromPtrLen16[1].ShouldBe('\0');

            var viewFromPtr16 = ReadOnlyUtf16StringView.FromPointer(p16);
            viewFromPtr16.Length.ShouldBe(1);
            viewFromPtr16[0].ShouldBe('a');

            var roFromCStr16 = ReadOnlyUtf16StringView.FromUtf16CString(ReadOnlyUtf16CString.FromPointer(p16));
            roFromCStr16.Length.ShouldBe(1);
        }
    }

    [Fact]
    public void FromPointer_NullPointer_ReturnsEmptyView()
    {
        // Arrange & Act
        var viewNull = ReadOnlyUtf16StringView.FromPointer((char*)null);
        var viewNint = ReadOnlyUtf16StringView.FromPointer((nint)0);
        var viewCStr = ReadOnlyUtf16StringView.FromUtf16CString(ReadOnlyUtf16CString.Null);

        // Assert
        viewNull.IsEmpty.ShouldBeTrue();
        viewNull.Length.ShouldBe(0);

        viewNint.IsEmpty.ShouldBeTrue();
        viewNint.Length.ShouldBe(0);

        viewCStr.IsEmpty.ShouldBeTrue();
        viewCStr.Length.ShouldBe(0);
    }

    [Fact]
    public void CopyAndTryCopy_ToDestinationView_RespectsBounds()
    {
        // Arrange
        char[] src = ['a', 'b', 'c'];
        var roSrc = ReadOnlyUtf16StringView.FromUtf16Span(src);

        // Act & Assert - Equal destination
        var dstEqual = new char[3];
        roSrc.TryCopyTo(Utf16StringView.FromUtf16Span(dstEqual)).ShouldBeTrue();
        dstEqual.ShouldBe(src);

        // Act & Assert - Smaller destination
        var dstSmaller = new char[2];
        roSrc.TryCopyTo(Utf16StringView.FromUtf16Span(dstSmaller)).ShouldBeFalse();
        Should.Throw<ArgumentException>(() => ReadOnlyUtf16StringView.FromUtf16Span(src).CopyTo(Utf16StringView.FromUtf16Span(dstSmaller)));
    }

    [Fact]
    public void Slice_ValidAndInvalidRanges_BehavesLikeSpan()
    {
        // Arrange
        char[] arr = ['h', 'e', 'l', 'l', 'o'];
        var roView = ReadOnlyUtf16StringView.FromUtf16Span(arr);

        // Act & Assert - Valid slices
        roView.Slice(1).Length.ShouldBe(4);
        roView.Slice(1, 2).ToString().ShouldBe("el");
        roView.Slice(5, 0).IsEmpty.ShouldBeTrue();

        // Act & Assert - Invalid slices
        Should.Throw<ArgumentOutOfRangeException>(() => ReadOnlyUtf16StringView.FromUtf16Span(arr).Slice(-1));
        Should.Throw<ArgumentOutOfRangeException>(() => ReadOnlyUtf16StringView.FromUtf16Span(arr).Slice(6));
        Should.Throw<ArgumentOutOfRangeException>(() => ReadOnlyUtf16StringView.FromUtf16Span(arr).Slice(2, 4));
    }

    [Fact]
    public void ToArray_CreatesIndependentCopy()
    {
        // Arrange
        char[] orig = ['a', 'b', 'c'];
        var roView = ReadOnlyUtf16StringView.FromUtf16Span(orig);

        // Act
        var copy = roView.ToArray();
        copy[0] = 'z';

        // Assert
        roView[0].ShouldBe('a');
    }

    [Fact]
    public void ImplicitConversions_BetweenViewsAndSpans_WorkSeamlessly()
    {
        // Arrange
        ReadOnlySpan<char> roSpan = ['a', 'b', 'c', 'd'];

        // Act
        ReadOnlyUtf16StringView roView = roSpan;
        ReadOnlySpan<char> roundtripRoSpan = roView;

        // Assert
        roView.Length.ShouldBe(4);
        roView[0].ShouldBe('a');
        roundtripRoSpan.Length.ShouldBe(4);
    }

    [Fact]
    public void EmptyProperty_ReturnsDefaultEmptyView()
    {
        // Arrange & Act
        var empty = ReadOnlyUtf16StringView.Empty;

        // Assert
        empty.IsEmpty.ShouldBeTrue();
        empty.Length.ShouldBe(0);
    }

    [Fact]
    public void FromPointerLength_Nint_CreatesExpectedView()
    {
        // Arrange
        char[] arr = ['a', 'b', 'c', 'd', 'e', 'f'];

        // Act & Assert
        fixed (char* p = arr)
        {
            var view = ReadOnlyUtf16StringView.FromPointerLength((nint)p, 4);
            view.Length.ShouldBe(4);
            view.ToString().ShouldBe("abcd");
        }
    }

    [Fact]
    public void FromUtf16CString_MutableCString_CreatesExpectedView()
    {
        // Arrange
        char[] arr = ['h', 'e', 'l', 'l', 'o', '\0'];

        // Act & Assert
        fixed (char* p = arr)
        {
            var mutableCStr = Utf16CString.FromPointer(p);
            var view = ReadOnlyUtf16StringView.FromUtf16CString(mutableCStr);

            view.Length.ShouldBe(5);
            view.ToString().ShouldBe("hello");
        }
    }

    [Fact]
    public void GetEnumerator_IteratesCharacters()
    {
        // Arrange
        char[] arr = ['h', 'e', 'l', 'l', 'o'];
        var view = ReadOnlyUtf16StringView.FromUtf16Span(arr);
        var chars = new List<char>();

        // Act
        foreach (var c in view)
        {
            chars.Add(c);
        }

        // Assert
        chars.ShouldBe(['h', 'e', 'l', 'l', 'o']);
    }

    [Fact]
    public void Equals_AndOperators_CompareContent()
    {
        // Arrange
        char[] arr1 = ['t', 'e', 's', 't'];
        char[] arr2 = ['t', 'e', 's', 't'];
        char[] arrDiff = ['d', 'i', 'f', 'f'];

        var v1 = ReadOnlyUtf16StringView.FromUtf16Span(arr1);
        var v2 = ReadOnlyUtf16StringView.FromUtf16Span(arr2);
        var vDiff = ReadOnlyUtf16StringView.FromUtf16Span(arrDiff);

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
        var view = ReadOnlyUtf16StringView.Empty;

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
