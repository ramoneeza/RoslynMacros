// ReSharper disable InconsistentNaming

using RoslynMacros.Common.Data;

namespace RoslynMacros.Common.Interfaces
{
    public interface IMacroTypeVariables : IVariables
    {
        IDataEngine Engine { get; }
        InterfaceData REFINTERFACE { get; }
        ClassData REFCLASS { get; }
        ITypeData REFTYPE { get; }
        string INTERFACENAME { get; }
        string INTERFACENAMESPACE { get; }
        string INTERFACEUSINGS { get; }
        string CLASSNAME { get; }
        string CLASSNAMESPACE { get; }
        string CLASSUSINGS { get; }
        ClassData CLASS { get; }
        InterfaceData INTERFACE { get; }
        ITypeData TYPE { get; }
        string TYPEUSINGS { get; }
        string TYPENAME { get; }
        string TYPEWIDENAME { get; }
        IPropertyData FindDecoData(string[] argumentos);
        IPropertyData FindProperty(string name);
        string FindDecoProperty(string[] argumentos);
        bool HasProperty(string name);
    }
}