using Microsoft.CodeAnalysis;
using SharpFFI.Annotations;

namespace SharpFFI.Generators.Core;

public record struct HandleAttributeData(DefaultHandleCtor Ctor);

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
