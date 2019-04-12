using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using RoslynMacros.Common.Classes;
using RoslynMacros.Common.Interfaces;

namespace RoslynMacros.Common.Data
{
    public class PropertyData : BaseData, IPropertyData
    {
        public bool HASGETTER { get; }
        public bool HASSETTER { get; }
        public bool ISVIRTUAL { get; }
        public string MODIFIERGETTER { get; } = "";
        public string MODIFIERSETTER { get; } = "";
        public PropertyDeclarationSyntax Prop { get; }

        public bool DECO(params string[] attributename)
        {
            return AttributesData.Any(a => attributename.Contains(a.NAME, StringComparer.OrdinalIgnoreCase));
        }

        public string DECOVALUE(string attributename)
        {
            return DECOVALUERAW(attributename).StripQuote();
        }
        public string DECOVALUERAW(string attributename)
        {
            return AttributesData
                       .FirstOrDefault(a =>
                           string.Compare(a.NAME, attributename, StringComparison.OrdinalIgnoreCase) == 0)?.Parameters
                       ?.FirstOrDefault() ?? "";
        }

        public IEnumerable<string> ATTRIBUTES => AttributesData.Select(a => a.NAME);

        public IEnumerable<string> ATTRIBUTESIN(params string[] atts) =>
            AttributesData.Select(a => a.NAME).Where(a => atts.Contains(a));

        public PropertyData(PropertyDeclarationSyntax prop) : base(prop.Modifiers.ToString(), prop.Type.ToString(),
            prop.Identifier.ToString(), null, prop.AttributeLists)
        {
            Prop = prop;
            ISVIRTUAL = prop.Modifiers.Any(a =>
                a.Kind() == SyntaxKind.AbstractKeyword || a.Kind() == SyntaxKind.VirtualKeyword);
            if (prop.AccessorList != null)
            {
                var al = prop.AccessorList.Accessors;
                var getter = al.FirstOrDefault(a => a.Kind() == SyntaxKind.GetAccessorDeclaration);
                var setter = al.FirstOrDefault(a => a.Kind() == SyntaxKind.SetAccessorDeclaration);
                if (getter != null)
                {
                    HASGETTER = true;
                    MODIFIERGETTER = getter.Modifiers.ToString();
                }

                if (setter != null)
                {
                    HASSETTER = true;
                    MODIFIERSETTER = setter.Modifiers.ToString();
                }
            }
            else
            {
                HASGETTER = true;
                MODIFIERGETTER = "";
            }
        }
    }
}