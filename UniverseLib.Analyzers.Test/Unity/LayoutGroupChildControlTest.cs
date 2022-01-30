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
    public class LayoutGroupChildControlTest : AnalyzerTestFixture
    {
        protected override string LanguageName => LanguageNames.CSharp;
        protected override IReadOnlyCollection<MetadataReference> References => Utils.UnityRefs;
        protected override DiagnosticAnalyzer CreateAnalyzer() => new LayoutGroupChildControlAnalyzer();

        [Test]
        public void LayoutGroupChildControlAccessedDirectly()
        {
            const string code = @"
class C
{
	void Main()
    {
        var x = new UnityEngine.UI.VerticalLayoutGroup();
        [|x.childControlWidth = false;|]
        [|x.childControlHeight = false;|]
    }
}
";

            HasDiagnostic(code, LayoutGroupChildControlAnalyzer.ID);
        }

        [Test]
        public void NoDiagnostic()
        {
            const string code = @"
public static class Il2CppExtensions
{
    public static void SetChildControlHeight(this HorizontalOrVerticalLayoutGroup group, bool value) => group.childControlHeight = value;
    public static void SetChildControlWidth(this HorizontalOrVerticalLayoutGroup group, bool value) => group.childControlWidth = value;
}
";
            NoDiagnostic(code, LayoutGroupChildControlAnalyzer.ID);
        }
    }
}
