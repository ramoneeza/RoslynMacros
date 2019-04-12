using System.IO;
using System.Text;
using RoslynMacros.Common.Data;

namespace RoslynMacros.Common.Interfaces
{
    public interface IVariables
    {
        FileInfo Filename { get; }
        string FILENAME { get; }
        string PATH { get; }
        FlagVar Flags { get; }
        string OUTPUT { get; set; }
        CollectionVar Includes { get; }
        CollectionVar Excludes { get; }
        CollectionStr Variables { get; }
        CollectionInt Values { get; }
        StringBuilder Output { get; }
        StringBuilder Log { get; }
        void Echo(string line);
        bool IsIncluded(string name);
        bool IsExcluded(string name);
        bool IsBasicType(string name);
        bool IsString(string name);
        bool IsBasicValueType(string name);
    }
}