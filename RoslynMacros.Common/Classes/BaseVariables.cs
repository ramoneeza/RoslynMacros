using System;
using System.IO;
using System.Text;
using RoslynMacros.Common.Data;
using RoslynMacros.Common.Interfaces;

namespace RoslynMacros.Common.Classes
{
    public abstract partial class BaseVariables : IVariables
    {
        public FileInfo Filename { get; }
        public string FILENAME => Filename.Name;
        public string PATH => Filename.DirectoryName;

        public string OUTPUT { get; set; }

        public StringBuilder Output { get; } = new StringBuilder();
        public StringBuilder Log { get; } = new StringBuilder();

        public FlagVar Flags { get; } = new FlagVar();
        public CollectionVar Includes { get; } = new CollectionVar();
        public CollectionVar Excludes { get; } = new CollectionVar();
        public CollectionStr Variables { get; } = new CollectionStr();
        public CollectionInt Values { get; } = new CollectionInt();

        public BaseVariables(IWalkerResult result, string output, AttributeParser[] attributes)
        {
            Filename=result.FilePath;
            OUTPUT = output;
            var flags = GetMacroVar("MacroFlags", attributes);
            var vars = GetMacroVar("MacroVars", attributes);
            var values = GetMacroVar("MacroValues", attributes);
            var includes = GetMacroVar("MacroInclude", attributes);
            var excludes = GetMacroVar("MacroExclude", attributes);
            Flags.AddBulk(flags);
            foreach (var varline in vars)
            {
                var v = varline.StripQuote();
                if (String.IsNullOrEmpty(v)) continue;
                var (vari, value) = v.GetAsign();
                if (!String.IsNullOrEmpty(vari)) Variables.Set(vari, value);
            }

            foreach (var varline in values)
            {
                var v = varline.StripQuote();
                if (String.IsNullOrEmpty(v)) continue;
                var (vari, value) = v.GetAsign();
                if (String.IsNullOrEmpty(vari)) continue;
                if (!Int32.TryParse(value, out var i)) continue;
                Values.Set(vari, i);
            }

            foreach (var varline in includes)
            {
                var v = varline.StripQuote();
                if (String.IsNullOrEmpty(v)) continue;
                Includes.Add(v);
            }

            foreach (var varline in excludes)
            {
                var v = varline.StripQuote();
                if (String.IsNullOrEmpty(v)) continue;
                Excludes.Add(v);
            }
        }

        public void Echo(string cad)
        {
            Console.WriteLine(cad);
        }
    }
}