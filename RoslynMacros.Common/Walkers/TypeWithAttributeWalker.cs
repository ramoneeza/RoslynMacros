using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using RoslynMacros.Common.Interfaces;
using RoslynMacros.Common.WalkerResults;

namespace RoslynMacros.Common.Walkers
{
    public abstract class TypeWithAttributeWalker<WR> : AbsWalker<WR> where WR : AbsWalkerTypeWithAttributeResult
    {
        protected TypeWithAttributeWalker(IDataEngine engine, string attribute) : base(engine)
        {
            AttributeName = attribute;
        }

        public string AttributeName { get; }
        protected abstract void Add(TypeDeclarationSyntax type, AttributeSyntax att);

        public override void Visit(SyntaxTree st)
        {
            foreach (var att in st.GetRoot().DescendantNodes().OfType<AttributeSyntax>())
                if (att.Name.ToString() == AttributeName)
                {
                    var tipo = att.Parent.Parent as TypeDeclarationSyntax;

                    Add(tipo, att);
                }
        }
    }
}