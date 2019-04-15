using System;
using System.Collections.Generic;
using System.IO;
using JetBrains.Annotations;
using RoslynMacros.Common.Interfaces;

namespace RoslynMacros.Macros
{
    [PublicAPI]
    public class MacroFactory : IMacroFactory
    {
        private readonly Dictionary<string, IMacro> _macros =
            new Dictionary<string, IMacro>(StringComparer.InvariantCultureIgnoreCase);

        public IProject Project { get; }
        public IOutputEngine OutputEngine { get; }
        public MacroFactory(IProject project,IOutputEngine outputEngine)
        {
            Project = project;
            OutputEngine = outputEngine;
        }

        public IMacro<T> GetMacro<T>(string fe, DirectoryInfo alternativepath, out string[] error) where T : IVariables
        {
            error = new string[0];
            if (!fe.EndsWith(".csmacro")) fe += ".csmacro";
            if (!_macros.ContainsKey(fe))
            {
                var pathfe = Path.Combine(Project.ProjectPath.FullName, fe);
                var pathfe2 = Path.Combine((alternativepath?.FullName) ?? Project.ProjectPath.FullName, fe);
                var pathfe3 = Path.Combine(Project.ProjectPath + "\\macros", fe);

                if (!File.Exists(pathfe)) pathfe = pathfe2;
                if (!File.Exists(pathfe)) pathfe = pathfe3;
                if (!File.Exists(pathfe))
                {
                    error = new[] {$"Macro not found:{fe}"};
                    return null;
                }

                var code = File.ReadAllText(pathfe);
                var m = new Macro<T>(fe, code);
                if (m.AreErrors)
                {
                    error = m.Errors;
                    foreach (var e in error)
                    {
                        OutputEngine.LogConsoleErrorWrite(e);
                    }

                    return null;
                }

                _macros.Add(fe, m);
            }

            var macrot = _macros[fe] as Macro<T>;
            if (macrot == null) error = new[] {$"Error Macro needs variables of type {typeof(T)}"};
            return macrot;
        }
    }
}