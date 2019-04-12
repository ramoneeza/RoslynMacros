using LightInject;
using RoslynMacros.Common.Data;
using RoslynMacros.Common.Interfaces;

namespace RoslynMacros.Common.Classes
{
    public abstract class BaseMacroExecute<T> : IMacroExecute where T : IVariables
    {
        public abstract string Name { get; }
        public abstract string AttributeName { get; }
        public abstract string Prefix { get; }
        public abstract void ExecuteScript();

        protected virtual string GetMacroName(string typename, AttributeParser att, out string[] args,
            out string outputname)
        {
            var (typea0, macroname) = att.GetArgument(0);
            switch (typea0)
            {
                case ArgumentType.Null:
                    args = new[] {att.GetStrippedArgument(0), att.GetStrippedArgument(1), att.GetStrippedArgument(2)};
                    macroname = "Any";
                    break;
                case ArgumentType.String:
                    args = new[] {att.GetStrippedArgument(1), att.GetStrippedArgument(2), att.GetStrippedArgument(3)};
                    break;
                default:
                    args = new[] {att.GetStrippedArgument(0), att.GetStrippedArgument(1), att.GetStrippedArgument(2)};
                    break;
            }

            if (!macroname.EndsWith(".csmacro")) macroname = $"{AttributeName}.{macroname}.csmacro";

            if (string.IsNullOrEmpty(args[0])) args[0] = att.Name;
            // ReSharper disable once InvokeAsExtensionMethod
            outputname = RegexHelper.NoInvalidChars($"{typename}.{args[0]}.{Prefix}Auto.cs");
            return macroname;
        }

        protected BaseMacroExecute(IDataEngine dataengine,IMacroFactory macroFactory,IOutputEngine outputengine,IProject project)
        {
            Engine = dataengine;
            MacroFactory = macroFactory;
            OutputEngine = outputengine;
            CurrentProject = project;
           
        }

        public IDataEngine Engine { get; }
        public IMacroFactory MacroFactory { get; }
        public IOutputEngine OutputEngine { get; }
        public IProject CurrentProject { get; }
       
        protected string[] Files => CurrentProject.Configuration.Files;
    }
}