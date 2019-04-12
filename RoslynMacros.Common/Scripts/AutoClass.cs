using LightInject;
using RoslynMacros.Common.Classes;
using RoslynMacros.Common.Interfaces;

namespace RoslynMacros.Common.Scripts
{
    public class AutoClass : BaseMacroExecuteClass
    {
        public override string Name => "AutoClass";

        public override string AttributeName => "AutoClass";

        public override string Prefix => "C.";

        public AutoClass(IDataEngine dataengine,IMacroFactory macroFactory,IOutputEngine outputengine,IProject project,IConfiguration configuration) : base(dataengine,macroFactory,outputengine,project,configuration)
        {
        }
    }
}