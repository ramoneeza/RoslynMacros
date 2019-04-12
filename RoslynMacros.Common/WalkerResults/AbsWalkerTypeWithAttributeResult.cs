using System.Linq;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using RoslynMacros.Common.Data;

namespace RoslynMacros.Common.WalkerResults
{
    public abstract class AbsWalkerTypeWithAttributeResult : AbsWalkerTypeResult
    {
        public AttributeParser Attribute { get; }
        public AttributeParser[] AttributeList { get; }
        public string AttrName => Attribute.Name;

        protected AbsWalkerTypeWithAttributeResult(TypeDeclarationSyntax typeDeclaration, AttributeSyntax attribute) :
            base(typeDeclaration)
        {
            Attribute = new AttributeParser(attribute);
            var alist = (AttributeListSyntax) attribute.Parent;
            AttributeList = alist.Attributes.Select(a => new AttributeParser(a)).ToArray();
        }
    }
}