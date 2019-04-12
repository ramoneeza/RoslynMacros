using System;

namespace RoslynMacros.ArgumentsParser
{
    public abstract class AbsArgumentAttribute : Attribute, IArgumentAttribute
    {
        public char PrefixShort { get; }
        public string Prefix{get;}
        public string Description { get; }
        public bool Optional { get; }
        public bool Array { get; }

        protected AbsArgumentAttribute(char c, string prefix,string description,bool optional,bool array)
        {
            PrefixShort = c;
            Prefix = prefix;
            Description = description;
            Optional = optional;
            Array = array;
        }
        public override string ToString()
        {
            return ToString(true);
        }
        public string ToString(bool showprefix)
        {
            return (showprefix)?
                ( (Optional) ? $"[(-{PrefixShort} | -{Prefix}) <{Description}>]" : $"<{Description}>") :
                ((Optional) ? $"[<{Description}>]" : $"<{Description}>");
        }

        public abstract object Convert(string[] values);
        public abstract object DefaultValue();
        
    }
}