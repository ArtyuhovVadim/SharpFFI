using SharpFFI.Primitives.Utf8;

namespace SharpFFI.Primitives.Tests;

public unsafe class Utf8StringViewTests
{
    [Fact]
    public void Creation_FromSpanVsFromPointer_NullByteBehavior()
    {
        // Arrange
        var arr8 = "a\0b\0"u8.ToArray();

        // Act & Assert
        fixed (byte* p8 = arr8)
        {
            var viewFromSpan = Utf8StringView.FromUtf8Span(arr8.AsSpan(0, 3));
            viewFromSpan.Length.ShouldBe(3);
            viewFromSpan[1].ShouldBe((byte)0);

            var viewFromPtrLen = Utf8StringView.FromPointerLength(p8, 3);
            viewFromPtrLen.Length.ShouldBe(3);
            viewFromPtrLen[1].ShouldBe((byte)0);

            var viewFromPtr = Utf8StringView.FromPointer(p8);
            viewFromPtr.Length.ShouldBe(1);
            viewFromPtr[0].ShouldBe((byte)'a');

            var viewFromCStr = Utf8StringView.FromUtf8CString(Utf8CString.FromPointer(p8));
            viewFromCStr.Length.ShouldBe(1);
        }
    }

    [Fact]
    public void FromPointer_NullPointer_ReturnsEmptyView()
    {
        // Arrange & Act
        var viewNull = Utf8StringView.FromPointer((byte*)null);
        var viewNint = Utf8StringView.FromPointer((nint)0);
        var viewCStr = Utf8StringView.FromUtf8CString(Utf8CString.Null);

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
        byte[] arr = [1, 2, 3, 4];
        var view = Utf8StringView.FromUtf8Span(arr);

        // Act & Assert - Ref indexer
        view[0] = 42;
        arr[0].ShouldBe((byte)42);

        // Act & Assert - Fill
        view.Fill(99);
        arr.ShouldAllBe(b => b == 99);

        // Act & Assert - Clear
        view.Clear();
        arr.ShouldAllBe(b => b == 0);
    }

    [Fact]
    public void CopyAndTryCopy_ToDestinationView_RespectsBounds()
    {
        // Arrange
        byte[] src = [1, 2, 3];
        var srcView = Utf8StringView.FromUtf8Span(src);

        // Act & Assert - Larger destination
        var dstLarger = new byte[5];
        srcView.TryCopyTo(Utf8StringView.FromUtf8Span(dstLarger)).ShouldBeTrue();
        dstLarger[0].ShouldBe((byte)1);
        dstLarger[1].ShouldBe((byte)2);
        dstLarger[2].ShouldBe((byte)3);

        // Act & Assert - Equal destination
        var dstEqual = new byte[3];
        srcView.CopyTo(Utf8StringView.FromUtf8Span(dstEqual));
        dstEqual.ShouldBe(src);

        // Act & Assert - Smaller destination
        var dstSmaller = new byte[2];
        srcView.TryCopyTo(Utf8StringView.FromUtf8Span(dstSmaller)).ShouldBeFalse();
        Should.Throw<ArgumentException>(() => Utf8StringView.FromUtf8Span(src).CopyTo(Utf8StringView.FromUtf8Span(dstSmaller)));
    }

    [Fact]
    public void Slice_ValidAndInvalidRanges_BehavesLikeSpan()
    {
        // Arrange
        var arr = "hello"u8.ToArray();
        var view = Utf8StringView.FromUtf8Span(arr);

        // Act & Assert - Valid slices
        view.Slice(1).Length.ShouldBe(4);
        view.Slice(1, 2).ToString().ShouldBe("el");
        view.Slice(5, 0).IsEmpty.ShouldBeTrue();

        // Act & Assert - Invalid slices
        Should.Throw<ArgumentOutOfRangeException>(() => Utf8StringView.FromUtf8Span(arr).Slice(-1));
        Should.Throw<ArgumentOutOfRangeException>(() => Utf8StringView.FromUtf8Span(arr).Slice(6));
        Should.Throw<ArgumentOutOfRangeException>(() => Utf8StringView.FromUtf8Span(arr).Slice(2, 4));
    }

    [Fact]
    public void ToArray_CreatesIndependentCopy()
    {
        // Arrange
        byte[] orig = [1, 2, 3];
        var view = Utf8StringView.FromUtf8Span(orig);

        // Act
        var copy = view.ToArray();
        copy[0] = 99;

        // Assert
        view[0].ShouldBe((byte)1);
    }

    [Fact]
    public void ImplicitConversions_BetweenViewsAndSpans_WorkSeamlessly()
    {
        // Arrange
        Span<byte> span = stackalloc byte[4];
        span[0] = 42;

        // Act
        Utf8StringView view = span;
        Span<byte> roundtripSpan = view;
        ReadOnlySpan<byte> roSpanFromView = view;
        ReadOnlyUtf8StringView roViewFromView = view;

        // Assert
        view.Length.ShouldBe(4);
        view[0].ShouldBe((byte)42);
        roundtripSpan.Length.ShouldBe(4);
        roundtripSpan[0].ShouldBe((byte)42);
        roSpanFromView.Length.ShouldBe(4);
        roViewFromView.Length.ShouldBe(4);
    }

    [Fact]
    public void EmptyProperty_ReturnsDefaultEmptyView()
    {
        // Arrange & Act
        var empty = Utf8StringView.Empty;

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
            var view = Utf8StringView.FromPointerLength((nint)p, 4);
            view.Length.ShouldBe(4);
            view.ToString().ShouldBe("abcd");
        }
    }

    [Fact]
    public void GetEnumerator_IteratesCharacters()
    {
        // Arrange
        var arr = "hello"u8.ToArray();
        var view = Utf8StringView.FromUtf8Span(arr);
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
        var arr1 = "test"u8.ToArray();
        var arr2 = "test"u8.ToArray();
        var arrDiff = "diff"u8.ToArray();

        var v1 = Utf8StringView.FromUtf8Span(arr1);
        var v2 = Utf8StringView.FromUtf8Span(arr2);
        var vDiff = Utf8StringView.FromUtf8Span(arrDiff);

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
        var view = Utf8StringView.Empty;

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
