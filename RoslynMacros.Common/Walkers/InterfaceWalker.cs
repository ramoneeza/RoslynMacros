using System.Linq;
using JetBrains.Annotations;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using RoslynMacros.Common.Interfaces;
using RoslynMacros.Common.WalkerResults;

namespace RoslynMacros.Common.Walkers
{
    [PublicAPI]
    public class InterfaceWalker : AbsWalker<InterfaceResult>
    {
        public InterfaceWalker(IDataEngine engine) : base(engine)
        {
        }

        public override void Visit(SyntaxTree st)
        {
            foreach (var interf in st.GetRoot().DescendantNodes().OfType<InterfaceDeclarationSyntax>())
                Results.Add(new InterfaceResult(interf));
        }
    }
}