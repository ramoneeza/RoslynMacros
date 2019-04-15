using System.IO;
using Microsoft.CodeAnalysis;

namespace RoslynMacros.Common.Interfaces
{
    public interface IProject
    {
        IConfiguration Configuration { get; }
        FileInfo ProjectFile { get; }
        DirectoryInfo ProjectPath { get; }
        Project Project { get; }
        Compilation Compilation { get; }
        bool Changed { get; }
        string StripBase(FileInfo file);
        string StripBase(DirectoryInfo file);
        string RelativePath(FileInfo file);
        bool AddCsFile(FileInfo file, FileInfo nest);
        void WriteResults(IVariables variables);
        void SaveProject();
    }
}