using System.Collections.Generic;
using System.IO;
using Microsoft.CodeAnalysis;

namespace RoslynMacros.Common.Interfaces
{
    public interface IDataEngine
    {
        Compilation Compilation { get; }
        Dictionary<string, ITypeData> AllTypeData { get; }
        SemanticModel SemanticModel(SyntaxTree st);
        ITypeData GetRecordForSymbol(string s);
        SyntaxTree[] ProyectSyntaxTrees { get; }
        IEnumerable<SyntaxTree> GetSyntaxTrees(string[] files);
        IMacro<T> GetMacro<T>(string fe, DirectoryInfo alternativepath, out string[] error) where T : IVariables;
    }
}