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
    public class LayoutGroupChildControlAnalyzer : DiagnosticAnalyzer
    {
        public const string ID = "ULib003";

        private static readonly DiagnosticDescriptor Rule = new DiagnosticDescriptor(
            ID,
            "HorizontalOrVerticalLayoutGroup.childControlWidth/Height should be set with SetChildControlWidth/Height",
            "HorizontalOrVerticalLayoutGroup.childControlWidth/Height should be set with SetChildControlWidth/Height",
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
                if (contextType.NamespaceEquals("UniverseLib.Runtime.Il2Cpp"))
                    return;
            }

            if ((symbol.Name == "childControlWidth" || symbol.Name == "childControlHeight")
                && IsHorizontalOrVerticalLayoutGroup(symbol.ContainingType))
                context.ReportDiagnostic(Diagnostic.Create(Rule, memberAccess.Name.GetLocation()));
        }

        private static bool IsHorizontalOrVerticalLayoutGroup(ITypeSymbol typeSymbol)
        {
            if (typeSymbol.Name == "HorizontalOrVerticalLayoutGroup" && typeSymbol.NamespaceEquals("UnityEngine.UI"))
                return true;

            if (typeSymbol.BaseType != null)
                return IsHorizontalOrVerticalLayoutGroup(typeSymbol.BaseType);

            return false;
        }
    }
}
