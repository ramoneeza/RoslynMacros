namespace RoslynMacros.ArgumentsParser
{
    public interface IArgumentAttribute
    {
        char PrefixShort { get; }
        string Prefix { get; }
        string Description { get; }
        bool Optional { get; }
        string ToString(bool showprefix);
        bool Array { get; }
        object Convert(string[] values);
        object DefaultValue();
    }
}