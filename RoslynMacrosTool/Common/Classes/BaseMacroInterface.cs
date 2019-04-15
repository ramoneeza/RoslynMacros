using System.Linq;
using LightInject;
using RoslynMacros.Common.Interfaces;
using RoslynMacros.Common.Variables;
using RoslynMacros.Common.WalkerResults;
using RoslynMacros.Common.Walkers;

namespace RoslynMacros.Common.Classes
{
    public abstract class BaseMacroExecuteInterface : BaseMacroExecuteType
    {
        public override void ExecuteScript()
        {
            var walker = new InterfaceWithAttributeWalker(Engine, AttributeName);
            // Visit syntaxtrees

            var sts = Engine.GetSyntaxTrees(Files);
            foreach (var st in sts) walker.Visit(st);

            // Execute x results
            var interf = walker.Results.Where(s => s.Attribute.GetArgument(0).type != ArgumentType.Null)
                .OrderBy(s => s.Attribute.GetStrippedArgument(0));
            foreach (var a in interf)
                if (!ExecutePerInterface(a))
                    return; // Return if error on macro compilation
            // End by macrotype
        }

        protected virtual bool ExecutePerInterface(InterfaceWithAttributeResult a)
        {
            // Load Macro
            var macroname = GetMacroName(a.TypeName, a.Attribute, out var args, out var outputname);
            var macro = MacroFactory.GetMacro<MacroTypeVariables>(macroname, a.FsPath, out var errores);

            // Check for errors in macro
            if (macro == null)
            {
                OutputEngine.LogConsoleErrorWrite($"{a.TypeName} : {macroname}");
                foreach (var e in errores) OutputEngine.LogConsoleErrorWrite(e);
                return false;
            }

            // Execute macro
            var refinterface = args[0].StartsWith("I") ? args[0] : "";
            var refdata = Engine.GetRecordForSymbol(refinterface);
            var variables = CreateVariables(a, outputname, a.AttributeList, refdata);
            var res = macro.Execute(variables, out var error);

            // Check for errors
            if (!res) OutputEngine.LogConsoleErrorWrite(error);

            // Write Results 

            CurrentProject.WriteResults(variables);
            return res;
        }


        protected BaseMacroExecuteInterface(IDataEngine dataengine,IMacroFactory macroFactory,IOutputEngine outputengine,IProject project,IConfiguration configuration) : base(dataengine,macroFactory,outputengine,project,configuration)
        {
        }
    }
}