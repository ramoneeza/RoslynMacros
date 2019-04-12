using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LightInject;
using RoslynMacros.Common;
using RoslynMacros.Common.Classes;
using RoslynMacros.Common.Interfaces;
using RoslynMacros.Common.Variables;
using RoslynMacros.Common.WalkerResults;
using RoslynMacros.Common.Walkers;


namespace RoslynMacros.Scripts
{
    public class AutoConstructor : BaseMacroExecuteType
    {
        public override string Name => nameof(AutoConstructor);
        public override string AttributeName => "AutoConstructor";
        public override string Prefix => "";
        public AutoConstructor(IDataEngine dataengine,IMacroFactory macroFactory,IOutputEngine outputengine,IProject project,IConfiguration configuration) : base(dataengine,macroFactory,outputengine,project,configuration)
        {
        }

        public override void ExecuteScript()
        {
            var walker = new InterfaceWithAttributeWalker(Engine,AttributeName);
            // Visit syntaxtrees

            var sts=Engine.GetSyntaxTrees(Files);
            foreach(var st in sts) walker.Visit(st);

            // Execute x results
            var interf = walker.Results.Where(s => s.Attribute.GetArgument(0).type!=ArgumentType.Null).OrderBy(s => s.Attribute.GetStrippedArgument(0));
            foreach (var a in interf)
            {
                if (!ExecutePerInterface(a))
                    return;// Return if error on macro compilation
            }
            // End by macrotype
        }

        protected virtual bool ExecutePerInterface(InterfaceWithAttributeResult a)
        {
            var walker2 = new ClassImplementsInterfaceWalker(Engine, a.TypeName);
            // Visit syntaxtrees

            var sts = Engine.GetSyntaxTrees(Files);
            foreach (var st in sts) walker2.Visit(st);
            foreach (var cl in walker2.Results)
            {
                if (!ExecutePerClass(cl, a))
                    return false; // Return if error on macro compilation
            }

            return true;
        }
        protected virtual bool ExecutePerClass(ClassResult cl,InterfaceWithAttributeResult i)
        {
            // Load Macro
            var macroname = GetMacroName(cl.TypeName,i.Attribute, out var args, out var outputname);
            var macro = MacroFactory.GetMacro<MacroTypeVariables>(macroname,cl.FsPath, out var errores);
            // Check for errors in macro
            if (macro == null)
            {
                OutputEngine.LogConsoleErrorWrite($"{cl.TypeName} : {macroname}");
                foreach (var e in errores)OutputEngine.LogConsoleErrorWrite(e);
                return false;
            }

            // Execute macro
            var refinterface = args[0].StartsWith("I") ? args[0] : "";
            var refdata = Engine.GetRecordForSymbol(refinterface);
            MacroTypeVariables variables = CreateVariables(cl,outputname,i.AttributeList,refdata);
            var res = macro.Execute(variables, out var error);

            // Check for errors
            if (!res) OutputEngine.LogConsoleErrorWrite(error);

            // Write Results 

            CurrentProject.WriteResults(variables);
            return res;
        }



    }
   
}
