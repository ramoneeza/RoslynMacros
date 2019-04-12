using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.Linq;
using System.Reflection;

namespace RoslynMacros.ArgumentsParser
{
    internal class ArgumentProperties
    {
        #region Private
        private readonly ImmutableDictionary<PropertyInfo,IArgumentAttribute> _properties;
        private readonly ImmutableDictionary<string,(PropertyInfo, IArgumentAttribute)> _prefixes;

        private static readonly Dictionary<Type,ArgumentProperties> _argumentcollection=new Dictionary<Type, ArgumentProperties>();

        private ArgumentProperties(Type type)
        {
            if (!type.IsClass) throw new ArgumentException();
            ForType = type;
            var allproperties = type.GetProperties();
            var builder = ImmutableDictionary.CreateBuilder<PropertyInfo,IArgumentAttribute>();
            var builderprefix = ImmutableDictionary.CreateBuilder<string,(PropertyInfo,IArgumentAttribute)>(StringComparer.OrdinalIgnoreCase);
            foreach (var p in allproperties)
            {
                var attr = p.GetCustomAttributes().OfType<IArgumentAttribute>().FirstOrDefault();
                if (attr==null) continue;
                var tupla = (p, attr);
                builder.Add(p,attr);
                builderprefix.Add(attr.Prefix,tupla);
                builderprefix.Add(attr.PrefixShort.ToString(),tupla);
            }
            _properties = builder.ToImmutable();
            _prefixes = builderprefix.ToImmutable();
        }

        #endregion

        public Type ForType { get; }
        public IArgumentAttribute FindArgumentAttribute(PropertyInfo prop) =>(_properties.TryGetValue(prop, out var a) ? a : null);
        public (PropertyInfo,IArgumentAttribute) FindByPrefix(string prefix) =>_prefixes.TryGetValue(prefix, out var a) ? a : (null,null);
        public IEnumerable<(PropertyInfo Property, IArgumentAttribute ArgumentAttribute)> GetAll() => _properties.Select(kv => (kv.Key, kv.Value));
        public IEnumerable<PropertyInfo> GetAllProperties() => _properties.Keys;
        
        public static ArgumentProperties Factory(Type t)=>!_argumentcollection.TryGetValue(t, out var ap)?(_argumentcollection[t] =new ArgumentProperties(t)):ap;
        
    }

    public class ParseArguments<T> where T:class,new()
    {


        #region Private
        private readonly ArgumentProperties _properties=ArgumentProperties.Factory(typeof(T));

        private bool ParseAttribute(T res, PropertyInfo prop, IArgumentAttribute attr, string[] values)
        {
            try
            {
                var parsed = attr.Convert(values);
                if (parsed == null) return false;
                prop.SetValue(res,parsed);
                return true;
            }
#pragma warning disable 168
            catch (Exception ex)
#pragma warning restore 168
            {
                return false;
            }
        }

        private static IEnumerable<(string,string[])> Prefixer(IEnumerable<string> e)
        {
            string prefix = "";
            var items=new List<string>();
            foreach (var item in e.Append("-$$"))
            {
                if (item.StartsWith("-"))
                {
                    if (prefix!="" || items.Count>0)
                    {
                        yield return (prefix,items.ToArray());
                    }
                    prefix = item.Substring(1);
                    items.Clear();
                }
                else
                {
                    items.Add(item);
                }
            }
        }

        #endregion

        public T Parse(string[] args,string defaultprefix="")
        {
            var prefixerargs = Prefixer(args);
            var res=new T();
            var matches = _properties.GetAll().Where(pa=>!pa.Item2.Optional).ToDictionary(k => k.Item1, k =>false);
            foreach (var (prefix,values) in prefixerargs)
            {
                var p=(prefix != "")?prefix:defaultprefix;
                var (prop,attr) = _properties.FindByPrefix(p);
                if (prop == null||attr==null) return null;
                if (matches.TryGetValue(prop,out var b) && b) return null;
                if (!ParseAttribute(res,prop,attr,values)) return null;
                matches[prop] = true;
            }
            if (matches.Values.Any(m => !m)) return null;
            return res;
        }

        public IEnumerable<string> ParseHelp(string defaultprefix="")
        {
            var fp = _properties.FindByPrefix(defaultprefix).Item2;
            if (fp != null) yield return fp.ToString(false);
            foreach (var a in _properties.GetAll().Where(p=>p.ArgumentAttribute!=fp).Select(p=>p.ArgumentAttribute))
                yield return a.ToString();
        }

        
    }
}