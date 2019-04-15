using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using RoslynMacros.Common.Data;

namespace RoslynMacros.Common.Classes
{
    public partial class BaseVariables
    {
        // NON STATIC for Macro Use

        public Type FindType(string name)
        {
            name = name ?? "";
            if (!name.Contains("<")) return BasicType(name);
            var (gen, genpar) = GenericType(name);
            if (gen == null) return null;
            return gen.MakeGenericType(genpar);
        }

        public (Type, Type[]) GenericType(string name)
        {
            var p = name.IndexOf('<');
            if (p < 0) return (null, null);
            var p2 = name.LastIndexOf('>');
            if (p2 < 0) return (null, null);
            var gen = name.Substring(0, p);
            if (!Collections.TryGetValue(gen, out var tcol))
            {
                if (gen != "Nullable") return (null, null);
                tcol = typeof(Nullable<>);
            }

            var par = name.Substring(p + 1, p2 - p - 1);
            var pars = par.Split(',');
            var parst = pars.Select(FindType).ToArray();
            if (parst.Any(pp => pp == null)) return (null, null);
            return (tcol, parst);
        }

        public bool IsBasicType(string name) => BasicType(name) != null;

        public bool IsBasicValueType(string name)
        {
            var t = BasicType(name);
            if (t == null) return false;
            return t.IsValueType;
        }

        public bool IsString(string name)
        {
            var t = BasicType(name);
            if (t == null) return false;
            return t == typeof(string);
        }

        public bool IsNullableBasicType(string name)
        {
            var t = BasicType(name);
            if (t == null) return false;
            return (t.DeclaringType == typeof(Nullable<>));
        }

        public bool IsEnumerableBasicType(string name)
        {
            name = name ?? "";
            var p = name.IndexOf("<", StringComparison.Ordinal);
            if (p < 0) return false;
            if (!Collections.TryGetValue(name.Substring(0, p), out var t)) return false;
            return typeof(IEnumerable).IsAssignableFrom(t);
        }

        public bool IsIncluded(string name)
        {
            return Includes.Contains(name);
        }

        public bool IsExcluded(string name)
        {
            return Excludes.Contains(name);
        }

        // STATIC


        protected static string[] GetMacroVar(string macrovar, AttributeParser[] atts)
        {
            var att = atts.FirstOrDefault(a => a.Name == macrovar);
            return att == null ? new string[0] : att.Arguments;
        }

        public Type BasicType(string name)
        {
            name = name ?? "";
            if (name.EndsWith("?"))
            {
                var t = BasicType(name.Substring(0, name.Length - 1));
                if (t == null) return null;
                var st = typeof(Nullable<>);
                return st.MakeGenericType(t);
            }

            if (BasicTypes.TryGetValue(name, out var tt)) return tt;
            if (name.StartsWith("Nullable<"))
            {
                var (_, tts) = GenericType(name);
                if (tts == null) return null;
                var st = typeof(Nullable<>);
                return st.MakeGenericType(tts);
            }

            return Type.GetType($"System.{name}");
        }

        private static readonly Dictionary<string, Type> Collections = new Dictionary<string, Type>();

        private static readonly Dictionary<string, Type> BasicTypes = new Dictionary<string, Type>
        {
            ["object"] = typeof(Object),
            ["string"] = typeof(String),
            ["bool"] = typeof(Boolean),
            ["byte"] = typeof(Byte),
            ["char"] = typeof(Char),
            ["decimal"] = typeof(Decimal),
            ["double"] = typeof(Double),
            ["short"] = typeof(Int16),
            ["int"] = typeof(Int32),
            ["long"] = typeof(Int64),
            ["sbyte"] = typeof(SByte),
            ["float"] = typeof(Single),
            ["ushort"] = typeof(UInt16),
            ["uint"] = typeof(UInt32),
            ["ulong"] = typeof(UInt64),
            ["string"] = typeof(String),
            ["void"] = typeof(void)
        };

        static BaseVariables()
        {
            var coltypes = typeof(IList<>).Assembly.GetExportedTypes()
                .Where(t => t.Namespace == "System.Collections.Generic");
            foreach (var t in coltypes)
            {
                if (!t.IsGenericType) continue;
                var name = t.Name ?? "";
                var p = name.IndexOf("`", StringComparison.Ordinal);
                if (p < 0) continue;
                name = name.Substring(0, p);
                Collections.Add(name, t);
            }
        }
    }
}