using System.Collections.Generic;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

// ReSharper disable InconsistentNaming

namespace RoslynMacros.Common.Interfaces
{
    public interface ITypeData
    {
        IDataEngine Engine { get; }
        TypeDeclarationSyntax Declaration { get; }
        string[] Modifier { get; }
        string NAME { get; }
        string WIDENAME { get; }
        string NAMESPACE { get; }
        string[] Usings { get; }
        string[] GenericsParameters { get; }
        string[] Inheritance { get; }
        INamedTypeSymbol[] InheritanceAll { get; }
        Dictionary<string, IPropertyData> Properties { get; }
        Dictionary<string, IMethodData> Methods { get; }
        Dictionary<string, IEventData> Events { get; }
        Dictionary<string, IPropertyData> PropertiesAll { get; }
        Dictionary<string, IMethodData> MethodsAll { get; }
        Dictionary<string, IEventData> EventsAll { get; }
        IPropertyData[] PROPERTIES { get; }
        IPropertyData[] PROPERTIESREAD { get; }
        IPropertyData[] PROPERTIESRW { get; }
        IPropertyData[] PROPERTIESALL { get; }
        IPropertyData[] PROPERTIESALLREAD { get; }
        IPropertyData[] PROPERTIESALLRW { get; }
        IPropertyData[] PROPERTIESDECORATED(params string[] deco);
        IEventData[] EVENTS { get; }
        IEventData[] EVENTSALL { get; }

        IEnumerable<string> ATTRIBUTES { get; }
        bool DECO(string attributename);
        string DECOVALUE(string attributename);
    }
}