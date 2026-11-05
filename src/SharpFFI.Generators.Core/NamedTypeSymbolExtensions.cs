using System.Text;
using Microsoft.CodeAnalysis;

namespace SharpFFI.Generators.Core;

/// <summary>
/// Extension methods for <see cref="INamedTypeSymbol"/> and <see cref="ISymbol"/> to simplify code generation.
/// </summary>
public static class NamedTypeSymbolExtensions
{
    private static readonly SymbolDisplayFormat TypeDeclarationFormat = new(
        genericsOptions: SymbolDisplayGenericsOptions.IncludeTypeParameters,
        typeQualificationStyle: SymbolDisplayTypeQualificationStyle.NameOnly);

    private static readonly HashSet<char> InvalidFileNameChars = new(Path.GetInvalidFileNameChars());

    /// <summary>
    /// Gets the namespace containing the symbol, or an empty string if declared in the global namespace.
    /// </summary>
    public static string GetNamespace(this ISymbol symbol)
    {
        var ns = symbol.ContainingNamespace;
        return ns is null || ns.IsGlobalNamespace ? string.Empty : ns.ToDisplayString();
    }

    /// <summary>
    /// Returns <see langword="true"/> if the symbol is declared in the global namespace.
    /// </summary>
    public static bool IsInGlobalNamespace(this ISymbol symbol) =>
        symbol.ContainingNamespace is null || symbol.ContainingNamespace.IsGlobalNamespace;

    /// <summary>
    /// Gets the raw unadorned name of the symbol (e.g. "FixedList8"), useful for constructor declarations.
    /// </summary>
    public static string GetRawName(this INamedTypeSymbol symbol) => symbol.Name;

    /// <summary>
    /// Gets the type name formatted for declarations and references, including generic type parameter names.
    /// For example: "FixedList8&lt;T&gt;" or "MyHandle".
    /// </summary>
    public static string GetTypeDeclarationName(this INamedTypeSymbol symbol)
    {
        if (!symbol.IsGenericType || symbol.TypeParameters.IsEmpty)
            return symbol.Name;

        return symbol.ToDisplayString(TypeDeclarationFormat);
    }

    /// <summary>
    /// Gets the generic type parameters clause (e.g. "&lt;T&gt;" or "&lt;T1, T2&gt;"), or an empty string if not generic.
    /// </summary>
    public static string GetTypeParametersString(this INamedTypeSymbol symbol)
    {
        if (!symbol.IsGenericType || symbol.TypeParameters.IsEmpty)
            return string.Empty;

        return $"<{string.Join(", ", symbol.TypeParameters.Select(static p => p.Name))}>";
    }

    /// <summary>
    /// Gets the names of all generic type parameters declared on the type.
    /// </summary>
    public static IReadOnlyList<string> GetTypeParameterNames(this INamedTypeSymbol symbol)
    {
        if (!symbol.IsGenericType || symbol.TypeParameters.IsEmpty)
            return Array.Empty<string>();

        return symbol.TypeParameters.Select(static p => p.Name).ToArray();
    }

