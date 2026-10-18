using System.Diagnostics.CodeAnalysis;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace SharpFFI.Generators.Core;

public static class PartialValidator
{
    public static bool IsValid(TypeDeclarationSyntax syntax) => syntax.Modifiers.Any(SyntaxKind.PartialKeyword);

    public static bool IsValid(TypeDeclarationSyntax syntax, INamedTypeSymbol typeSymbol, INamedTypeSymbol attributeSymbol, [NotNullWhen(false)] out Diagnostic? error)
    {
        if (IsValid(syntax))
        {
            error = null;
            return true;
        }

        error = Diagnostic.Create(DiagnosticDescriptors.MustBePartialWarning, syntax.Identifier.GetLocation(), typeSymbol.ToDisplayString(), attributeSymbol.ToDisplayString());
        return false;
    }
}
