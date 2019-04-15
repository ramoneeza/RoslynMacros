using JetBrains.Annotations;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using RoslynMacros.Common.Interfaces;
using RoslynMacros.Common.WalkerResults;

namespace RoslynMacros.Common.Walkers
{
    [PublicAPI]
    public class ClassWithAttributeWalker : TypeWithAttributeWalker<ClassWithAttributeResult>
    {
        public ClassWithAttributeWalker(IDataEngine engine, string attribute) : base(engine, attribute)
        {
        }

        protected override void Add(TypeDeclarationSyntax type, AttributeSyntax att)
        {
            var r = new ClassWithAttributeResult(type as ClassDeclarationSyntax, att);
            Results.Add(r);
        }
    }
}