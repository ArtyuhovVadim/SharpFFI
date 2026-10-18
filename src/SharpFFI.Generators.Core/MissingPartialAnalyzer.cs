using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using System.Collections.Immutable;

namespace SharpFFI.Generators.Core;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
public class MissingPartialAnalyzer : DiagnosticAnalyzer
{
    private static readonly ImmutableArray<string> SupportedAttributes = ImmutableArray.Create(HandleAttributeData.AttributeFullName,
                                                                                               FixedStringAttributeData.AttributeFullName);

    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get; } = ImmutableArray.Create(DiagnosticDescriptors.MustBePartialWarning);

    public override void Initialize(AnalysisContext context)
    {
        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
        context.EnableConcurrentExecution();

        context.RegisterCompilationStartAction(static context =>
        {
            var attributeSymbols = SupportedAttributes
                                   .Select(context.Compilation.GetTypeByMetadataName)
                                   .Where(static x => x is not null)
                                   .ToImmutableArray();

            if (attributeSymbols.IsEmpty)
                return;

            context.RegisterSymbolAction(context => Analyze(context, attributeSymbols!), SymbolKind.NamedType);
        });
    }

    private static void Analyze(SymbolAnalysisContext context, ImmutableArray<INamedTypeSymbol> supportedAttributes)
    {
        var symbol = (INamedTypeSymbol)context.Symbol;

        if (symbol.DeclaringSyntaxReferences.FirstOrDefault()?.GetSyntax(context.CancellationToken) is not TypeDeclarationSyntax syntax)
            return;

        foreach (var attribute in symbol.GetAttributes())
        {
            if (attribute.AttributeClass is not { } attributeSymbol || !supportedAttributes.Contains(attributeSymbol, SymbolEqualityComparer.Default))
                continue;

            if (PartialValidator.IsValid(syntax, symbol, attributeSymbol, out var error))
                continue;

            context.ReportDiagnostic(error);
        }
    }
}
