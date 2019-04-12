using JetBrains.Annotations;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using RoslynMacros.Common.Interfaces;
using RoslynMacros.Common.WalkerResults;

namespace RoslynMacros.Common.Walkers
{
    [PublicAPI]
    public class InterfaceWithAttributeWalker : TypeWithAttributeWalker<InterfaceWithAttributeResult>
    {
        public InterfaceWithAttributeWalker(IDataEngine engine, string attribute) : base(engine, attribute)
        {
        }

        protected override void Add(TypeDeclarationSyntax type, AttributeSyntax att)
        {
            var r = new InterfaceWithAttributeResult(type as InterfaceDeclarationSyntax, att);
            Results.Add(r);
        }
    }
}