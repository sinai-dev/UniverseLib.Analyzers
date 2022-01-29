using System;
using System.Linq;
using System.Collections.Generic;
using RoslynTestKit;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis;
using System.Collections.ObjectModel;
using UniverseLib.Analyzers.Unity;
using NUnit.Framework;

namespace UniverseLib.Analyzers.Test.Unity
{
    public class SelectableColorBlockAnalyzerTest : AnalyzerTestFixture
    {
        protected override string LanguageName => LanguageNames.CSharp;
        protected override IReadOnlyCollection<MetadataReference> References => Utils.UnityRefs;
        protected override DiagnosticAnalyzer CreateAnalyzer() => new SelectableColorBlockAnalyzer();

        [Test]
        public void SelectableColorBlockAssignedDirectly()
        {
            const string code = @"
class C
{
	void Main()
    {
        var x = new UnityEngine.UI.Button();
        [|x.colors = new UnityEngine.UI.ColorBlock();|]
    }
}";

            HasDiagnostic(code, SelectableColorBlockAnalyzer.ID);
        }

        [Test]
        public void NoDiagnostic()
        {
            const string code = @"
class C
{
	void Main()
    {
        var x = new UnityEngine.UI.Button();
        var colors = x.colors;
";

            NoDiagnostic(code, SelectableColorBlockAnalyzer.ID);
        }
    }
}
