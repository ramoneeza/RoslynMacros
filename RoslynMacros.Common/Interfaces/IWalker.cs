using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace RoslynMacros.Common.Interfaces
{
    public interface IWalker
    {
        void Visit(SyntaxTree st);
    }

    public interface IWalker<WR> : IWalker where WR : IWalkerResult
    {
        IList<WR> Results { get; }
    }

    public interface IWalkerResult
    {
        FileInfo FilePath { get; }
        DirectoryInfo FsPath { get; }
    }

    public interface IWalkerTypeResult<out T> : IWalkerTypeResult where T : TypeDeclarationSyntax
    {
        T DeclarationSyntax { get; }
    }

    public interface IWalkerTypeResult : IWalkerResult
    {
        Type TypeDeclarationSyntaxType { get; }
        TypeDeclarationSyntax TypeDeclarationSyntax { get; }
        string TypeName { get; }
        string WideName { get; }
    }
}