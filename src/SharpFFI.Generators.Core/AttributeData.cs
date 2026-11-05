using Microsoft.CodeAnalysis;
using SharpFFI.Annotations;

namespace SharpFFI.Generators.Core;

public record struct HandleAttributeData(DefaultHandleCtor Ctor)
{
    public const string AttributeFullName = "SharpFFI.Annotations.HandleAttribute";

    public const string AttributeShortName = "Handle";

    public static HandleAttributeData FromAttributeData(AttributeData attribute)
    {
        var result = new HandleAttributeData(DefaultHandleCtor.None);

        foreach (var pair in attribute.NamedArguments)
        {
            if (pair is { Key: "Ctor", Value.Value: int intValue })
            {
                result.Ctor = (DefaultHandleCtor)intValue;
            }
        }

        return result;
    }
}

public record struct FixedStringAttributeData
{
    public const string AttributeFullName = "SharpFFI.Annotations.FixedStringAttribute";

    public const string AttributeShortName = "FixedString";
}

public record struct FixedArrayAttributeData(int Length)
{
    public const string AttributeFullName = "SharpFFI.Annotations.FixedArrayAttribute";

    public const string AttributeShortName = "FixedArray";

    public static FixedArrayAttributeData FromAttributeData(AttributeData attribute) =>
        new((int?)attribute.ConstructorArguments[0].Value ?? throw new InvalidOperationException("Failed to retrieve FixedArray length."));
}

public record struct FixedListAttributeData(int Capacity)
{
    public const string AttributeFullName = "SharpFFI.Annotations.FixedListAttribute";

    public const string AttributeShortName = "FixedList";

    public static FixedListAttributeData FromAttributeData(AttributeData attribute) =>
        new((int?)attribute.ConstructorArguments[0].Value ?? throw new InvalidOperationException("Failed to retrieve FixedList length."));
}