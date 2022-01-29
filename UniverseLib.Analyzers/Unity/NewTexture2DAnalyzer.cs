using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using UnityEngine.UI;

namespace UniverseLib.Analyzers.Unity
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class NewTexture2DAnalyzer : DiagnosticAnalyzer
    {
        public const string ID = "ULib009";

        private static readonly DiagnosticDescriptor Rule = new DiagnosticDescriptor(
            ID,
            "Texture2D constructor should be called via UniverseLib.Runtime.TextureHelper.NewTexture2D",
            "Texture2D constructor should be called via UniverseLib.Runtime.TextureHelper.NewTexture2D",
            "UniverseLib.Unity",
            DiagnosticSeverity.Warning,
            true);

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(Rule);

        public override void Initialize(AnalysisContext context)
        {
            context.EnableConcurrentExecution();
            context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
            context.RegisterSyntaxNodeAction(ObjectCreationUsage, SyntaxKind.ObjectCreationExpression);
        }

        private void ObjectCreationUsage(SyntaxNodeAnalysisContext context)
        {
            AnalyzeObjectCreation(context.Node as ObjectCreationExpressionSyntax, context);
        }

        private void AnalyzeObjectCreation(ObjectCreationExpressionSyntax creation, SyntaxNodeAnalysisContext context)
        {
            if (context.ContainingSymbol?.ContainingType is ITypeSymbol contextType)
            {
                if (contextType.Name.Contains("TextureHelper")
                    && contextType.FullNamespace().StartsWith("UniverseLib.Runtime"))
                    return;
            }

            var typeSymbol = context.SemanticModel.GetSymbolInfo(creation.Type, context.CancellationToken).Symbol;

            if (typeSymbol.Name == "Texture2D"
                && typeSymbol.NamespaceEquals("UnityEngine"))
                context.ReportDiagnostic(Diagnostic.Create(Rule, context.Node.GetLocation()));
        }
    }
}