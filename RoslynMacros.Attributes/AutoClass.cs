using JetBrains.Annotations;
using System;
using System.Diagnostics;

namespace RoslynMacros
{
    [PublicAPI]
    [AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = true)]
    public class AutoClass: MacroTypeAttribute
    {
        public string GType { get; }
        public AutoClass(string macroname,Type className,Type gType=null) : base(macroname,className)
        {
            GType = gType?.Name??"";
            Debug.Assert(className.IsClass, "ClassName.IsClass");
        }
        public AutoClass(Type className,Type gType=null) : this(className.Name,className,gType)
        {
        }
    }
}
