using Microsoft.CodeAnalysis;
using SharpFFI.Annotations;

namespace SharpFFI.Generators.Core;

public record struct HandleAttributeData(DefaultHandleCtor Ctor)
{
    public const string AttributeFullName = "SharpFFI.Annotations.HandleAttribute";

    public const string AttributeShortName = "Handle";
}

public record struct FixedStringAttributeData
{
    public const string AttributeFullName = "SharpFFI.Annotations.FixedStringAttribute";

    public const string AttributeShortName = "FixedString";
}

public static class AttributeDataExtensions
{
    extension(AttributeData attribute)
    {
        public HandleAttributeData ToHandleAttribute()
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
}
