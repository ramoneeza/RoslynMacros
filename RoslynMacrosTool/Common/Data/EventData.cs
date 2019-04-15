using Microsoft.CodeAnalysis.CSharp.Syntax;
using RoslynMacros.Common.Classes;
using RoslynMacros.Common.Interfaces;

namespace RoslynMacros.Common.Data
{
    public class EventData : BaseData, IEventData
    {
        private EventDeclarationSyntax Event { get; }

        public EventData(EventDeclarationSyntax ev) : base(ev.Modifiers.ToString(), ev.Type.ToString(),
            ev.Identifier.ToString(), null, ev.AttributeLists)
        {
            Event = ev;
        }
    }
}