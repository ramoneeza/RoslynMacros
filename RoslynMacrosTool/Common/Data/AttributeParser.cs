using System.Linq;
using JetBrains.Annotations;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace RoslynMacros.Common.Data
{
    [PublicAPI]
    public class AttributeParser
    {
        public AttributeSyntax Attribute { get; }
        public string Name { get; }
        public string[] Arguments { get; }

        public AttributeParser(AttributeSyntax attribute)
        {
            Attribute = attribute;
            Name = attribute.Name.ToString();
            Arguments = (attribute.ArgumentList == null)
                ? new string[0]
                : attribute.ArgumentList.Arguments.Select(a => a.ToString()).ToArray();
        }

        public string GetStrippedArgument(int i)
        {
            var arg = (i < Arguments.Length) ? Arguments[i] : "";
            return arg.StripArgument();
        }

        public (ArgumentType type, string value) GetArgument(int i)
        {
            var arg = (i < Arguments.Length) ? Arguments[i] : "";
            return arg.DecodeArgument();
        }
    }
}