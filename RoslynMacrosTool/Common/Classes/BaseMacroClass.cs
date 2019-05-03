using System.Linq;
using LightInject;
using RoslynMacros.Common.Data;
using RoslynMacros.Common.Interfaces;
using RoslynMacros.Common.Variables;
using RoslynMacros.Common.WalkerResults;
using RoslynMacros.Common.Walkers;

namespace RoslynMacros.Common.Classes
{
    public abstract class BaseMacroExecuteType : BaseMacroExecute<IMacroTypeVariables>
    {
        protected BaseMacroExecuteType(IDataEngine dataengine,IMacroFactory macroFactory,IOutputEngine outputengine,IProject project,IConfiguration configuration) : base(dataengine,macroFactory,outputengine,project)
        {
        }

        protected virtual MacroTypeVariables CreateVariables(IWalkerTypeResult result, string output,
            AttributeParser[] attributeList, ITypeData reftype)
        {
            return new MacroTypeVariables(Engine, result, output, attributeList, reftype);
        }
    }

    public abstract class BaseMacroExecuteClass : BaseMacroExecuteType
    {
        protected BaseMacroExecuteClass(IDataEngine dataengine,IMacroFactory macroFactory,IOutputEngine outputengine,IProject project,IConfiguration configuration) : base(dataengine,macroFactory,outputengine,project,configuration)
        {
        }

        public override void ExecuteScript()
        {
            var walker = new ClassWithAttributeWalker(Engine, AttributeName);

            // Visit syntaxtrees
            var sts = Engine.GetSyntaxTrees(Files);
            foreach (var st in sts) walker.Visit(st);

            // Execute x results
            var classes = walker.Results.Where(s => s.Attribute.GetArgument(0).type != ArgumentType.Null)
                .OrderBy(s => s.Attribute.GetStrippedArgument(0));
            foreach (var a in classes)
                if (!ExecutePerClass(a))
                    return; // Return if error on macro compilation
            // End by macrotype
        }

        private bool ExecutePerClass(ClassWithAttributeResult a)
        {
            // Load Macro
            var macroname = GetMacroName(a.ClassName, a.Attribute, out var args, out var outputname);
            var macro = MacroFactory.GetMacro<MacroTypeVariables>(macroname, a.FsPath, out var errores);

            // Check for errors in macro
            if (macro == null)
            {
                OutputEngine.LogConsoleErrorWrite($"{a.ClassName} : {macroname}");
                foreach (var e in errores) OutputEngine.LogConsoleErrorWrite(e);
                return false;
            }

            // Execute macro
            //var refinterface = args[0].StartsWith("I") ? args[0] : "";
            var reftype = args[0]?? "";
            var refdata = Engine.GetRecordForSymbol(reftype);
            if (refdata == null)
            {
                OutputEngine.ConsoleWrite($"{reftype} is defined outside project. @REFINTERFACE/@REFCLASS will be null.");
            }
            var variables = CreateVariables(a, outputname, a.AttributeList, refdata);
            var res = macro.Execute(variables, out var error);

            // Check for errors
            if (!res) OutputEngine.LogConsoleErrorWrite(error);

            // Write Results 

            CurrentProject.WriteResults(variables);
            return res;
        }
    }
}