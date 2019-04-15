namespace RoslynMacros.Common.Interfaces
{
    public interface IMacro
    {
        string Name { get; }
        string Compiled { get; }

        bool AreErrors { get; }
        string[] Errors { get; }
    }

    public interface IMacro<in T> : IMacro where T : IVariables
    {
        bool Execute(T variables, out string error);
    }
}