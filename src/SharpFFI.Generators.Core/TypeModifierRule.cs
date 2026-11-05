using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace SharpFFI.Generators.Core;

public sealed class TypeModifierRule(string attributeFullName)
{
    public string AttributeFullName { get; } = attributeFullName;

    public HashSet<SyntaxKind> RequiredModifiers { get; } = [];

    public HashSet<SyntaxKind> ForbiddenModifiers { get; } = [];

    public HashSet<SyntaxKind>? AllowedModifiers { get; private set; }

    public static TypeModifierRule For(string attributeFullName) => new(attributeFullName);

    public TypeModifierRule Require(params SyntaxKind[] modifiers)
    {
        foreach (var modifier in modifiers)
            RequiredModifiers.Add(modifier);

        return this;
    }

    public TypeModifierRule Forbid(params SyntaxKind[] modifiers)
    {
        foreach (var modifier in modifiers)
            ForbiddenModifiers.Add(modifier);

        return this;
    }

    public TypeModifierRule Allow(params SyntaxKind[] modifiers)
    {
        AllowedModifiers ??= [];
        foreach (var modifier in modifiers)
            AllowedModifiers.Add(modifier);

        return this;
    }

    public bool IsValid(TypeDeclarationSyntax syntax) => TypeModifierValidator.IsValid(syntax, this);

    public IEnumerable<DiagnosticDescriptor> GetSupportedDiagnostics()
    {
        if (RequiredModifiers.Count > 0)
            yield return DiagnosticDescriptors.RequiredModifierMissingError;

        if (ForbiddenModifiers.Count > 0 || AllowedModifiers is not null)
            yield return DiagnosticDescriptors.ForbiddenModifierError;
    }
}
