using System;
using System.IO;
using System.Text;
using RoslynMacros.Common.Interfaces;

namespace RoslynMacros.Common.Classes
{
    public class BaseOutputEngine : IOutputEngine, IDisposable
    {
        public IProject Project { get; }
        public bool Verbose => Project.Configuration.Verbose;

        public void LogWrite(string line)
        {
            if (Verbose) _log.WriteLine(line);
        }

        public void LogWriteError(string line)
        {
            _log.WriteLine(line);
        }

        public void ConsoleWrite(string line)
        {
            Console.WriteLine(line);
        }

        public void ErrorWrite(string line)
        {
            Console.Error.WriteLine(line);
        }

        public void LogConsoleWrite(string line)
        {
            ConsoleWrite(line);
            LogWrite(line);
        }

        public void LogConsoleErrorWrite(string line)
        {
            ErrorWrite(line);
            LogWriteError(line);
        }

        public void Dispose() => ((IDisposable) _log).Dispose();

        private StreamWriter _log { get; }

        public BaseOutputEngine(IProject project)
        {
            Project = project;
            _log = new StreamWriter(Path.Combine(project.ProjectPath.FullName, "RoslynMacros.log"), false,
                Encoding.UTF8);
        }
    }
}