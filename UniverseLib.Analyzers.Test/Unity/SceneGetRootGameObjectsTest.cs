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
    public class SceneGetRootGameObjectsTest : AnalyzerTestFixture
    {
        protected override string LanguageName => LanguageNames.CSharp;
        protected override IReadOnlyCollection<MetadataReference> References => Utils.UnityRefs;
        protected override DiagnosticAnalyzer CreateAnalyzer() => new SceneGetRootGameObjectsAnalyzer();

        [Test]
        public void InputClassUsed()
        {
            const string code = @"
class C
{
	void Main()
    {
        var scene = new UnityEngine.SceneManagement.Scene();
        [|scene.GetRootGameObjects();|]        
    }
}";

            HasDiagnostic(code, SceneGetRootGameObjectsAnalyzer.ID);
        }
    }
}
