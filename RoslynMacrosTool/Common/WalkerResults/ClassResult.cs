using System;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace RoslynMacros.Common.WalkerResults
{
    public class ClassResult : AbsWalkerTypeResult
    {
        public override Type TypeDeclarationSyntaxType => typeof(ClassDeclarationSyntax);
        public ClassDeclarationSyntax ClassDeclarationSyntax => TypeDeclarationSyntax as ClassDeclarationSyntax;

        public ClassResult(ClassDeclarationSyntax typeDeclaration) : base(typeDeclaration)
        {
        }
    }
}