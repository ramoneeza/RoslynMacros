using System;
using System.IO;
using System.Linq;
using System.Reflection;

namespace RoslynMacros.ArgumentsParser
{
    public class FilesArgumentAttribute : AbsArgumentAttribute
    {
        private static readonly Lazy<MethodInfo> _internalFileMatch=new Lazy<MethodInfo>(() =>
        {
            Type patternMatcherType = typeof(FileSystemWatcher).Assembly.GetType("System.IO.PatternMatcher");
            return  patternMatcherType.GetMethod("StrictMatchPattern", System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.Public);
        });

        public static bool FileMatch(string mask,string file)
        {
            return (bool)_internalFileMatch.Value.Invoke(null, new object[] {mask, file});
        }
        public string Mask { get; }
        public FilesArgumentAttribute(char c, string prefix,string description="",bool optional=true,string mask=""):base(c,prefix,description,optional,true)
        {
            Mask = mask;
        }

        public override object Convert(string[] values)
        {
            if (Mask == ""|| values.All(v =>FileMatch(Mask,v))) return values;
            return null;
        }

        public override object DefaultValue()
        {
            return new string[0];
        }
    }
}