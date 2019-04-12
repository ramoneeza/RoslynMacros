using System;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace RoslynMacros.Common.WalkerResults
{
    public class InterfaceResult : AbsWalkerTypeResult
    {
        public override Type TypeDeclarationSyntaxType => typeof(InterfaceDeclarationSyntax);

        public InterfaceDeclarationSyntax InterfaceDeclarationSyntax =>
            TypeDeclarationSyntax as InterfaceDeclarationSyntax;

        public InterfaceResult(InterfaceDeclarationSyntax typeDeclaration) : base(typeDeclaration)
        {
        }
    }
}