using System.Diagnostics.CodeAnalysis;
using System.Text.RegularExpressions;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace SharpFFI.Generators.Core;

public static class FixedStringNameValidator
{
    private static readonly Regex Pattern = new(@"^(ReadOnly)?Utf8FixedCString(?<size>\d+)$", RegexOptions.Compiled);

    public static bool IsValid(string name) => TryParse(name, out _, out _);

    public static bool IsValid(TypeDeclarationSyntax syntax) => IsValid(syntax.Identifier.ValueText);

    public static bool TryParse(string name, out int size, out bool isReadOnly)
    {
        var match = Pattern.Match(name);
        if (match.Success && int.TryParse(match.Groups["size"].Value, out size) && size > 0)
        {
            isReadOnly = match.Groups[1].Success;
            return true;
        }

        size = 0;
        isReadOnly = false;
        return false;
    }

    public static bool Validate(TypeDeclarationSyntax syntax, INamedTypeSymbol symbol, [NotNullWhen(false)] out Diagnostic? error)
    {
        if (IsValid(syntax))
        {
            error = null;
            return true;
        }

        error = Diagnostic.Create(DiagnosticDescriptors.InvalidNameError, syntax.Identifier.GetLocation(), symbol.Name);
        return false;
    }
}
