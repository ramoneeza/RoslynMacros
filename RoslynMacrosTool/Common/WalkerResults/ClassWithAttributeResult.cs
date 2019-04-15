using System;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace RoslynMacros.Common.WalkerResults
{
    public class ClassWithAttributeResult : AbsWalkerTypeWithAttributeResult
    {
        public ClassDeclarationSyntax ClassDeclarationSyntax => TypeDeclarationSyntax as ClassDeclarationSyntax;
        public string ClassName => TypeName;

        public ClassWithAttributeResult(ClassDeclarationSyntax @class, AttributeSyntax attribute) : base(@class,
            attribute)
        {
        }

        public override Type TypeDeclarationSyntaxType => typeof(ClassDeclarationSyntax);
    }
}