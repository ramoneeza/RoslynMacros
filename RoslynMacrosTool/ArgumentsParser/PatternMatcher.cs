using System;
using System.IO;
using System.Reflection;

namespace RoslynMacros.ArgumentsParser
{
    public static class PatternMatcher
    {
        private static readonly MethodInfo _strictMatchPattern;

        public static bool StrictMatchPattern(string name, string mask)
        {
            return (bool)_strictMatchPattern.Invoke(null,new object[]{mask, name});
        }
        static PatternMatcher()
        {
            Type patternMatcherType = typeof(FileSystemWatcher).Assembly.GetType("System.IO.PatternMatcher");
            _strictMatchPattern= patternMatcherType.GetMethod("StrictMatchPattern", System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.Public);
        }
    }
}