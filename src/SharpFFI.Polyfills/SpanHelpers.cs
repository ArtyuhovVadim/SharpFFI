// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Buffers.Binary;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Numerics;
using System.Runtime.CompilerServices;

namespace System;

internal static partial class SpanHelpers // .Byte
{
    // IndexOfNullByte processes memory in aligned chunks, and thus it won't crash even if it accesses memory beyond the null terminator.
    // This behavior is an implementation detail of the runtime and callers outside System.Private.CoreLib must not depend on it.
    internal static unsafe int IndexOfNullByte(byte* searchSpace)
    {
        const int Length = int.MaxValue;
        const uint uValue = 0; // Use uint for comparisons to avoid unnecessary 8->32 extensions
        nuint offset = 0; // Use nuint for arithmetic to avoid unnecessary 64->32->64 truncations
        nuint lengthToExamine = (nuint)(uint)Length;

        while (lengthToExamine >= 8)
        {
            lengthToExamine -= 8;

            if (uValue == searchSpace[offset])
                goto Found;
            if (uValue == searchSpace[offset + 1])
                goto Found1;
            if (uValue == searchSpace[offset + 2])
                goto Found2;
            if (uValue == searchSpace[offset + 3])
                goto Found3;
            if (uValue == searchSpace[offset + 4])
                goto Found4;
            if (uValue == searchSpace[offset + 5])
                goto Found5;
            if (uValue == searchSpace[offset + 6])
                goto Found6;
            if (uValue == searchSpace[offset + 7])
                goto Found7;

            offset += 8;
        }

        if (lengthToExamine >= 4)
        {
            lengthToExamine -= 4;

            if (uValue == searchSpace[offset])
                goto Found;
            if (uValue == searchSpace[offset + 1])
                goto Found1;
            if (uValue == searchSpace[offset + 2])
                goto Found2;
            if (uValue == searchSpace[offset + 3])
                goto Found3;

            offset += 4;
        }

        while (lengthToExamine > 0)
        {
            lengthToExamine -= 1;

            if (uValue == searchSpace[offset])
                goto Found;

            offset += 1;
        }

        throw new ArgumentException("The string must be null-terminated.");
        Found: // Workaround for https://github.com/dotnet/runtime/issues/8795
        return (int)offset;
        Found1:
        return (int)(offset + 1);
        Found2:
        return (int)(offset + 2);
        Found3:
        return (int)(offset + 3);
        Found4:
        return (int)(offset + 4);
        Found5:
        return (int)(offset + 5);
        Found6:
        return (int)(offset + 6);
        Found7:
        return (int)(offset + 7);
    }
}