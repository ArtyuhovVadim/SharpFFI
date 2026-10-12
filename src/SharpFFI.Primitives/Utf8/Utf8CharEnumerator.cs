using System.Runtime.CompilerServices;
using System.Text;

namespace SharpFFI.Primitives.Utf8;

public ref struct Utf8CharEnumerator(ReadOnlySpan<byte> view)
{
    private ReadOnlySpan<byte> _remaining = view;

    private char _current;
    private char _pendingLowSurrogate;
    private bool _hasPending;

    public char Current
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => _current;
    }

    public bool MoveNext()
    {
        if (_hasPending)
        {
            _current = _pendingLowSurrogate;
            _hasPending = false;
            return true;
        }

        if (_remaining.IsEmpty)
            return false;

        Rune.DecodeFromUtf8(_remaining, out var rune, out var bytesConsumed);
        _remaining = _remaining[bytesConsumed..];

        if (rune.IsBmp)
        {
            _current = (char)rune.Value;
        }
        else
        {
            Span<char> buffer = stackalloc char[2];
            rune.EncodeToUtf16(buffer);

            _current = buffer[0];
            _pendingLowSurrogate = buffer[1];
            _hasPending = true;
        }

        return true;
    }
}