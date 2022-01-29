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
    public class SpriteCreateTest : AnalyzerTestFixture
    {
        protected override string LanguageName => LanguageNames.CSharp;
        protected override IReadOnlyCollection<MetadataReference> References => Utils.UnityRefs;
        protected override DiagnosticAnalyzer CreateAnalyzer() => new SpriteCreateAnalyzer();

        [Test]
        public void InputClassUsed()
        {
            const string code = @"
using UnityEngine;

class C
{
	void Main()
    {
        var sprite = [|Sprite.Create(null, default, default);|]       
    }
}";

            HasDiagnostic(code, SpriteCreateAnalyzer.ID);
        }
    }
}
