using System.Text;
using SharpFFI.Annotations;
using SharpFFI.Primitives;
using SharpFFI.Primitives.Utf8;

namespace SharpFFI.Demo;

[Handle]
public readonly partial struct GlHandle;

public static unsafe class Program
{
    public static void Main()
    {
        Console.WriteLine("SharpFFI Primitives Demo\n");

        DemoBooleans();
        DemoScopedCStrings();
        DemoNativeCStrings();
        DemoRawCStrings();
        DemoStringViews();
        DemoUtf8Enumeration();
    }

    private static void DemoBooleans()
    {
        Console.WriteLine("1. Booleans (Bool8 and Bool32)");

        // ABI-compatible booleans for C and C++ interop
        Bool8 b8 = true;
        Console.WriteLine($"Bool8 from true: Value = {b8.Value} (size: {sizeof(Bool8)} byte)");

        // Raw non-zero values are normalized to 1
        var normalized = new Bool8((byte)42);
        Console.WriteLine($"Bool8 from raw byte 42: Value = {normalized.Value} (normalized to True)");

        Bool32 b32 = false;
        Console.WriteLine($"Bool32 from false: Value = {b32.Value} (size: {sizeof(Bool32)} bytes)\n");
    }

    private static void DemoScopedCStrings()
    {
        Console.WriteLine("2. Scoped C-Strings (Stack buffer with heap fallback)");

        // Fast path: fits in stackalloc buffer without heap allocations
        var textFit = "Hello from stack";
        Span<byte> stackBuffer = stackalloc byte[64];
        using var scopedFit = Utf8ScopedCString.FromString(textFit, stackBuffer);
        Console.WriteLine($"Source: {textFit}");
        Console.WriteLine($"Result: {scopedFit.ToString()} (fits in stack buffer, length: {scopedFit.Length})");

        // Heap fallback: exceeds buffer size, automatically allocates and frees native memory
        var textLarge = "Long string exceeding buffer size to demonstrate heap fallback";
        Span<byte> tinyBuffer = stackalloc byte[4];
        using var scopedHeap = Utf8ScopedCString.FromString(textLarge, tinyBuffer);
        Console.WriteLine($"Source: {textLarge}");
        Console.WriteLine($"Result: {scopedHeap.ToString()} (allocated on native heap, length: {scopedHeap.Length})\n");
    }

    private static void DemoNativeCStrings()
    {
        Console.WriteLine("3. Native C-Strings (Unmanaged heap with RAII)");

        // Heap-allocated null-terminated UTF-8 string for interop structures
        var source = "Hello Native UTF-8";
        using var native = NativeUtf8CString.FromString(source);
        Console.WriteLine($"Source: {source}");
        Console.WriteLine($"Native string: {native.ToString()} (length: {native.Length})");
        Console.WriteLine($"First character: {(char)native[0]}");

        // Safe conversion to unmanaged pointer wrapper
        ReadOnlyUtf8CString readOnlyPtr = native;
        Console.WriteLine($"Read-only pointer wrapper: {readOnlyPtr.ToString()}\n");
    }

    private static void DemoRawCStrings()
    {
        Console.WriteLine("4. Raw C-Strings (Pointer wrappers)");

        // Wrapping an existing null-terminated byte buffer
        byte[] buffer = [(byte)'D', (byte)'a', (byte)'t', (byte)'a', 0];
        fixed (byte* p = buffer)
        {
            var cstr = Utf8CString.FromPointer(p);
            Console.WriteLine($"Source memory: {cstr.ToString()}");

            // In-place modification through indexer
            cstr[0] = (byte)'B';
            Console.WriteLine($"After in-place modification: {cstr.ToString()}");

            ReadOnlyUtf8CString ro = cstr;
            Console.WriteLine($"Read-only view over same pointer: {ro.ToString()}\n");
        }
    }

    private static void DemoStringViews()
    {
        Console.WriteLine("5. String Views (Zero-allocation slicing and edits)");

        byte[] data = "SharpFFI high performance interop"u8.ToArray();
        var view = Utf8StringView.FromUtf8Span(data);
        Console.WriteLine($"Source: {view.ToString()}");

        // Slicing without copying
        var slice = view.Slice(9, 16);
        Console.WriteLine($"Slice range [9..25]: {slice.ToString()}");

        // In-place modification: replace 'high' with 'best'
        ReadOnlySpan<byte> replacement = "best"u8;
        replacement.CopyTo(slice.Slice(0, 4));
        Console.WriteLine($"After editing slice: {view.ToString()}\n");
    }

    private static void DemoUtf8Enumeration()
    {
        Console.WriteLine("6. UTF-8 Character Enumeration");

        // Decoding UTF-8 bytes to UTF-16 chars, including multi-byte emoji
        var sampleText = "C# 🚀 FFI";
        var bytes = Encoding.UTF8.GetBytes(sampleText);
        var roView = ReadOnlyUtf8StringView.FromUtf8Span(bytes);

        Console.WriteLine($"Source: {sampleText}");
        Console.Write("Decoded stream: ");
        foreach (char c in roView)
        {
            Console.Write(char.IsSurrogate(c) ? $"'\\u{(int)c:X4}' " : $"'{c}' ");
        }
        Console.WriteLine();
    }
}