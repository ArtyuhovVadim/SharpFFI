using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace SharpFFI.Generators.Core;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
public sealed class FixedStringNameAnalyzer : DiagnosticAnalyzer
{
    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get; } = ImmutableArray.Create(DiagnosticDescriptors.InvalidNameError);

    public override void Initialize(AnalysisContext context)
    {
        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
        context.EnableConcurrentExecution();

        context.RegisterCompilationStartAction(compilationContext =>
        {
            if (compilationContext.Compilation.GetTypeByMetadataName(FixedStringAttributeData.AttributeFullName) is not { } attributeSymbol)
                return;

            compilationContext.RegisterSymbolAction(symbolContext => Analyze(symbolContext, attributeSymbol), SymbolKind.NamedType);
        });
    }

    private static void Analyze(SymbolAnalysisContext context, INamedTypeSymbol attributeSymbol)
    {
        var symbol = (INamedTypeSymbol)context.Symbol;

        if (symbol.DeclaringSyntaxReferences.FirstOrDefault()?.GetSyntax(context.CancellationToken) is not TypeDeclarationSyntax syntax)
            return;

        foreach (var attribute in symbol.GetAttributes())
        {
            if (SymbolEqualityComparer.Default.Equals(attribute.AttributeClass, attributeSymbol))
            {
                if (!FixedStringNameValidator.Validate(syntax, symbol, out var diagnostic))
                    context.ReportDiagnostic(diagnostic);
            }
        }
    }
}
