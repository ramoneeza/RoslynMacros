using Microsoft.CodeAnalysis.CSharp.Syntax;
using RoslynMacros.Common.Classes;
using RoslynMacros.Common.Interfaces;

namespace RoslynMacros.Common.Data
{
    public class MethodData : BaseData, IMethodData
    {
        public MethodDeclarationSyntax Method { get; }
        public string Parameters { get; set; }

        public MethodData(MethodDeclarationSyntax m) : base(m.Modifiers.ToString(), m.ReturnType.ToString(),
            m.Identifier.ToString(), null, m.AttributeLists)
        {
            Method = m;
            Parameters = m.ParameterList.ToString();
        }
    }
}