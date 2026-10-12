using System.Runtime.InteropServices;
using SharpFFI.Annotations;
using SharpFFI.Primitives.Utf16;
using SharpFFI.Primitives.Utf8;

namespace SharpFFI.Test;

[StructLayout(LayoutKind.Sequential)]
public struct Aboba
{
    public int hahaha;
    public Utf8FixedCString67 name;

    [FixedString]
    public partial struct Utf8FixedCString67;
}

[FixedString]
public partial struct ReadOnlyUtf8FixedCString228;

[Handle]
public readonly partial struct GlWindow;

public static class Program
{
    public static unsafe void Main(string[] args)
    {
        fixed (char* ptr = "Hello 🌍🌍🌍 Привет")
        {
            var view = new Utf16StringView(ptr);
            Console.WriteLine(view.ToString());
            Console.WriteLine(view.Length);

            foreach (var ch in view)
                Console.WriteLine(ch);
        }


        fixed (byte* ptr = &"Hello"u8[0])
        {
            var view = new Utf8StringView(ptr);
            Console.WriteLine(view.ToString());
            Console.WriteLine(view.Length);

            foreach (var ch in view)
                Console.WriteLine(ch);
        }

        Console.WriteLine();

        fixed (byte* ptr = &"Привет 🌍"u8[0])
        {
            var view = new Utf8StringView(ptr);
            Console.WriteLine(view.ToString());
            Console.WriteLine(view.Length);

            foreach (var ch in view)
                Console.WriteLine(ch);
        }

        Console.WriteLine();

        {
            var path = new Utf8CString(Marshal.StringToCoTaskMemUTF8("path/67/файла/из/win/api/func"));
            var view = path.AsView();
            var slice = view[0..4];
            slice.Fill((byte)'*');

            Console.WriteLine(path);
        }

        Console.WriteLine();
    }
}