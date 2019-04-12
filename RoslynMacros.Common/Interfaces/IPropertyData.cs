using System.Collections.Generic;

// ReSharper disable InconsistentNaming

namespace RoslynMacros.Common.Interfaces
{
    public interface IPropertyData : IBaseData
    {
        bool HASGETTER { get; }
        bool HASSETTER { get; }
        bool ISVIRTUAL { get; }
        string MODIFIERGETTER { get; }
        string MODIFIERSETTER { get; }
        IEnumerable<string> ATTRIBUTES { get; }
        IEnumerable<string> ATTRIBUTESIN(params string[] atts);
        bool DECO(params string[] attributename);
        string DECOVALUE(string attributename);
        string DECOVALUERAW(string attributename);
    }
}