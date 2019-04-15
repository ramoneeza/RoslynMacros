using System;
using System.IO;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using RoslynMacros.Common.Interfaces;

namespace RoslynMacros.Common.WalkerResults
{
    public abstract class AbsWalkerTypeResult : IWalkerTypeResult
    {
        public FileInfo FilePath { get; }
        public DirectoryInfo FsPath { get; }
        public TypeDeclarationSyntax TypeDeclarationSyntax { get; }

        public abstract Type TypeDeclarationSyntaxType { get; }

        public string TypeName { get; }
        public string WideName { get; }

        protected AbsWalkerTypeResult(TypeDeclarationSyntax typeDeclaration)
        {
            TypeDeclarationSyntax = typeDeclaration;
            FilePath = new FileInfo(typeDeclaration.SyntaxTree.FilePath);
            FsPath = FilePath.Directory;
            TypeName = typeDeclaration.Identifier.ToString();
            WideName = typeDeclaration.WideName();
        }
    }
}