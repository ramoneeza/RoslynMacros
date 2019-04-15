using System.Linq;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using RoslynMacros.Common.Interfaces;

// ReSharper disable InconsistentNaming

namespace RoslynMacros.Common.Data
{
    public class InterfaceData : TypeData<InterfaceDeclarationSyntax>
    {
        public InterfaceData(IDataEngine engine, InterfaceDeclarationSyntax @interface) : base(engine, @interface)
        {
        }
    }

    public class ClassData : TypeData<ClassDeclarationSyntax>
    {
        public bool ISABSTRACT => Modifier.Contains("abstract");
        public ClassData(IDataEngine engine, ClassDeclarationSyntax @class) : base(engine, @class)
        {
        }
    }
}