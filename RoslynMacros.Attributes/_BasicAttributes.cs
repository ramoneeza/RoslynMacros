using JetBrains.Annotations;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace RoslynMacros
{

    #region Abstract Attributes
#pragma warning disable RCS1203 // Use AttributeUsageAttribute.
    public abstract class MacroArrayAttribute : Attribute
    {
        [NotNull] public string[] Data { get; }
        // This is a positional argument
        protected MacroArrayAttribute(params string[] flags) { Data = flags ?? new string[] { }; }
    }
    public abstract class MacroDicAttribute : Attribute
    {
        [NotNull] public Dictionary<string, string> Data { get; }

        protected MacroDicAttribute(params string[] data)
        {
            Data = new Dictionary<string, string>();
            if (data != null)
                foreach (var v in data)
                {
                    var nv = v.Split('|');
                    if (nv.Length == 0)
                        throw new ArgumentException("Falta nombre variable");
                    var a = nv[0];
                    var b = (nv.Length > 1) ? nv[1] : "";
                    if (!string.IsNullOrEmpty(a)) Data.Add(a, b);

                }
        }
    }
    public abstract class MacroTypeAttribute : Attribute
    {
        // ReSharper disable once PrivateFieldCanBeConvertedToLocalVariable
        // ReSharper disable once MemberCanBePrivate.Global
        public Type TypeName { get; }
        public string MacroName { get; }
        protected MacroTypeAttribute(string macroname, Type typeName)
        {
            MacroName = macroname;
            TypeName = typeName;
        }
        
    }
    
    


#pragma warning restore RCS1203 // Use AttributeUsageAttribute.
    #endregion


    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Interface, Inherited = false, AllowMultiple = true)]
    [PublicAPI]
    public sealed class MacroFlagsAttribute : MacroArrayAttribute
    {
        public MacroFlagsAttribute(params string[] flags) : base(flags) { }
    }
    [PublicAPI]
    [AttributeUsageAttribute(AttributeTargets.Class | AttributeTargets.Interface, Inherited = false, AllowMultiple = true)]
    public sealed class MacroVarsAttribute : MacroDicAttribute
    {
        public MacroVarsAttribute(params string[] vars) : base(vars)
        {
            Debug.Assert(Data.Keys.All(k => k[0] == '@'), "Data.Keys.All(k=>k[0]=='@')");
        }
    }
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Interface, Inherited = false, AllowMultiple = true)]
    [PublicAPI]
    public sealed class MacroIncludeAttribute : MacroArrayAttribute
    {
        public MacroIncludeAttribute(params string[] flags) : base(flags) { }
    }
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Interface, Inherited = false, AllowMultiple = true)]
    [PublicAPI]
    public sealed class MacroExcludeAttribute : MacroArrayAttribute
    {
        public MacroExcludeAttribute(params string[] flags) : base(flags) { }
    }
}
