using System.Runtime.CompilerServices;
using SharpFFI.Primitives.Extensions;

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

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
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

        UnicodeUtility.DecodeScalarFromUtf8(_remaining, out var scalarValue, out var bytesConsumed);
        _remaining = _remaining.Slice(bytesConsumed);

        if (UnicodeUtility.IsBmpCodePoint(scalarValue))
        {
            _current = (char)scalarValue;
        }
        else
        {
            UnicodeUtility.GetUtf16SurrogatesFromSupplementaryPlaneScalar(scalarValue, out _current, out _pendingLowSurrogate);
            _hasPending = true;
        }

        return true;
    }
}