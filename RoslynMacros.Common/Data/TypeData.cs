using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using RoslynMacros.Common.Interfaces;

namespace RoslynMacros.Common.Data
{
    public class TypeData : ITypeData
    {
        public static TypeData Factory(IDataEngine engine, TypeDeclarationSyntax type)
        {
            switch (type)
            {
                case ClassDeclarationSyntax clase: return new ClassData(engine, clase);
                case InterfaceDeclarationSyntax interf: return new InterfaceData(engine, interf);
                default: return new TypeData(engine, type);
            }
        }

        public TypeDeclarationSyntax Declaration => Declarations.First();
        public IDataEngine Engine { get; }
        public SemanticModel SemanticModel { get; }
        public INamedTypeSymbol Symbol { get; }
        protected List<TypeDeclarationSyntax> Declarations { get; } = new List<TypeDeclarationSyntax>();
        public string[] Modifier { get;}
        public string NAME { get; }
        public string WIDENAME { get; }
        public string NAMESPACE { get; }
        public string GENERICS { get; private set; }
        public string[] GenericsParameters { get; private set; }
        public string[] Inheritance { get; private set; }
        public INamedTypeSymbol[] InheritanceAll { get; private set; }
        public Dictionary<string, IPropertyData> Properties { get; }
        public Dictionary<string, IMethodData> Methods { get; }
        public Dictionary<string, IEventData> Events { get; }
        public bool InheritanceLoaded { get; private set; }
        public Dictionary<string, IPropertyData> PropertiesAll { get; }
        public Dictionary<string, IMethodData> MethodsAll { get; }
        public Dictionary<string, IEventData> EventsAll { get; }

        public bool DECO(string attributename)
        {
            return AttributesData.Any(a =>
                string.Compare(a.NAME, attributename, StringComparison.OrdinalIgnoreCase) == 0);
        }

        public string DECOVALUE(string attributename)
        {
            return (AttributesData
                       .FirstOrDefault(a =>
                           string.Compare(a.NAME, attributename, StringComparison.OrdinalIgnoreCase) == 0)?.Parameters
                       ?.FirstOrDefault() ?? "").StripArgument();
        }

        public IEnumerable<string> ATTRIBUTES => AttributesData.Select(a => a.NAME);


        public IAttrData[] AttributesData { get; private set; }
        public string[] AttributesLists { get; private set; }

        public string[] Usings { get; }

