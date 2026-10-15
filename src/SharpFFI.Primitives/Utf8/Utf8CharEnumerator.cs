using System.Runtime.CompilerServices;
using SharpFFI.Primitives.Extensions;

namespace SharpFFI.Primitives.Utf8;

/// <summary>
/// An enumerator that traverses a UTF-8 byte span, decoding code points into UTF-16 characters (<see cref="char"/>).
/// Handles multi-byte sequences and expands supplementary code points into surrogate pairs.
/// </summary>
/// <param name="view">The UTF-8 byte span to enumerate.</param>
public ref struct Utf8CharEnumerator(ReadOnlySpan<byte> view)
{
    private ReadOnlySpan<byte> _remaining = view;

    private char _current;
    private char _pendingLowSurrogate;
    private bool _hasPending;

    /// <summary>
    /// Gets the <see cref="char"/> at the current position of the enumerator.
    /// </summary>
    public char Current
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => _current;
    }

    /// <summary>
    /// Advances the enumerator to the next <see cref="char"/> in the sequence.
    /// </summary>
    /// <returns><see langword="true"/> if the enumerator successfully advanced; <see langword="false"/> if the end of the sequence has been reached.</returns>
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