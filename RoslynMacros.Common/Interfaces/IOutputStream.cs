using System.IO;

namespace RoslynMacros.Common.Interfaces
{
    public interface IOutputStream
    {
        void End();
        void Write(string cad);
        void WriteLine(string cad);
        FileInfo Nest { get; set; }
        FileInfo File { get; }
    }
}