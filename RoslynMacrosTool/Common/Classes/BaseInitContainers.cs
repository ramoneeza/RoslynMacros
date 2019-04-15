using System.Globalization;
using System.Reflection;
using LightInject;
using RoslynMacros.Common.Interfaces;
using RoslynMacros.Common.Scripts;

namespace RoslynMacros.Common.Classes
{
    public abstract class BaseInitContainers : IInitContainers
    {
        public IServiceContainer ServiceContainer { get; }
        public IConfiguration Configuration { get; }
        private Assembly[] Assemblies { get; }

        public BaseInitContainers(IServiceContainer container, IConfiguration configuration,
            params Assembly[] assemblies)
        {
            ServiceContainer = container;
            Configuration = configuration;
            Assemblies = assemblies;
        }

        public void InitAll()
        {
            ServiceContainer.Register(_ => Configuration, new PerContainerLifetime());
            InitProject();
            InitDataEngine();
            InitOutputEngine();
            InitMacroFactory();
            InitScripts(typeof(AutoImplement).Assembly);
            foreach (var assembly in Assemblies) InitScripts(assembly);
        }

        public virtual void InitDataEngine()
        {
            ServiceContainer.Register<IDataEngine>(s=>new BaseDataEngine(s.GetInstance<IMacroFactory>(),s.GetInstance<IProject>()),new PerContainerLifetime());
        }

        public abstract void InitMacroFactory();

        public virtual void InitOutputEngine()
        {
            ServiceContainer.Register<IOutputEngine>(s=>new BaseOutputEngine(s.GetInstance<IProject>()), new PerContainerLifetime());
        }

        public abstract void InitProject();

        public virtual void InitScripts(Assembly assembly)
        {
            ServiceContainer.RegisterAssembly(assembly, () => new PerRequestLifeTime(),
                (i, t) =>typeof(IMacroExecute).IsAssignableFrom(t), (i, t) => t.Name.ToLower(CultureInfo.CurrentCulture));
        }
    }
}