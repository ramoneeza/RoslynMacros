using System.Collections.Generic;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using RoslynMacros.Common.Data;
using RoslynMacros.Common.Interfaces;

namespace RoslynMacros.Common.Classes
{
    public abstract class BaseData : IBaseData
    {
        public string MODIFIER { get; }
        public string RETURNTYPE { get; }
        public string NAME { get; }
        public string WIDENAME { get; }
        public string[] GENERIC { get; }
        public IAttrData[] AttributesData { get; }

        public string[] AttibutesLists { get; }

        protected BaseData(string modifier, string returnType, string name, string[] generic,
            IEnumerable<AttributeListSyntax> attls)
        {
            MODIFIER = modifier;
            RETURNTYPE = returnType;
            NAME = name;
            GENERIC = generic ?? new string[0];
            WIDENAME = (GENERIC.Length == 0) ? NAME : $"{NAME}<{string.Join(",", GENERIC)}>";
            if (attls == null)
            {
                AttributesData = new IAttrData[0];
                AttibutesLists = new string[0];
            }
            else
            {
                var att = new List<IAttrData>();
                var lst = new List<string>();
                foreach (var lista in attls)
                {
                    lst.Add(lista.ToString());
                    foreach (var a in lista.Attributes) att.Add(new AttrData(a));
                }

                AttributesData = att.ToArray();
                AttibutesLists = lst.ToArray();
            }
        }
    }
}