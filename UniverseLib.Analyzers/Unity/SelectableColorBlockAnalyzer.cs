using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Text;

namespace UniverseLib.Analyzers.Unity
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class SelectableColorBlockAnalyzer : DiagnosticAnalyzer
    {
        public const string ID = "ULib001";

        private static readonly DiagnosticDescriptor Rule = new DiagnosticDescriptor(
            ID,
            "Selectable.colors should be set with RuntimeHelper.SetColorBlock",
            "Selectable.colors is set directly, it should be set with RuntimeHelper.SetColorBlock",
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
                if (contextType.NamespaceEquals("UniverseLib.Runtime.Mono"))
                    // We are in the actual RuntimeHelper class and it's mono, ignore the diagnostic.
                    return;
            }

            if (symbol is IPropertySymbol propSymbol)
            {
                var propertyUsage = context.Node.GetPropertyUsage();
                if (propertyUsage == Extensions.PropertyUsage.Set || propertyUsage == Extensions.PropertyUsage.GetAndSet)
                {
                    if (propSymbol.Type.Name == "ColorBlock" && IsSelectable(symbol.ContainingType))
                        context.ReportDiagnostic(Diagnostic.Create(Rule, memberAccess.Name.GetLocation()));
                }
            }
        }

        private bool IsSelectable(ITypeSymbol type)
        {
            if (type.Name == "Selectable" && type.NamespaceEquals("UnityEngine.UI"))
                return true;

            if (type.BaseType != null)
                return IsSelectable(type.BaseType);

            return false;
        }
    }
}