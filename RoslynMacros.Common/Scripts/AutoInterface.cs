using LightInject;
using RoslynMacros.Common.Classes;
using RoslynMacros.Common.Interfaces;

namespace RoslynMacros.Common.Scripts
{
    public class AutoInterface : BaseMacroExecuteInterface
    {
        public override string Name => "AutoInterface";
        public override string AttributeName => "AutoInterface";
        public override string Prefix => "";

        public AutoInterface(IDataEngine dataengine,IMacroFactory macroFactory,IOutputEngine outputengine,IProject project,IConfiguration configuration) : base(dataengine,macroFactory,outputengine,project,configuration)
        {
        }
    }
}