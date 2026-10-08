using System.Runtime.InteropServices;
using SharpFFI.Annotations;
using SharpFFI.Primitives.Utf8;

/*
 * Базовые типы:
 *   Массив строк?
 *   Обертка над динамическим массивом и просто массивом (данные + длина) или срез
 *   Поддержка NativeAAot
 *   Добавить поддержку конкретных фреймворков и использовать их фичи для улучшения производительности
 *   Посмотреть на Utf8StringMarshaller
 *   Добавить интеграцию с стандартным генератором marshaller'ов (LibraryImport)
 *   Оптимизировать как в Utf8StringMarshaller
 *
 * Генератор:
 *   Строки фиксированного размера struct Player { int id; char name[64]; }; [InlineArray]?
 *   Создать аттрибут для генерации Opaque Pointers, в c# и c/c++ опциональн+о
 *   C Union -> c# union или костыльную структуру на более ранних версиях
 *   Автоматическая генерация экспортируемых функций с поддержкой моих типов?
 *   Автоматический маппинг VTable
 *   Генерация C++ Flattening Wrapper'а на си
 *   Массивы фиксированного размера?
 *
 * С/С++:
 *   Написать хедер онли библиотеку, которая будет повторять структуры из си шарпа и содержать в себе все необходимые функции
 */

namespace SharpFFI.Test;

[Handle]
public readonly partial struct GlWindow;

public static class Program
{
    public static unsafe void Main(string[] args)
    {
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
    }
}