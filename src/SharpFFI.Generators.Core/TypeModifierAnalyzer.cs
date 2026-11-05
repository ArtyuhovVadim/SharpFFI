using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace SharpFFI.Generators.Core;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
public sealed class TypeModifierAnalyzer : DiagnosticAnalyzer
{
    private readonly TypeModifierRule[] _rules;

    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get; }

    public TypeModifierAnalyzer() : this(TypeModifierValidator.DefaultRules) { }

    public TypeModifierAnalyzer(IEnumerable<TypeModifierRule> rules)
    {
        _rules = [.. rules];
        SupportedDiagnostics = _rules.SelectMany(static x => x.GetSupportedDiagnostics()).Distinct().ToImmutableArray();
    }

    public override void Initialize(AnalysisContext context)
    {
        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
        context.EnableConcurrentExecution();

        context.RegisterCompilationStartAction(compilationContext =>
        {
            var rulesBySymbol = new Dictionary<INamedTypeSymbol, TypeModifierRule>(SymbolEqualityComparer.Default);

            foreach (var rule in _rules)
            {
                if (compilationContext.Compilation.GetTypeByMetadataName(rule.AttributeFullName) is { } attributeSymbol)
                    rulesBySymbol[attributeSymbol] = rule;
            }

            if (rulesBySymbol.Count == 0)
                return;

            compilationContext.RegisterSymbolAction(symbolContext => Analyze(symbolContext, rulesBySymbol), SymbolKind.NamedType);
        });
    }

    private static void Analyze(SymbolAnalysisContext context, Dictionary<INamedTypeSymbol, TypeModifierRule> rulesBySymbol)
    {
        var symbol = (INamedTypeSymbol)context.Symbol;

        if (symbol.DeclaringSyntaxReferences.FirstOrDefault()?.GetSyntax(context.CancellationToken) is not TypeDeclarationSyntax syntax)
            return;

        foreach (var attribute in symbol.GetAttributes())
        {
            if (attribute.AttributeClass is { } attributeClass && rulesBySymbol.TryGetValue(attributeClass, out var rule))
            {
                if (!TypeModifierValidator.Validate(syntax, symbol, attributeClass, rule, out var diagnostic))
                {
                    context.ReportDiagnostic(diagnostic);
                }
            }
        }
    }
}
