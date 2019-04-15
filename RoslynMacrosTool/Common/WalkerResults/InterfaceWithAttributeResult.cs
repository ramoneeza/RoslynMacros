using System;
using JetBrains.Annotations;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace RoslynMacros.Common.WalkerResults
{
    [PublicAPI]
    public class InterfaceWithAttributeResult : AbsWalkerTypeWithAttributeResult
    {
        InterfaceDeclarationSyntax InterfaceDeclarationSyntax => TypeDeclarationSyntax as InterfaceDeclarationSyntax;

        public InterfaceWithAttributeResult(InterfaceDeclarationSyntax @interface, AttributeSyntax attribute) : base(
            @interface, attribute)
        {
        }

        public override Type TypeDeclarationSyntaxType => typeof(InterfaceDeclarationSyntax);
    }
}