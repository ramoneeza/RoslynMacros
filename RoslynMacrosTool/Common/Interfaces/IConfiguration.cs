using System.IO;

namespace RoslynMacros.Common.Interfaces
{
    public interface IConfiguration
    {
        FileInfo Project { get; }
        string[] Scripts { get; }
        string[] Files { get; }
        bool Verbose { get; }
    }
}