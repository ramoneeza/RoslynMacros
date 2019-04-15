using LightInject;
using RoslynMacros.Common.Classes;
using RoslynMacros.Common.Interfaces;

namespace RoslynMacros.Common.Scripts
{
    public class AutoImplement : BaseMacroExecuteClass
    {
        public override string Name => nameof(AutoImplement);
        public override string AttributeName => "AutoImplement";
        public override string Prefix => "";

        public AutoImplement(IDataEngine dataengine,IMacroFactory macroFactory,IOutputEngine outputengine,IProject project,IConfiguration configuration) : base(dataengine,macroFactory,outputengine,project,configuration)
        {
        }
    }
}