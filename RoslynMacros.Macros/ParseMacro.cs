using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using JetBrains.Annotations;
using Microsoft.CodeAnalysis.CSharp.Scripting;
using Microsoft.CodeAnalysis.Scripting;
using RoslynMacros.Common.Data;
using RoslynMacros.Common.Interfaces;
using RoslynMacros.CompileMacro;
using RoslynMacros.Parser;

namespace RoslynMacros
{
    [PublicAPI]
    public class Macro<T> : IMacro<T> where T : IVariables
    {
        public string Name { get; }
        public string Compiled { get; }
        protected ScriptRunner<bool> MacroDelegate { get; private set; }
        public bool AreErrors { get; }
        public string[] Errors { get; } = new string[0];

        protected IEnumerable<Assembly> GetReferences()
        {
            return new List<Assembly>
            {
                Assembly.GetAssembly(typeof(IMacroTypeVariables)),
                Assembly.GetAssembly(typeof(T)),
                Assembly.GetAssembly(typeof(FlagVar)),
                Assembly.GetAssembly(typeof(IPropertyData)),
                Assembly.GetAssembly(typeof(List<>)),
                Assembly.GetAssembly(typeof(Enumerable)),
                Assembly.GetAssembly(typeof(System.Runtime.CompilerServices.DynamicAttribute)),
                Assembly.GetAssembly(typeof(Microsoft.CSharp.RuntimeBinder.Binder)),
                Assembly.GetAssembly(typeof(StringBuilder))
            };
        }

        protected IEnumerable<string> GetImports()
        {
            return new[]
            {
                "System", "System.Linq", "System.Collections.Generic", "System.Text",
                "RoslynMacros.Common", "RoslynMacros.Common.Classes", "RoslynMacros.Common.Data",
                "RoslynMacros.Common.Interfaces",
                "RoslynMacros.Common.Variables"
            };
        }

        public Macro(string name, string macro)
        {
            Name = name;
            AreErrors = false;
            var compiler = new CompileCsMacro();
            Compiled = compiler.Compile(macro);
            AreErrors = compiler.Error != "";
            MacroDelegate = null;
            if (AreErrors)
            {
                Errors = compiler.Tokens.Where(t => t.TokenType == TokenType.Error).Select(t => t.Value).Append("--")
                    .Append(compiler.Error).ToArray();
                return;
            }

            var options = ScriptOptions.Default
                .AddReferences(GetReferences().ToArray())
                .AddImports(GetImports().ToArray());
            var script = CSharpScript.Create<bool>(Compiled, globalsType: typeof(T), options: options);
            try
            {
                MacroDelegate = script.CreateDelegate();
            }
            catch (CompilationErrorException cex)
            {
                var compiledlines = Compiled.Split('\n');
                var er = new List<string>();
                er.Add("Macro C# syntax error:");
                er.Add(cex.Message);
                foreach (var d in cex.Diagnostics)
                {
                    er.Add(d.GetMessage());
                    var lp = d.Location.GetLineSpan();
                    var lines = compiledlines.Skip(lp.StartLinePosition.Line)
                        .Take(lp.EndLinePosition.Line - lp.StartLinePosition.Line+1);
                    er.AddRange(lines);
                }
                
                Errors = er.ToArray();
                throw new Exception(string.Join(Environment.NewLine,Errors));
            }
            catch(Exception ex)
            {
                var er = new List<string>();
                er.Add("Macro C# syntax error:");
                er.Add(ex.Message);
                var ie = ex.InnerException;
                while (ie != null)
                {
                    er.Add(ie.Message);
                    ie = ie.InnerException;
                }
                Errors = er.ToArray();
                throw new Exception(string.Join(Environment.NewLine,Errors));
            }
        }

        public bool Execute(T variables, out string error)
        {
            try
            {
                error = "";
                return MacroDelegate(variables).Result;
            }
            catch (Exception e)
            {
                error = e.Message;
                return false;
            }
        }
    }
}