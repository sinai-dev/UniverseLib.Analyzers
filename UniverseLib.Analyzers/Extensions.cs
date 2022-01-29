using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.Text;

namespace UniverseLib.Analyzers
{
    public static class Extensions
    {
        public static string FullNamespace(this ISymbol symbol)
        {
            var sb = new StringBuilder();

            var ns = symbol.ContainingNamespace;
            while (ns != null && !string.IsNullOrEmpty(ns.Name))
            {
                sb.Insert(0, ns.Name);
                if (ns.ContainingNamespace != null && !string.IsNullOrEmpty(ns.ContainingNamespace.Name))
                    sb.Insert(0, ".");
                ns = ns.ContainingNamespace;
            }

            return sb.ToString();
        }

        public static bool NamespaceEquals(this ISymbol symbol, string _namespace)
            => symbol.FullNamespace() == _namespace;

        public enum PropertyUsage
        {
            Get,
            Set,
            GetAndSet
        }

        // https://github.com/dotnet/roslyn/issues/15527
        // Allow to figure how the property was used,
        // Through its getter, or setter, or both.
        //
        // ++/--    pre/postfix increments            get/set
        // =        lhs of simple assignments         set
        // +=, -=   lhs of other assigments           get/set
        // x.y      rhs, of compound member access    recurr up
        //
        // any other use is just a get
        //
        // TOOD: ref parameters?
        //
        public static PropertyUsage GetPropertyUsage(this SyntaxNode node)
        {
            var kind = node.Parent.Kind();

            if (kind == SyntaxKind.PostIncrementExpression ||
                kind == SyntaxKind.PostDecrementExpression ||
                kind == SyntaxKind.PreIncrementExpression ||
                kind == SyntaxKind.PreDecrementExpression)
            {
                return PropertyUsage.GetAndSet;
            }
            else if (node.Parent is AssignmentExpressionSyntax)
            {
                var assignment = (AssignmentExpressionSyntax)(node.Parent);

                if (assignment.Left == node)
                {
                    return kind == SyntaxKind.SimpleAssignmentExpression
                        ? PropertyUsage.Set
                        : PropertyUsage.GetAndSet;
                }
            }
            else if (node.Parent is MemberAccessExpressionSyntax)
            {
                var m = (MemberAccessExpressionSyntax)(node.Parent);

                if (m.Name == node)
                {
                    return node.Parent.GetPropertyUsage();
                }
            }

            return PropertyUsage.Get;
        }
    }
}