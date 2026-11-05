using Microsoft.CodeAnalysis;

namespace SharpFFI.Generators.Core;

public static class DiagnosticDescriptors
{
    public static readonly DiagnosticDescriptor InvalidNameError = new(
        id: "FFI0001",
        title: "Invalid FixedString struct name",
        messageFormat: "The name of struct '{0}' decorated with [FixedString] must match the pattern 'Utf8FixedCString<size>' or 'ReadOnlyUtf8FixedCString<size>' (e.g., Utf8FixedCString16, ReadOnlyUtf8FixedCString67)",
        category: "Design",
        defaultSeverity: DiagnosticSeverity.Error,
        isEnabledByDefault: true);

    public static readonly DiagnosticDescriptor ForbiddenModifierError = new(
        id: "FFI0002",
        title: "Modifier is not allowed",
        messageFormat: "Type '{0}' decorated with [{1}] cannot be declared with the '{2}' modifier",
        category: "Design",
        defaultSeverity: DiagnosticSeverity.Error,
        isEnabledByDefault: true);

    public static readonly DiagnosticDescriptor RequiredModifierMissingError = new(
        id: "FFI0003",
        title: "Required modifier is missing",
        messageFormat: "Type '{0}' decorated with [{1}] must be declared with the '{2}' modifier",
        category: "Design",
        defaultSeverity: DiagnosticSeverity.Error,
        isEnabledByDefault: true);

    public static readonly DiagnosticDescriptor EnclosingTypeMustBePartialError = new(
        id: "FFI0004",
        title: "Enclosing type must be partial",
        messageFormat: "Type '{0}' containing '{1}' must be declared with the 'partial' keyword",
        category: "Design",
        defaultSeverity: DiagnosticSeverity.Error,
        isEnabledByDefault: true);
}
