using Microsoft.CodeAnalysis;
using RoslynTestKit;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UniverseLib.Analyzers.Test
{
    public static class Utils
    {
        public static IReadOnlyCollection<MetadataReference> UnityRefs { get; } = new ReadOnlyCollection<MetadataReference>(
            new[]
            {
                ReferenceSource.FromType(typeof(UnityEngine.GameObject)),
                ReferenceSource.FromType(typeof(UnityEngine.UI.Button))
            });
    }
}
