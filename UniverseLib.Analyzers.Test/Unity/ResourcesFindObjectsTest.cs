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
    public class ResourcesFindObjectsTest : AnalyzerTestFixture
    {
        protected override string LanguageName => LanguageNames.CSharp;
        protected override IReadOnlyCollection<MetadataReference> References => Utils.UnityRefs;
        protected override DiagnosticAnalyzer CreateAnalyzer() => new ResourcesFindObjectsAnalyzer();

        [Test]
        public void InputClassUsed()
        {
            const string code = @"
class C
{
	void Main()
    {
        [|UnityEngine.Resources.FindObjectsOfTypeAll<UnityEngine.GameObject>();|]
        [|UnityEngine.Resources.FindObjectsOfTypeAll(typeof(UnityEngine.GameObject));|]
    }
}";

            HasDiagnostic(code, ResourcesFindObjectsAnalyzer.ID);
        }
    }
}
