using LightInject;

namespace RoslynMacros.Common.Interfaces
{
    public interface IMacroExecute
    {
        string Name { get; }
        IDataEngine Engine { get; }
        IMacroFactory MacroFactory { get; }
        IOutputEngine OutputEngine { get; }
        IProject CurrentProject { get; }
        void ExecuteScript();
    }
}