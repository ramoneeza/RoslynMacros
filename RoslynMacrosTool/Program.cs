using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using RoslynMacros.ArgumentsParser;
using RoslynMacros.Common.Interfaces;
using RoslynMacros.Scripts;

namespace RoslynMacros
{
    class Program
    {
        static int Main(string[] args)
        {
            Microsoft.Build.Locator.MSBuildLocator.RegisterDefaults();
            try
            {
                var parser = new ParseArguments<Arguments>();

                var a = parser.Parse(args,"s");
                if (a == null)
                {
                    Console.WriteLine("RoslynMacros use:");
                    Console.WriteLine("----------------");
                    foreach (var c in parser.ParseHelp())
                    {
                        Console.WriteLine(c);
                    }

                    return 1;
                }


                var project = FindProject();
                if (string.IsNullOrEmpty(project)) throw new FileNotFoundException("Project file not found", "");
                Console.WriteLine($"Using project file: {project}.");
                var conf=new Configuration(a,new FileInfo(project));
                ExecuteCmd(project, conf);
                Console.WriteLine("END.");
                return 0;
            }
            finally
            {
                if (Debugger.IsAttached)
                {
                    Console.WriteLine("Press any key to continue . . .");
                    while (Console.KeyAvailable)
                    {
                        Console.ReadKey();
                    }

                    Console.ReadKey();
                }
            }
        }

        
        private static void ExecuteCmd(string project, IConfiguration configuration)
        {
            using (var container = new LightInject.ServiceContainer())
            {
                var init=new InitContainer(container,configuration,typeof(AutoConstructor).Assembly);
                init.InitAll();
                var cmd = new Command(container);
                cmd.Execute();
            }
        }
        
        private static string FindProject()
        {
            var project = "";
            var path = new DirectoryInfo(Directory.GetCurrentDirectory());
            while (path!=null)
            {
                project = Directory.GetFiles(path.FullName, "*.csproj").FirstOrDefault();
                if (!string.IsNullOrEmpty(project)) break;
                path = path.Parent;
            }
            return project;
        }
    }
}
