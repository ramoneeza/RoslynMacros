using JetBrains.Annotations;
using System;

namespace RoslynMacros
{
    [PublicAPI]
    [AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = true)]
    public sealed class WrapInterface : MacroTypeAttribute
    {
        public string Container { get; }
        public string Getter { get; }
        public string Setter { get; }
        public WrapInterface(string macroname,Type interfaceName, string container, string getter = null, string setter = null) : base(macroname,interfaceName)
        {
            Container = container;
            Getter = getter;
            Setter = setter;
        }

    }
}
