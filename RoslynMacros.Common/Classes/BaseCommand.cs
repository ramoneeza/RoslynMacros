using System;
using System.Globalization;
using JetBrains.Annotations;
using LightInject;
using RoslynMacros.Common.Interfaces;

namespace RoslynMacros.Common.Classes
{
    [PublicAPI]
    public class BaseCommand : ICommand

    {
        public IServiceFactory ServiceFactory { get; }

        public BaseCommand(IServiceFactory serviceFactory)
        {
            ServiceFactory = serviceFactory;

        }

        public virtual void Execute()
        {
            var configuration = ServiceFactory.GetInstance<IConfiguration>();
            var output = ServiceFactory.GetInstance<IOutputEngine>();
            var project = ServiceFactory.GetInstance<IProject>();
            foreach (var s in configuration.Scripts)
            {
                output.ConsoleWrite($"Executing Script {s}");
                
                var mt = ServiceFactory.TryGetInstance<IMacroExecute>(s.Trim().ToLower(CultureInfo.CurrentCulture));
                if (mt == null || !mt.Name.Equals(s,StringComparison.CurrentCultureIgnoreCase))
                {
                    output.ConsoleWrite($"Error: No Script {s} found.");
                    continue;
                }

                mt.ExecuteScript();
            }

            if (project.Changed) project.SaveProject();
        }
    }
}