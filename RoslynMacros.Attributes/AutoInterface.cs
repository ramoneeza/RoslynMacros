using JetBrains.Annotations;
using System;
using System.Diagnostics;

namespace RoslynMacros
{
    [PublicAPI]
    [AttributeUsage(AttributeTargets.Interface, Inherited = false, AllowMultiple = true)]
    public sealed class AutoInterface : MacroTypeAttribute
    {
        public Type GType { get; }
        public AutoInterface(Type interfaceName, Type gType = null) : this(interfaceName.Name, interfaceName, gType) { }
        public AutoInterface(string macroName, Type interfaceName, Type gType = null) : base(macroName, interfaceName)
        {
            Debug.Assert(interfaceName.IsInterface, "ClassName.IsInterface");
            GType = gType;
        }
    }
    [PublicAPI]
    [AttributeUsage(AttributeTargets.Interface, Inherited = false, AllowMultiple = true)]
    public sealed class Contract : MacroTypeAttribute
    {
        public Type GType { get; }
        public Contract(Type interfaceName, Type gType = null) : this(interfaceName.Name, interfaceName, gType) { }
        public Contract(string macroName, Type interfaceName, Type gType = null) : base(macroName, interfaceName)
        {
            Debug.Assert(interfaceName.IsInterface, "ClassName.IsInterface");
            GType = gType;
        }
    }
}
