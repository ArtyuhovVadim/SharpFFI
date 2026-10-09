using System;
using System.Runtime.CompilerServices;
using System.Text;

namespace SharpFFI.Primitives.Utf16;

public ref struct Utf16CharEnumerator(ReadOnlySpan<char> view)
{
    private ReadOnlySpan<char> _remaining = view;

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

        Rune.DecodeFromUtf16(_remaining, out var rune, out var charsConsumed);
        _remaining = _remaining[charsConsumed..];

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