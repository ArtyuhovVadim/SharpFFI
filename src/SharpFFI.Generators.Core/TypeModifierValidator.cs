using System.Diagnostics.CodeAnalysis;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace SharpFFI.Generators.Core;

public static class TypeModifierValidator
{
    public static readonly TypeModifierRule Handle = TypeModifierRule.For(HandleAttributeData.AttributeFullName)
        .Require(SyntaxKind.PartialKeyword);

    public static readonly TypeModifierRule FixedString = TypeModifierRule.For(FixedStringAttributeData.AttributeFullName)
        .Require(SyntaxKind.PartialKeyword)
        .Forbid(SyntaxKind.RefKeyword);

    public static readonly TypeModifierRule FixedArray = TypeModifierRule.For(FixedArrayAttributeData.AttributeFullName)
        .Require(SyntaxKind.PartialKeyword)
        .Forbid(SyntaxKind.ReadOnlyKeyword);

    public static readonly TypeModifierRule FixedList = TypeModifierRule.For(FixedListAttributeData.AttributeFullName)
        .Require(SyntaxKind.PartialKeyword)
        .Forbid(SyntaxKind.ReadOnlyKeyword);

    public static readonly TypeModifierRule[] DefaultRules =
    [
        Handle,
        FixedString,
        FixedArray,
        FixedList
    ];

    public static bool IsValid(TypeDeclarationSyntax syntax, TypeModifierRule rule)
    {
        foreach (var required in rule.RequiredModifiers)
        {
            if (!syntax.Modifiers.Any(required))
                return false;
        }

        foreach (var modifier in syntax.Modifiers)
        {
            var kind = modifier.Kind();
            if (rule.ForbiddenModifiers.Contains(kind) || (rule.AllowedModifiers is { } allowed && !allowed.Contains(kind)))
                return false;
        }

        return true;
    }

    public static bool Validate(
        TypeDeclarationSyntax syntax,
        INamedTypeSymbol typeSymbol,
        INamedTypeSymbol attributeSymbol,
        TypeModifierRule rule,
        [NotNullWhen(false)] out Diagnostic? error)
    {
        error = null;

        foreach (var modifier in syntax.Modifiers)
        {
            var kind = modifier.Kind();

            if (rule.ForbiddenModifiers.Contains(kind) || (rule.AllowedModifiers is { } allowed && !allowed.Contains(kind)))
            {
                error = Diagnostic.Create(
                    DiagnosticDescriptors.ForbiddenModifierError,
                    modifier.GetLocation(),
                    typeSymbol.Name,
                    attributeSymbol.Name,
                    modifier.ValueText);
                return false;
            }
        }

        foreach (var required in rule.RequiredModifiers)
        {
            if (!syntax.Modifiers.Any(required))
            {
                error = Diagnostic.Create(
                    DiagnosticDescriptors.RequiredModifierMissingError,
                    syntax.Identifier.GetLocation(),
                    typeSymbol.Name,
                    attributeSymbol.Name,
                    SyntaxFacts.GetText(required));
                return false;
            }
        }

        return true;
    }
}
