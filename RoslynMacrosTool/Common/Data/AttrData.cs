using Microsoft.CodeAnalysis.CSharp.Syntax;
using RoslynMacros.Common.Interfaces;

namespace RoslynMacros.Common.Data
{
    public class AttrData : AttributeParser, IAttrData
    {
        public string NAME => Name;
        public string[] Parameters => Arguments;

        public AttrData(AttributeSyntax at) : base(at)
        {
        }
    }
}