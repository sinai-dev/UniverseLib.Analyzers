using System;
using System.Linq;
using System.Collections.Generic;
using RoslynTestKit;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis;
using System.Collections.ObjectModel;
using UniverseLib.Analyzers.Reflection;
using NUnit.Framework;

namespace UniverseLib.Analyzers.Test.Reflection
{
    public class AssemblyGetTypesTest : AnalyzerTestFixture
    {
        protected override string LanguageName => LanguageNames.CSharp;
        protected override IReadOnlyCollection<MetadataReference> References => Utils.UnityRefs;
        protected override DiagnosticAnalyzer CreateAnalyzer() => new AssemblyGetTypesAnalyzer();

        [Test]
        public void AssemblyGetTypesUsed()
        {
            const string code = @"
class C
{
	void Main()
    {
        [|typeof(C).Assembly.GetTypes();|]
    }
}";

            HasDiagnostic(code, AssemblyGetTypesAnalyzer.ID);
        }

        [Test]
        public void NoDiagnostic()
        {
            const string code = @"
using System;
using System.Reflection;

namespace UniverseLib
{
    public static class ReflectionExtensions
    {
        public static IEnumerable<Type> TryGetTypes(this Assembly asm)
        {
            return asm.GetTypes();
        }
    }
}";
            NoDiagnostic(code, AssemblyGetTypesAnalyzer.ID);
        }
    }
}
