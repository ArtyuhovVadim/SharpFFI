using SharpFFI.Primitives.Utf16;

namespace SharpFFI.Primitives.Tests;

public unsafe class Utf16StringViewTests
{
    [Fact]
    public void Creation_FromSpanVsFromPointer_NullByteBehavior()
    {
        // Arrange
        char[] arr16 = ['a', '\0', 'b', '\0'];

        // Act & Assert
        fixed (char* p16 = arr16)
        {
            var viewFromSpan16 = Utf16StringView.FromUtf16Span(arr16.AsSpan(0, 3));
            viewFromSpan16.Length.ShouldBe(3);
            viewFromSpan16[1].ShouldBe('\0');

            var viewFromPtrLen16 = Utf16StringView.FromPointerLength(p16, 3);
            viewFromPtrLen16.Length.ShouldBe(3);
            viewFromPtrLen16[1].ShouldBe('\0');

            var viewFromPtr16 = Utf16StringView.FromPointer(p16);
            viewFromPtr16.Length.ShouldBe(1);
            viewFromPtr16[0].ShouldBe('a');

            var viewFromCStr16 = Utf16StringView.FromUtf16CString(Utf16CString.FromPointer(p16));
            viewFromCStr16.Length.ShouldBe(1);
        }
    }

    [Fact]
    public void FromPointer_NullPointer_ReturnsEmptyView()
    {
        // Arrange & Act
        var viewNull = Utf16StringView.FromPointer((char*)null);
        var viewNint = Utf16StringView.FromPointer((nint)0);
        var viewCStr = Utf16StringView.FromUtf16CString(Utf16CString.Null);

        // Assert
        viewNull.IsEmpty.ShouldBeTrue();
        viewNull.Length.ShouldBe(0);

        viewNint.IsEmpty.ShouldBeTrue();
        viewNint.Length.ShouldBe(0);

        viewCStr.IsEmpty.ShouldBeTrue();
        viewCStr.Length.ShouldBe(0);
    }

    [Fact]
    public void MutatingOperations_FillClearAndRefIndexer_ModifySpan()
    {
        // Arrange
        char[] arr = ['a', 'b', 'c', 'd'];
        var view = Utf16StringView.FromUtf16Span(arr);

        // Act & Assert - Ref indexer
        view[0] = 'z';
        arr[0].ShouldBe('z');

        // Act & Assert - Fill
        view.Fill('x');
        arr.ShouldAllBe(c => c == 'x');

        // Act & Assert - Clear
        view.Clear();
        arr.ShouldAllBe(c => c == '\0');
    }

    [Fact]
    public void CopyAndTryCopy_ToDestinationView_RespectsBounds()
    {
        // Arrange
        char[] src = ['a', 'b', 'c'];
        var srcView = Utf16StringView.FromUtf16Span(src);

        // Act & Assert - Larger destination
        var dstLarger = new char[5];
        srcView.TryCopyTo(Utf16StringView.FromUtf16Span(dstLarger)).ShouldBeTrue();
        dstLarger[0].ShouldBe('a');

        // Act & Assert - Equal destination
        var dstEqual = new char[3];
        srcView.CopyTo(Utf16StringView.FromUtf16Span(dstEqual));
        dstEqual.ShouldBe(src);

        // Act & Assert - Smaller destination
        var dstSmaller = new char[2];
        srcView.TryCopyTo(Utf16StringView.FromUtf16Span(dstSmaller)).ShouldBeFalse();
        Should.Throw<ArgumentException>(() => Utf16StringView.FromUtf16Span(src).CopyTo(Utf16StringView.FromUtf16Span(dstSmaller)));
    }

    [Fact]
    public void Slice_ValidAndInvalidRanges_BehavesLikeSpan()
    {
        // Arrange
        char[] arr = ['h', 'e', 'l', 'l', 'o'];
        var view = Utf16StringView.FromUtf16Span(arr);

        // Act & Assert - Valid slices
        view.Slice(1).Length.ShouldBe(4);
        view.Slice(1, 2).ToString().ShouldBe("el");
        view.Slice(5, 0).IsEmpty.ShouldBeTrue();

        // Act & Assert - Invalid slices
        Should.Throw<ArgumentOutOfRangeException>(() => Utf16StringView.FromUtf16Span(arr).Slice(-1));
        Should.Throw<ArgumentOutOfRangeException>(() => Utf16StringView.FromUtf16Span(arr).Slice(6));
        Should.Throw<ArgumentOutOfRangeException>(() => Utf16StringView.FromUtf16Span(arr).Slice(2, 4));
    }

    [Fact]
    public void ToArray_CreatesIndependentCopy()
    {
        // Arrange
        char[] orig = ['a', 'b', 'c'];
        var view = Utf16StringView.FromUtf16Span(orig);

        // Act
        var copy = view.ToArray();
        copy[0] = 'z';

        // Assert
        view[0].ShouldBe('a');
    }

    [Fact]
    public void ImplicitConversions_BetweenViewsAndSpans_WorkSeamlessly()
    {
        // Arrange
        Span<char> span = stackalloc char[4];
        span[0] = 'k';

        // Act
        Utf16StringView view = span;
        Span<char> roundtripSpan = view;
        ReadOnlySpan<char> roSpanFromView = view;
        ReadOnlyUtf16StringView roViewFromView = view;

        // Assert
        view.Length.ShouldBe(4);
        view[0].ShouldBe('k');
        roundtripSpan.Length.ShouldBe(4);
        roundtripSpan[0].ShouldBe('k');
        roSpanFromView.Length.ShouldBe(4);
        roViewFromView.Length.ShouldBe(4);
    }

    [Fact]
    public void EmptyProperty_ReturnsDefaultEmptyView()
    {
        // Arrange & Act
        var empty = Utf16StringView.Empty;

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
            var view = Utf16StringView.FromPointerLength((nint)p, 4);
            view.Length.ShouldBe(4);
            view.ToString().ShouldBe("abcd");
        }
    }

    [Fact]
    public void GetEnumerator_IteratesCharacters()
    {
        // Arrange
        char[] arr = ['h', 'e', 'l', 'l', 'o'];
        var view = Utf16StringView.FromUtf16Span(arr);
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

        var v1 = Utf16StringView.FromUtf16Span(arr1);
        var v2 = Utf16StringView.FromUtf16Span(arr2);
        var vDiff = Utf16StringView.FromUtf16Span(arrDiff);

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
        var view = Utf16StringView.Empty;

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
