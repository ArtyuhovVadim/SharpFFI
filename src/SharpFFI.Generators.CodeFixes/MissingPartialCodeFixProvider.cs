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
[ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(MissingPartialCodeFixProvider))]
public class MissingPartialCodeFixProvider : CodeFixProvider
{
    public const string Name = "Add 'partial' modifier";

    public override ImmutableArray<string> FixableDiagnosticIds { get; } = ImmutableArray.Create(DiagnosticDescriptors.MustBePartialWarning.Id);

    public override FixAllProvider GetFixAllProvider() => WellKnownFixAllProviders.BatchFixer;

    public override async Task RegisterCodeFixesAsync(CodeFixContext context)
    {
        var root = await context.Document.GetSyntaxRootAsync(context.CancellationToken).ConfigureAwait(false);
        var diagnostic = context.Diagnostics[0];
        var diagnosticSpan = diagnostic.Location.SourceSpan;
        var typeDeclaration = root?.FindToken(diagnosticSpan.Start).Parent?.AncestorsAndSelf().OfType<TypeDeclarationSyntax>().FirstOrDefault();

        if (typeDeclaration is null)
            return;

        context.RegisterCodeFix(CodeAction.Create(Name, token => AddPartialModifierAsync(context.Document, typeDeclaration, token), nameof(MissingPartialCodeFixProvider)), diagnostic);
    }

    private static async Task<Document> AddPartialModifierAsync(Document document, TypeDeclarationSyntax typeDeclaration, CancellationToken token)
    {
        var newModifiers = typeDeclaration.Modifiers.Add(SyntaxFactory.Token(SyntaxKind.PartialKeyword));
        var newTypeDecl = typeDeclaration.WithModifiers(newModifiers);

        var root = await document.GetSyntaxRootAsync(token).ConfigureAwait(false);
        var newRoot = root!.ReplaceNode(typeDeclaration, newTypeDecl);
        return document.WithSyntaxRoot(newRoot);
    }
}