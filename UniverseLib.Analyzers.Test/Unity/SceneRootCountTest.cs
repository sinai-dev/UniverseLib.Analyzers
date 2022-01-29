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
    public class SceneRootCountTest : AnalyzerTestFixture
    {
        protected override string LanguageName => LanguageNames.CSharp;
        protected override IReadOnlyCollection<MetadataReference> References => Utils.UnityRefs;
        protected override DiagnosticAnalyzer CreateAnalyzer() => new SceneRootCountAnalyzer();

        [Test]
        public void InputClassUsed()
        {
            const string code = @"
class C
{
	void Main()
    {
        var scene = new UnityEngine.SceneManagement.Scene();
        [|var y = scene.rootCount;|]        
    }
}";

            HasDiagnostic(code, SceneRootCountAnalyzer.ID);
        }
    }
}
