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

    public static readonly DiagnosticDescriptor RefNotAllowedError = new(
        id: "FFI0002",
        title: "Ref modifier is not allowed",
        messageFormat: "Struct '{0}' with [FixedString] cannot be a ref struct. Fixed strings must be embeddable in standard structs.",
        category: "Design",
        defaultSeverity: DiagnosticSeverity.Error,
        isEnabledByDefault: true);

    public static readonly DiagnosticDescriptor MustBePartialWarning = new(
        id: "FFI0003",
        title: "Type must be partial",
        messageFormat: "Type '{0}' decorated with [{1}] must be declared with the 'partial' keyword",
        category: "Design",
        defaultSeverity: DiagnosticSeverity.Warning,
        isEnabledByDefault: true);

    public static readonly DiagnosticDescriptor EnclosingTypeMustBePartialWarning = new(
        id: "FFI0004",
        title: "Enclosing type must be partial",
        messageFormat: "Type '{0}' containing '{1}' must be declared with the 'partial' keyword",
        category: "Design",
        defaultSeverity: DiagnosticSeverity.Warning,
        isEnabledByDefault: true);
}
