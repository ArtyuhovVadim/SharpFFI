using System.Text;
using SharpFFI.Primitives.Utf16;
using SharpFFI.Primitives.Utf8;

namespace SharpFFI.Primitives.Tests;

public unsafe class CrossTypeCStringTests
{
    [Theory]
    [InlineData("")]
    [InlineData("a")]
    [InlineData("Hello, Мир! 🚀")]
    public void CrossType_Utf8_EqualityAndHashCode_AreConsistentAcrossRepresentations(string text)
    {
        // Arrange
        using var native8 = NativeUtf8CString.FromString(text);

        Span<byte> stackBuf8 = stackalloc byte[256];
        var scoped8 = Utf8ScopedCString.FromString(text, stackBuf8);

        var rawUtf8 = Encoding.UTF8.GetBytes(text);
        var nullTermUtf8 = new byte[rawUtf8.Length + 1];
        rawUtf8.CopyTo(nullTermUtf8, 0);
        nullTermUtf8[rawUtf8.Length] = 0;

        // Act & Assert
        fixed (byte* p8 = nullTermUtf8)
        {
            var cstr8 = Utf8CString.FromPointer(p8);
            var roCstr8 = ReadOnlyUtf8CString.FromPointer(p8);

            // Hash code consistency across types
            var expectedHash8 = native8.GetHashCode();
            cstr8.GetHashCode().ShouldBe(expectedHash8);
            roCstr8.GetHashCode().ShouldBe(expectedHash8);

            // View equality across all 4 types
            var nativeView8 = native8.AsReadOnlyView();
            var scopedView8 = scoped8.AsReadOnlyView();
            var cstrView8 = cstr8.AsReadOnlyView();
            var roCstrView8 = roCstr8.AsReadOnlyView();

            nativeView8.Equals(scopedView8).ShouldBeTrue();
            nativeView8.Equals(cstrView8).ShouldBeTrue();
            nativeView8.Equals(roCstrView8).ShouldBeTrue();
            scopedView8.Equals(cstrView8).ShouldBeTrue();
            scopedView8.Equals(roCstrView8).ShouldBeTrue();
            cstrView8.Equals(roCstrView8).ShouldBeTrue();
        }

        scoped8.Dispose();
    }

    [Theory]
    [InlineData("")]
    [InlineData("a")]
    [InlineData("Hello, Мир! 🚀")]
    public void CrossType_Utf16_EqualityAndHashCode_AreConsistentAcrossRepresentations(string text)
    {
        // Arrange
        using var native16 = NativeUtf16CString.FromString(text);

        Span<char> stackBuf16 = stackalloc char[256];
        var scoped16 = Utf16ScopedCString.FromString(text, stackBuf16);

        var nullTermUtf16 = new char[text.Length + 1];
        text.CopyTo(0, nullTermUtf16, 0, text.Length);
        nullTermUtf16[text.Length] = '\0';

        // Act & Assert
        fixed (char* p16 = nullTermUtf16)
        {
            var cstr16 = Utf16CString.FromPointer(p16);
            var roCstr16 = ReadOnlyUtf16CString.FromPointer(p16);

            // Hash code consistency across types
            var expectedHash16 = native16.GetHashCode();
            cstr16.GetHashCode().ShouldBe(expectedHash16);
            roCstr16.GetHashCode().ShouldBe(expectedHash16);

            // View equality across all 4 types
            var nativeView16 = native16.AsReadOnlyView();
            var scopedView16 = scoped16.AsReadOnlyView();
            var cstrView16 = cstr16.AsReadOnlyView();
            var roCstrView16 = roCstr16.AsReadOnlyView();

            nativeView16.Equals(scopedView16).ShouldBeTrue();
            nativeView16.Equals(cstrView16).ShouldBeTrue();
            nativeView16.Equals(roCstrView16).ShouldBeTrue();
            scopedView16.Equals(cstrView16).ShouldBeTrue();
            scopedView16.Equals(roCstrView16).ShouldBeTrue();
            cstrView16.Equals(roCstrView16).ShouldBeTrue();
        }

        scoped16.Dispose();
    }
}
