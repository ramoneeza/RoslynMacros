using System.Linq;
using JetBrains.Annotations;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using RoslynMacros.Common.Interfaces;
using RoslynMacros.Common.WalkerResults;

namespace RoslynMacros.Common.Walkers
{
    [PublicAPI]
    public class ClassWalker : AbsWalker<ClassResult>
    {
        public ClassWalker(IDataEngine engine) : base(engine)
        {
        }

        public override void Visit(SyntaxTree st)
        {
            foreach (var clase in st.GetRoot().DescendantNodes().OfType<ClassDeclarationSyntax>())
                Results.Add(new ClassResult(clase));
        }
    }
}