using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace RoslynMacros.Common
{
    public enum ArgumentType
    {
        Null,
        Type,
        Name,
        String,
        Value
    }

    public static class RegexHelper
    {
        private static readonly Regex RStripPar = new Regex(@"^\s*\((.*)\)\s*$", RegexOptions.Compiled);
        private static readonly Regex RStripQuote = new Regex(@"^\s*\""(.*)\""\s*$", RegexOptions.Compiled);
        private static readonly Regex RStripTypeof = new Regex(@"^\s*typeof\((.*)\)\s*$", RegexOptions.Compiled);
        private static readonly Regex RStripNameof = new Regex(@"^\s*nameof\((.*)\)\s*$", RegexOptions.Compiled);

        private static readonly Regex RAsign =
            new Regex(@"^\s*(?<left>[^\s\=]+)\s*\=\s*(?<right>[^\s]+|\""[^\""]*\"")\s*$", RegexOptions.Compiled);

        private static string StripBase(this string cad, Regex pattern)
        {
            cad = cad ?? "";
            var m = pattern.Match(cad);
            if (!m.Success) return cad;
            return m.Groups[1].Value;
        }

        private static string QuitEscape(this string cad)
        {
            return cad.Replace("\\\"", "\"").Replace("\\\\", "\\");
        }


        public static string StripPar(this string cad) => QuitEscape(StripBase(cad, RStripPar));
        public static string StripQuote(this string cad) => QuitEscape(StripBase(cad, RStripQuote));

        // ReSharper disable once InconsistentNaming
        public static string StripPOrQ(this string cad) => StripQuote(StripBase(cad, RStripPar));

        public static string StripTypeOf(this string cad) => StripBase(cad, RStripTypeof);
        public static string StripNameOf(this string cad) => StripBase(cad, RStripNameof);
        public static string StripArgument(this string cad) => StripTypeOf(StripPOrQ(cad));
        public static string StripArgument2(this string cad) => StripNameOf(StripPOrQ(cad));

        public static (ArgumentType type, string value) DecodeArgument(this string cad)
        {
            if (string.IsNullOrWhiteSpace(cad)) return (ArgumentType.Null, "");
            if (cad.StartsWith("typeof(")) return (ArgumentType.Type, StripArgument(cad));
            if (cad.StartsWith("nameof(")) return (ArgumentType.Name, StripArgument2(cad));
            if (cad.StartsWith("\"")) return (ArgumentType.String, StripArgument(cad));
            return (ArgumentType.Value, StripArgument(cad));
        }

        private static readonly char[] InvalidFileChars = Path.GetInvalidFileNameChars();

        public static string NoInvalidChars(this string cad) =>
            new string(cad.Select(c => InvalidFileChars.Contains(c) ? '_' : c).ToArray());

        public static string[] GetArguments(this string cad)
        {
            var i = 0;
            return GetList(ref cad, ref i);
        }

        private static string[] GetList(ref string cad, ref int i)
        {
            if (String.IsNullOrEmpty(cad) || (cad[i] != '(')) return null;
            var res = new List<string>();
            i++;
            var start = i;

            while ((i < cad.Length))
            {
                var c = cad[i];
                switch (c)
                {
                    case '(':

                        var rl = GetList(ref cad, ref i);
                        if (rl == null) return null;
                        //res.Add("(" + string.Join(",", rl) + ")");
                        break;
                    case ')':
                        var p = cad.Substring(start, i - start);
                        res.Add(p);
                        return res.ToArray();
                    case '"':
                        var lt = GetLit(ref cad, ref i);
                        if (lt == null) return null;
                        res.Add(lt);
                        break;
                    case ',':
                        var p2 = cad.Substring(start, i - start);
                        res.Add(p2);
                        start = i + 1;
                        break;
                }

                i++;
            }

            return null;
        }

        private static string GetLit(ref string cad, ref int i)
        {
            if (cad[i] != '"') return null;
            var start = i;
            i++;
            while (i < cad.Length - 1)
            {
                if (cad[i] == '"') return cad.Substring(start, i - start + 1);
                i++;
            }

            return null;
        }

        public static (string, string) GetAsign(this string cad)
        {
            var r = RAsign.Match(cad ?? "");
            if (!r.Success) return ("", "");
            return (r.Groups["left"].Value, r.Groups["right"].Value);
        }

        public static string ReplaceAll(string cad, params (string, string)[] replacements)
        {
            foreach (var r in replacements) cad = cad.Replace(r.Item1, r.Item2);

            return cad;
        }


        public static string[] GetGenerics(this TypeParameterListSyntax tpl)
        {
            if ((tpl == null) || (tpl.Parameters.Count == 0)) return new string[0];
            return tpl.Parameters.Select(p => p.ToString()).ToArray();
        }

        public static string WideName(this TypeDeclarationSyntax type)
        {
            var name = type.Identifier.ToString();
            var tpl = type.TypeParameterList;
            if (tpl == null) return name;
            if (tpl.Parameters.Count == 0) return name;
            var usings = tpl.GetGenerics();
            return $"{name}<{string.Join(",", tpl)}>";
        }
    }
}