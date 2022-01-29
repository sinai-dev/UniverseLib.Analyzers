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
    public class InputFieldOnEndEditTest : AnalyzerTestFixture
    {
        protected override string LanguageName => LanguageNames.CSharp;
        protected override IReadOnlyCollection<MetadataReference> References => Utils.UnityRefs;
        protected override DiagnosticAnalyzer CreateAnalyzer() => new InputFieldOnEndEditAnalyzer();

        [Test]
        public void InputFieldOnEndEditAccessedDirectly()
        {
            const string code = @"
class C
{
	void Main()
    {
        var x = new UnityEngine.UI.InputField();
        [|x.onEndEdit += null;|]
    }
}";

            HasDiagnostic(code, InputFieldOnEndEditAnalyzer.ID);
        }
    }
}
