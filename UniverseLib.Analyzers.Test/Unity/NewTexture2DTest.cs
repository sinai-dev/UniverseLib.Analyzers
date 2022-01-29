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
    public class NewTexture2DTest : AnalyzerTestFixture
    {
        protected override string LanguageName => LanguageNames.CSharp;
        protected override IReadOnlyCollection<MetadataReference> References => Utils.UnityRefs;
        protected override DiagnosticAnalyzer CreateAnalyzer() => new NewTexture2DAnalyzer();

        [Test]
        public void Texture2DConstructedDirectly()
        {
            const string code = @"
using UnityEngine;

class C
{
    Texture2D tex;

	void Main()
    {
        [|tex = new Texture2D(1024, 1024, UnityEngine.TextureFormat.ARGB32, false, false, IntPtr.Zero);|]
    }
}";

            HasDiagnostic(code, NewTexture2DAnalyzer.ID);
        }
    }
}