    /// <summary>
    /// Gets the fully qualified name with the <c>global::</c> prefix (e.g. "global::SharpFFI.Demo.FixedList8&lt;T&gt;").
    /// </summary>
    public static string GetFullyQualifiedName(this ITypeSymbol symbol) =>
        symbol.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat);

    /// <summary>
    /// Gets the minimally qualified name for the type.
    /// </summary>
    public static string GetMinimallyQualifiedName(this ITypeSymbol symbol) =>
        symbol.ToDisplayString(SymbolDisplayFormat.MinimallyQualifiedFormat);

    /// <summary>
    /// Gets the C# type kind keyword (e.g. "struct", "class", "record struct", "record class", "interface").
    /// </summary>
    public static string GetTypeKindKeyword(this INamedTypeSymbol symbol)
    {
        if (symbol.IsRecord)
            return symbol.IsValueType ? "record struct" : "record class";
        if (symbol.IsValueType)
            return "struct";
        if (symbol.TypeKind == TypeKind.Interface)
            return "interface";
        return "class";
    }

    /// <summary>
    /// Gets whether the type is nested inside another type.
    /// </summary>
    public static bool IsNested(this INamedTypeSymbol symbol) => symbol.ContainingType is not null;

    /// <summary>
    /// Gets all containing types from outermost to innermost. Returns an empty list if not nested.
    /// </summary>
    public static IReadOnlyList<INamedTypeSymbol> GetContainingTypes(this INamedTypeSymbol symbol)
    {
        var current = symbol.ContainingType;
        if (current is null)
            return Array.Empty<INamedTypeSymbol>();

        var list = new List<INamedTypeSymbol>();
        while (current is not null)
        {
            list.Add(current);
            current = current.ContainingType;
        }

        list.Reverse();
        return list;
    }

    /// <summary>
    /// Generates generic constraint clauses (e.g., "where T : unmanaged, new()").
    /// Returns an empty string if there are no constraints.
    /// </summary>
    public static string GetTypeConstraints(this INamedTypeSymbol symbol)
    {
        if (!symbol.IsGenericType || symbol.TypeParameters.IsEmpty)
            return string.Empty;

        var clauses = new List<string>();
        foreach (var parameter in symbol.TypeParameters)
        {
            var parts = new List<string>();

            if (parameter.HasUnmanagedTypeConstraint)
                parts.Add("unmanaged");
            else if (parameter.HasValueTypeConstraint)
                parts.Add("struct");
            else if (parameter.HasReferenceTypeConstraint)
                parts.Add("class");
            else if (parameter.HasNotNullConstraint)
                parts.Add("notnull");

            foreach (var constraintType in parameter.ConstraintTypes)
            {
                parts.Add(constraintType.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat));
            }

            if (parameter.HasConstructorConstraint)
                parts.Add("new()");

            if (parts.Count > 0)
                clauses.Add($"where {parameter.Name} : {string.Join(", ", parts)}");
        }

        return clauses.Count == 0 ? string.Empty : string.Join(" ", clauses);
    }

    /// <summary>
    /// Generates a unique, collision-free, and filesystem-safe hint name for Source Generator <c>AddSource</c>.
    /// <para>Format: <c>[Namespace.][ContainingTypes.][TypeName{T}][.suffix].g.cs</c></para>
    /// <para>Generic angle brackets '&lt;' and '&gt;' are safely replaced with '{' and '}'.</para>
    /// </summary>
    /// <param name="symbol">The named type symbol.</param>
    /// <param name="suffix">Optional generator suffix (e.g. "FixedList" or "Handle").</param>
    /// <param name="extension">File extension, defaults to ".g.cs".</param>
    public static string GetHintName(this INamedTypeSymbol symbol, string? suffix = null, string extension = ".g.cs")
    {
        var sb = new StringBuilder();

        var ns = symbol.GetNamespace();
        if (!string.IsNullOrEmpty(ns))
        {
            sb.Append(ns).Append('.');
        }

        var containingTypes = symbol.GetContainingTypes();
        foreach (var containing in containingTypes)
        {
            sb.Append(FormatTypeForFileName(containing)).Append('.');
        }

        sb.Append(FormatTypeForFileName(symbol));

        if (!string.IsNullOrEmpty(suffix))
        {
            if (!suffix!.StartsWith("."))
                sb.Append('.');
            sb.Append(suffix);
        }

        if (!string.IsNullOrEmpty(extension))
        {
            if (!extension.StartsWith("."))
                sb.Append('.');
            sb.Append(extension);
        }

        return SanitizeFileName(sb.ToString());
    }

    private static string FormatTypeForFileName(INamedTypeSymbol symbol)
    {
        if (!symbol.IsGenericType || symbol.TypeParameters.IsEmpty)
            return symbol.Name;

        return $"{symbol.Name}{{{string.Join(",", symbol.TypeParameters.Select(static p => p.Name))}}}";
    }

    private static string SanitizeFileName(string fileName)
    {
        var sb = new StringBuilder(fileName.Length);
        foreach (var c in fileName)
        {
            sb.Append(InvalidFileNameChars.Contains(c) ? '_' : c);
        }
        return sb.ToString();
    }
}
