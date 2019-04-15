using LightInject;

namespace RoslynMacros.Common.Interfaces
{
    public interface ICommand
    {
        IServiceFactory ServiceFactory { get; }
        void Execute();
    }
}