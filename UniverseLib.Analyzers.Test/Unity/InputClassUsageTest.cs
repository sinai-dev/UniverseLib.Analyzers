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
    public class InputClassUsageTest : AnalyzerTestFixture
    {
        protected override string LanguageName => LanguageNames.CSharp;
        protected override IReadOnlyCollection<MetadataReference> References => Utils.UnityRefs;
        protected override DiagnosticAnalyzer CreateAnalyzer() => new InputClassUsageAnalyzer();

        [Test]
        public void InputClassUsed()
        {
            const string code = @"
namespace UnityEngine.InputSystem
{
    public static class TestClass
    {
        public static bool TestProperty => false;
    }
}

class C
{
	void Main()
    {
        [|UnityEngine.Input.GetKeyDown(UnityEngine.KeyCode.F5);|]
        [|UnityEngine.InputSystem.TestClass.TestProperty;|]
        var y = UnityEngine.KeyCode.F5;
    }
}";

            HasDiagnostic(code, InputClassUsageAnalyzer.ID);
        }
    }
}
