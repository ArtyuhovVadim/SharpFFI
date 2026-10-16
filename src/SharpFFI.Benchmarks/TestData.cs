using System.Text;

namespace SharpFFI.Benchmarks;

public enum Charset
{
    Ascii,
    Cyrillic,
    MixedUnicode,
    EmojiHeavy
}

public static class TestData
{
    private const string AsciiAlphabet = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
    private const string CyrillicAlphabet = "абвгдеёжзийклмнопрстуфхцчшщъыьэюяАБВГДЕЁЖЗИЙКЛМНОПРСТУФХЦЧШЩЪЫЬЭЮЯ";

    private static readonly string[] Emojis =
    [
        "\uD83D\uDE80", // 🚀
        "\uD83D\uDD25", // 🔥
        "\uD83C\uDF89", // 🎉
        "\uD83D\uDE00", // 😀
        "\uD83D\uDCBB", // 💻
        "\uD83D\uDCA5"  // 💥
    ];

    public static string GetString(int length, Charset charset)
    {
        if (length <= 0) return string.Empty;

        var sb = new StringBuilder(length);

        switch (charset)
        {
            case Charset.Ascii:
                for (int i = 0; i < length; i++)
                {
                    sb.Append(AsciiAlphabet[i % AsciiAlphabet.Length]);
                }
                break;

            case Charset.Cyrillic:
                for (int i = 0; i < length; i++)
                {
                    sb.Append(CyrillicAlphabet[i % CyrillicAlphabet.Length]);
                }
                break;

            case Charset.MixedUnicode:
                int index = 0;
                while (sb.Length < length)
                {
                    int remaining = length - sb.Length;
                    int pattern = index % 4;

                    if (pattern == 0)
                    {
                        sb.Append(AsciiAlphabet[index % AsciiAlphabet.Length]);
                    }
                    else if (pattern == 1)
                    {
                        sb.Append(CyrillicAlphabet[index % CyrillicAlphabet.Length]);
                    }
                    else if (pattern == 2)
                    {
                        sb.Append((char)(0x4E00 + (index % 100))); // CJK ideograph (3 UTF-8 bytes)
                    }
                    else
                    {
                        if (remaining >= 2)
                        {
                            sb.Append(Emojis[index % Emojis.Length]); // Emoji (2 UTF-16 chars, 4 UTF-8 bytes)
                        }
                        else
                        {
                            sb.Append('A');
                        }
                    }

                    index++;
                }
                break;

            case Charset.EmojiHeavy:
                int emojiIdx = 0;
                while (sb.Length < length)
                {
                    int remaining = length - sb.Length;
                    if (remaining >= 2)
                    {
                        sb.Append(Emojis[emojiIdx % Emojis.Length]);
                        emojiIdx++;
                    }
                    else
                    {
                        sb.Append('!');
                    }
                }
                break;
        }

        return sb.ToString();
    }

    public static byte[] GetUtf8Bytes(int length, Charset charset)
    {
        var str = GetString(length, charset);
        return Encoding.UTF8.GetBytes(str);
    }
}
