using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using RoslynMacros.Common.Data;
using RoslynMacros.Common.Interfaces;

namespace RoslynMacros.Common.Classes
{
    public class BaseDataEngine : IDataEngine
    {
        public IProject CurrentProject { get; }
        public DirectoryInfo ProjectPath => CurrentProject.ProjectPath;
        public Compilation Compilation => CurrentProject.Compilation;
        public SemanticModel SemanticModel(SyntaxTree st) => Compilation.GetSemanticModel(st);
        private Dictionary<string, IMacro> Macros { get; } = new Dictionary<string, IMacro>();
        protected IMacroFactory MacroFactory { get; }

        public IMacro<T> GetMacro<T>(string fe, DirectoryInfo alternativepath, out string[] error) where T : IVariables
        {
            return MacroFactory.GetMacro<T>(fe, alternativepath, out error);
        }


        public BaseDataEngine(IMacroFactory macrofactory, IProject currentproject)
        {
            MacroFactory = macrofactory;
            CurrentProject = currentproject;
        }

        public Dictionary<string, ITypeData> AllTypeData { get; } = new Dictionary<string, ITypeData>();

        private ITypeData GetRecordForSymbol(INamedTypeSymbol s)
        {
            if (!(s is INamedTypeSymbol ss &&
                  (ss.TypeKind == TypeKind.Interface || ss.TypeKind == TypeKind.Class))) return null;
            var st = ss.DeclaringSyntaxReferences.FirstOrDefault()?.GetSyntax();
            if (st is InterfaceDeclarationSyntax idt) return new InterfaceData(this, idt);
            if (st is ClassDeclarationSyntax cdt) return new ClassData(this, cdt);
            return null;
        }

        public InterfaceData GetRecordForInterface(string interfacename)
        {
            return GetRecordForSymbol(interfacename) as InterfaceData;
        }

        public ClassData GetRecordForClass(string classname)
        {
            return GetRecordForSymbol(classname) as ClassData;
        }

        public ITypeData GetRecordForSymbol(string sname)
        {
            if (!AllTypeData.TryGetValue(sname, out var value))
            {
                var s = Compilation.GetSymbolsWithName(sname, SymbolFilter.Type).FirstOrDefault();
                return GetRecordForSymbol(s as INamedTypeSymbol);
            }

            return value;
        }

        private SyntaxTree[] _proyectSyntaxTrees;

        public SyntaxTree[] ProyectSyntaxTrees =>
            _proyectSyntaxTrees ?? (_proyectSyntaxTrees = Compilation.SyntaxTrees.ToArray());

        public IEnumerable<SyntaxTree> GetSyntaxTrees(string[] files) =>
            ((files?.Length ?? 0) == 0)
                ? ProyectSyntaxTrees
                : ProyectSyntaxTrees.Where(s => files.Contains(Path.GetFileName(s.FilePath)));


        //public void WriteResults(IVariables variables, string fspath, string filename)
        //{
        //    var fs = variables.@OUTPUT;
        //    var fullfs = Path.Combine(fspath, fs);
        //    Output[fullfs].Write(variables.Output.ToString());
        //    Output[fullfs].Nest = filename;
        //    if (Verbose)
        //    {
        //        var log = variables.Log.ToString().Trim();
        //        if (!string.IsNullOrEmpty(log))
        //        {
        //            Output[fullfs + ".log"].WriteLine(log);
        //            Output[fullfs + ".log"].Nest = filename;
        //        }
        //    }
        //}
    }
}