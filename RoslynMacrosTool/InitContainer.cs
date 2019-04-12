using System.IO;
using System.Reflection;
using LightInject;
using RoslynMacros.Common.Classes;
using RoslynMacros.Common.Interfaces;
using RoslynMacros.Macros;
using RoslynMacros.MsBuild;

namespace RoslynMacros
{
    public class Configuration : IConfiguration
    {
        public FileInfo Project { get; }
    

        public string[] Scripts { get; }

        public string[] Files { get; }

        public bool Verbose { get; }

        public Configuration(Arguments a,FileInfo project)
        {
            Scripts = a.Scripts;
            Files = a.Files;
            Verbose = a.Verbose;
            Project = project;
        }
    }

    public class InitContainer:BaseInitContainers{
        public InitContainer(IServiceContainer container, IConfiguration configuration, params Assembly[] assemblies) : base(container, configuration, assemblies)
        {
        }

        public override void InitMacroFactory()
        {
            ServiceContainer.Register<IMacroFactory,MacroFactory>(new PerContainerLifetime());
        }

        public override void InitProject()
        {
            ServiceContainer.Register<IProject,MsBuildProject>(new PerContainerLifetime());
        }
    }
}