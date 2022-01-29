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
    public class InputClassUsageAnalyzer : DiagnosticAnalyzer
    {
        public const string ID = "ULib004";

        private static readonly DiagnosticDescriptor Rule = new DiagnosticDescriptor(
            ID,
            "Input API should be handled with UniverseLib.Input.InputManager",
            "Input API should be handled with UniverseLib.Input.InputManager",
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
                if (contextType.NamespaceEquals("UniverseLib.Input"))
                    return;
            }

            try
            {
                if (symbol.ContainingType != null && symbol.ContainingType.Name == "Input" && symbol.ContainingType.NamespaceEquals("UnityEngine"))
                    context.ReportDiagnostic(Diagnostic.Create(Rule, memberAccess.Name.GetLocation()));

                if (symbol.ContainingType != null && symbol.ContainingType.NamespaceEquals("UnityEngine.InputSystem"))
                    context.ReportDiagnostic(Diagnostic.Create(Rule, memberAccess.Name.GetLocation()));
            }
            catch { }
        }
    }
}