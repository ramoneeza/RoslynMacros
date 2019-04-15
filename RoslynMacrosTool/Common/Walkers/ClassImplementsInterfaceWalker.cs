using System.Linq;
using JetBrains.Annotations;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using RoslynMacros.Common.Interfaces;
using RoslynMacros.Common.WalkerResults;

namespace RoslynMacros.Common.Walkers
{
    [PublicAPI]
    public class ClassImplementsInterfaceWalker : AbsWalker<ClassResult>
    {
        public string Interface { get; }

        public ClassImplementsInterfaceWalker(IDataEngine engine, string interf) : base(engine)
        {
            Interface = interf;
        }

        public override void Visit(SyntaxTree st)
        {
            var model = Engine.SemanticModel(st);
            foreach (var clase in st.GetRoot().DescendantNodes().OfType<ClassDeclarationSyntax>())
            {
                var symbol = model.GetDeclaredSymbol(clase) as INamedTypeSymbol;
                if (symbol?.AllInterfaces.Any(i => i.Name == Interface) ?? false) Results.Add(new ClassResult(clase));
            }
        }
    }
}