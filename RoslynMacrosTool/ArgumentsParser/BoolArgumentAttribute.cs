using System.Linq;

namespace RoslynMacros.ArgumentsParser
{
    public sealed class BoolArgumentAttribute : AbsArgumentAttribute
    {
        public BoolArgumentAttribute(char c, string prefix,string description="",bool optional=true):base(c,prefix,description,optional,false)
        {
        }

        public override object Convert(string[] values)
        {
            if (values.Length >1) return null;
            if (values.Length == 1)
            {
                var v = values.First().ToLowerInvariant();
                switch (v)
                {
                    case "true": return true;
                    case "false": return false;
                    default: return null;
                }

            }
            return true;
        }

        public override object DefaultValue()
        {
            return false;
        }
    }
}