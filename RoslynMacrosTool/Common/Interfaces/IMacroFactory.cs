using System.IO;

namespace RoslynMacros.Common.Interfaces
{
    public interface IMacroFactory
    {
        IMacro<T> GetMacro<T>(string fe, DirectoryInfo alternativepath, out string[] error) where T : IVariables;
    }
}