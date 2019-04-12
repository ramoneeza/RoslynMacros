using JetBrains.Annotations;
using System;
using System.Diagnostics;

namespace RoslynMacros
{
    [PublicAPI]
    [AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = true)]
    public sealed class AutoImplement : MacroTypeAttribute
    {
        public Type GType { get; }
        public AutoImplement(Type interfaceName, Type gType=null) : this(interfaceName.Name,interfaceName,gType) {}
        public AutoImplement(string macroName, Type interfaceName, Type gType = null) : base(macroName,interfaceName)
        {
            Debug.Assert(interfaceName.IsInterface, "ClassName.IsInterface");
            GType = gType;
        }
    }
}
