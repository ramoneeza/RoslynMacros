using System.Reflection;
using LightInject;

namespace RoslynMacros.Common.Interfaces
{
    public interface IInitContainers
    {
        IServiceContainer ServiceContainer { get; }
        IConfiguration Configuration { get; }
        void InitScripts(Assembly assembly);
        void InitProject();
        void InitDataEngine();
        void InitOutputEngine();
        void InitMacroFactory();
        void InitAll();
    }
}