        protected TypeData(IDataEngine engine, TypeDeclarationSyntax declarationsyntax)
        {
            Engine = engine;
            NAME = declarationsyntax.Identifier.ToString();
            GenericsParameters = declarationsyntax.TypeParameterList.GetGenerics();
            WIDENAME = declarationsyntax.WideName();
            Engine.AllTypeData[WIDENAME] = this;
            Declarations.Add(declarationsyntax);

            NAMESPACE = declarationsyntax.FirstAncestorOrSelf<NamespaceDeclarationSyntax>().Name.ToString();
            Usings = declarationsyntax.FirstAncestorOrSelf<CompilationUnitSyntax>().Usings
                .Select(u => u.Name.ToString()).ToArray();
            SemanticModel = Engine.SemanticModel(declarationsyntax.SyntaxTree);
            Symbol = SemanticModel.GetDeclaredSymbol(declarationsyntax);
            Declarations.AddRange(Symbol.DeclaringSyntaxReferences.Select(r => r.GetSyntax())
                .OfType<TypeDeclarationSyntax>().Where(d => d != declarationsyntax));
            Modifier = Declarations.SelectMany(d => d.Modifiers).Select(m => m.Text).Distinct().ToArray();
            Inheritance = Declarations.SelectMany(d => d.ChildNodes()).OfType<BaseListSyntax>().SelectMany(b=>b.Types.Select(t => t.Type.ToString())).Distinct().ToArray();            
            Properties = new Dictionary<string, IPropertyData>();
            Methods = new Dictionary<string, IMethodData>();
            Events = new Dictionary<string, IEventData>();
            PropertiesAll = new Dictionary<string, IPropertyData>();
            MethodsAll = new Dictionary<string, IMethodData>();
            EventsAll = new Dictionary<string, IEventData>();
            
            var attls = declarationsyntax.AttributeLists;
            var att = new List<IAttrData>();
            var lst = new List<string>();
            foreach (var lista in attls)
            {
                lst.Add(lista.ToString());
                foreach (var a in lista.Attributes) att.Add(new AttrData(a));
            }

            AttributesData = att.ToArray();
            AttributesLists = lst.ToArray();

            

            foreach (var child in Declarations.SelectMany(d => d.ChildNodes()))
                switch (child)
                {
                    case PropertyDeclarationSyntax p:
                        if (!Properties.ContainsKey(p.Identifier.ToString()))
                            Properties.Add(p.Identifier.ToString(), new PropertyData(p));
                        break;
                    case EventDeclarationSyntax ev:
                        if (!Events.ContainsKey(ev.Identifier.ToString()))
                            Events.Add(ev.Identifier.ToString(), new EventData(ev));
                        break;
                    case MethodDeclarationSyntax m:
                        if (!Methods.ContainsKey(m.Identifier.ToString()))
                            Methods.Add(m.Identifier.ToString(), new MethodData(m));
                        break;
                }
            var ctype = Symbol;
            var inheritanceall = new HashSet<INamedTypeSymbol>();
            while (ctype != null && ctype.Name != "Object")
            {
                if (ctype != Symbol) inheritanceall.Add(ctype);
                foreach (var s in ctype.Interfaces)
                {
                    inheritanceall.Add(s);
                    foreach (var si in s.AllInterfaces)
                    {
                        inheritanceall.Add(si);
                    }
                }
                ctype = ctype.BaseType;
            }

            InheritanceAll = inheritanceall.ToArray();



        void LoadIfNotKey<V>(Dictionary<string, V> dic, KeyValuePair<string, V> p)
            {
                if (!dic.ContainsKey(p.Key)) dic[p.Key] = p.Value;
            }

            foreach (var p in Properties) LoadIfNotKey(PropertiesAll, p);
            foreach (var e in Events) LoadIfNotKey(EventsAll, e);
            foreach (var m in Methods) LoadIfNotKey(MethodsAll, m);
            foreach (var sub in InheritanceAll)
            {
                var subvalue = Engine.GetRecordForSymbol(sub.Name);
                if (subvalue == null) continue;
                foreach (var p in subvalue.Properties) LoadIfNotKey(PropertiesAll, p);
                foreach (var e in subvalue.Events) LoadIfNotKey(EventsAll, e);
                foreach (var m in subvalue.Methods) LoadIfNotKey(MethodsAll, m);
            }
            InheritanceLoaded = true;
        }

        public IPropertyData[] PROPERTIES => Properties.Values.ToArray();
        public IPropertyData[] PROPERTIESRW => Properties.Values.Where(p => p.HASGETTER && p.HASSETTER).ToArray();
        public IPropertyData[] PROPERTIESREAD => Properties.Values.Where(p => p.HASGETTER && !p.HASSETTER).ToArray();

        public IPropertyData[] PROPERTIESALL => PropertiesAll.Values.ToArray();
        public IPropertyData[] PROPERTIESALLRW => PropertiesAll.Values.Where(p => p.HASGETTER && p.HASSETTER).ToArray();

        public IPropertyData[] PROPERTIESALLREAD =>
            PropertiesAll.Values.Where(p => p.HASGETTER && !p.HASSETTER).ToArray();

        public IPropertyData[] PROPERTIESDECORATED(params string[] deco) =>
            PropertiesAll.Values.Where(p => p.DECO(deco)).ToArray();

        public IEventData[] EVENTS => Events.Values.ToArray();
        public IEventData[] EVENTSALL => EventsAll.Values.ToArray();
        public bool InheritsFrom(string name) => InheritanceAll.Any(s => s.Name == name);
        public bool DirectInheritsFrom(string name) => Inheritance.Contains(name);
    }

    public abstract class TypeData<T> : TypeData where T : TypeDeclarationSyntax
    {
        public new T Declaration => Declarations.First() as T;

        protected TypeData(IDataEngine engine, T declarationsyntax) : base(engine, declarationsyntax)
        {
        }
    }
}