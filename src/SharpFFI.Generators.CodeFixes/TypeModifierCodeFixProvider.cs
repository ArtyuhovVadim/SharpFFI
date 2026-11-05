using System.Collections.Immutable;
using System.Composition;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using SharpFFI.Generators.Core;

namespace SharpFFI.Generators.CodeFixes;

[Shared]
[ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(TypeModifierCodeFixProvider))]
public class TypeModifierCodeFixProvider : CodeFixProvider
{
    public override ImmutableArray<string> FixableDiagnosticIds { get; } = ImmutableArray.Create(
        DiagnosticDescriptors.ForbiddenModifierError.Id,
        DiagnosticDescriptors.RequiredModifierMissingError.Id,
        DiagnosticDescriptors.EnclosingTypeMustBePartialError.Id);

    public override FixAllProvider GetFixAllProvider() => WellKnownFixAllProviders.BatchFixer;

    public override async Task RegisterCodeFixesAsync(CodeFixContext context)
    {
        var root = await context.Document.GetSyntaxRootAsync(context.CancellationToken).ConfigureAwait(false);
        var diagnostic = context.Diagnostics[0];
        var token = root?.FindToken(diagnostic.Location.SourceSpan.Start);
        var typeDeclaration = token?.Parent?.AncestorsAndSelf().OfType<TypeDeclarationSyntax>().FirstOrDefault();

        if (typeDeclaration is null || token is not { } modifierOrIdentifierToken)
            return;

        if (diagnostic.Id == DiagnosticDescriptors.ForbiddenModifierError.Id)
        {
            var title = $"Remove '{modifierOrIdentifierToken.ValueText}' modifier";
            context.RegisterCodeFix(
                CodeAction.Create(title, ct => RemoveModifierAsync(context.Document, typeDeclaration, modifierOrIdentifierToken, ct), title),
                diagnostic);
        }
        else
        {
            const string title = "Add 'partial' modifier";
            context.RegisterCodeFix(
                CodeAction.Create(title, ct => AddPartialModifierAsync(context.Document, typeDeclaration, ct), title),
                diagnostic);
        }
    }

    private static async Task<Document> RemoveModifierAsync(Document document, TypeDeclarationSyntax typeDeclaration, SyntaxToken modifierToken, CancellationToken cancellationToken)
    {
        var newModifiers = typeDeclaration.Modifiers.Remove(modifierToken);
        var newTypeDecl = typeDeclaration.WithModifiers(newModifiers);

        var root = await document.GetSyntaxRootAsync(cancellationToken).ConfigureAwait(false);
        var newRoot = root!.ReplaceNode(typeDeclaration, newTypeDecl);
        return document.WithSyntaxRoot(newRoot);
    }

    private static async Task<Document> AddPartialModifierAsync(Document document, TypeDeclarationSyntax typeDeclaration, CancellationToken cancellationToken)
    {
        var partialToken = SyntaxFactory.Token(SyntaxKind.PartialKeyword).WithTrailingTrivia(SyntaxFactory.Space);
        var newModifiers = typeDeclaration.Modifiers.Add(partialToken);
        var newTypeDecl = typeDeclaration.WithModifiers(newModifiers);

        var root = await document.GetSyntaxRootAsync(cancellationToken).ConfigureAwait(false);
        var newRoot = root!.ReplaceNode(typeDeclaration, newTypeDecl);
        return document.WithSyntaxRoot(newRoot);
    }
}
