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
    public class LayerMaskLayerToNameTest : AnalyzerTestFixture
    {
        protected override string LanguageName => LanguageNames.CSharp;
        protected override IReadOnlyCollection<MetadataReference> References => Utils.UnityRefs;
        protected override DiagnosticAnalyzer CreateAnalyzer() => new LayerMaskLayerToNameAnalyzer();

        [Test]
        public void InputClassUsed()
        {
            const string code = @"
class C
{
	void Main()
    {
        [|UnityEngine.LayerMask.LayerToName(default);|]
    }
}";

            HasDiagnostic(code, LayerMaskLayerToNameAnalyzer.ID);
        }
    }
}
