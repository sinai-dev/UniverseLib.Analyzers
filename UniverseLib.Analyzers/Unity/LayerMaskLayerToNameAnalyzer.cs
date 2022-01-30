using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Text;
using UnityEngine.UI;

namespace UniverseLib.Analyzers.Unity
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class LayerMaskLayerToNameAnalyzer : DiagnosticAnalyzer
    {
        public const string ID = "ULib006";

        private static readonly DiagnosticDescriptor Rule = new DiagnosticDescriptor(
            ID,
            "LayerMask.LayerToName should be called via RuntimeHelper.LayerToName",
            "LayerMask.LayerToName should be called via RuntimeHelper.LayerToName",
            "UniverseLib.Unity",
            DiagnosticSeverity.Warning,
            true);

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(Rule);

        public override void Initialize(AnalysisContext context)
        {
            context.EnableConcurrentExecution();
            context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
            context.RegisterSyntaxNodeAction(AnalyzeSimpleMemberAccess, SyntaxKind.SimpleMemberAccessExpression);
        }

        private void AnalyzeSimpleMemberAccess(SyntaxNodeAnalysisContext context)
        {
            var memberAccess = (MemberAccessExpressionSyntax)context.Node;
            var symbol = context.SemanticModel.GetSymbolInfo(memberAccess.Name, context.CancellationToken).Symbol;

            if (symbol == null)
                return;

            if (context.ContainingSymbol?.ContainingType is ITypeSymbol contextType)
            {
                if (contextType.FullNamespace().StartsWith("UniverseLib.Runtime"))
                    return;
            }

            if (symbol.Name == "LayerToName"
                && symbol.ContainingType?.Name == "LayerMask" 
                && symbol.ContainingType.NamespaceEquals("UnityEngine"))
                context.ReportDiagnostic(Diagnostic.Create(Rule, memberAccess.Name.GetLocation()));
        }
    }
}