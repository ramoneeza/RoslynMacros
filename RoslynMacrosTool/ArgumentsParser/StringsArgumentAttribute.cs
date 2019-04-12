namespace RoslynMacros.ArgumentsParser
{
    public sealed class StringsArgumentAttribute : AbsArgumentAttribute
    {
        public StringsArgumentAttribute(char c, string prefix,string description="",bool optional=true):base(c,prefix,description,optional,true)
        {
        }

        public override object Convert(string[] values)
        {
            return values;
        }

        public override object DefaultValue()
        {
            return new string[0];
        }
    }
}