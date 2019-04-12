using System.Security.Cryptography;
using System.Text;
using RoslynMacros.ArgumentsParser;

// ReSharper disable UnusedAutoPropertyAccessor.Global

namespace RoslynMacros
{
    public class Arguments
    {
        [FilesArgument('f', "file","<.cs File, ...>",true,"*.cs")]
        public string[] Files { get; set; }
        [StringsArgument('s', "script", "<Script to run, ...>", false)]
        public string[] Scripts { get; set; }
        [BoolArgument('v', "verbose", "Verbose Output")]
        public bool Verbose { get; set; }
    }
}
