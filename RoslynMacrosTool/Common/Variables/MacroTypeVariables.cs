using System;
using System.Linq;
using RoslynMacros.Common.Classes;
using RoslynMacros.Common.Data;
using RoslynMacros.Common.Interfaces;

namespace RoslynMacros.Common.Variables
{
    public class MacroTypeVariables : BaseVariables, IMacroTypeVariables
    {
        public IDataEngine Engine { get; }
        public InterfaceData REFINTERFACE => REFTYPE as InterfaceData;
        public ClassData REFCLASS => REFTYPE as ClassData;
        public ITypeData REFTYPE { get; set; }
        public string INTERFACENAME => INTERFACE?.NAME ?? "";
        public string INTERFACENAMESPACE => INTERFACE?.NAMESPACE ?? "";
        public string INTERFACEUSINGS => (INTERFACE == null) ? "" : TYPEUSINGS;
        public string CLASSNAME => CLASS.NAME ?? "";
        public string CLASSNAMESPACE => CLASS?.NAMESPACE ?? "";
        public string CLASSUSINGS => (CLASS == null) ? "" : TYPEUSINGS;
        public ClassData CLASS => TYPE as ClassData;
        public InterfaceData INTERFACE => TYPE as InterfaceData;
        public ITypeData TYPE { get; }
        public string[] Usings => TYPE.Usings;
        public string TYPEUSINGS => string.Concat(Usings.Select(u => $"using {u};{Environment.NewLine}"));
        public string TYPENAME => TYPE.NAME;
        public string TYPEWIDENAME => TYPE.WIDENAME;

        public MacroTypeVariables(IDataEngine engine, IWalkerTypeResult result, string output,
            AttributeParser[] attributes, ITypeData reftype) : base(result, output, attributes)
        {
            Engine = engine;
            TYPE = TypeData.Factory(engine, result.TypeDeclarationSyntax);
            REFTYPE = reftype;
        }

        //Funciones

        public IPropertyData FindDecoData(params string[] argumentos)
        {
            if (argumentos.Length == 0) return null;
            var attrs = argumentos.Select(a => a.StripQuote()).ToArray();
            var inter = REFTYPE;
            var prop = inter.PropertiesAll.Values.FirstOrDefault(p => p.ATTRIBUTES.Any(a => attrs.Contains(a)));
            return prop;
        }

        public IPropertyData FindProperty(string name)
        {
            var inter = REFTYPE;
            var prop = inter.PropertiesAll.Values.FirstOrDefault(p => p.NAME == name);
            return prop;
        }

        public string FindDecoProperty(string[] argumentos)
        {
            var prop = FindDecoData(argumentos);
            return prop?.NAME ?? "";
        }

        public bool HasProperty(string name)
        {
            var prop = FindProperty(name);
            return prop != null;
        }
    }
}