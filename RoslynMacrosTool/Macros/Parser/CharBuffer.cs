using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace RoslynMacros.Parser
{
    internal abstract class Buffer<T>
    {
        protected abstract T EOFSymbol { get; }
        protected abstract bool IsEof(T item);
        protected T[] buffer;
        public int Pos { get; private set; }
        public virtual bool EOF => IsEof(buffer[Pos]);

        public T Peek() => buffer[Pos];

        public virtual T Pop()
        {
            var c = buffer[Pos];
            Pos++;
            return c;
        }

        public (T, T) Peek2() => (buffer[Pos], buffer[Pos + 1]);
        public (T, T, T) Peek3() => (buffer[Pos], buffer[Pos + 1], buffer[Pos + 2]);

        public (T, T) Pop2()
        {
            return (Pop(), Pop());
        }

        public (T, T, T) Pop3()
        {
            return (Pop(), Pop(), Pop());
        }

        public IEnumerable<T> PopWhile(Func<T, bool> fn)
        {
            var res = new List<T>();
            var p = Peek();
            while (!IsEof(p) && fn(Peek()))
            {
                res.Add(Pop());
                p = Peek();
            }

            return res;
        }

        public bool PopUntilPop(Func<T, bool> fn, out IEnumerable<T> res)
        {
            res = PopUntil(fn);
            var p = Peek();
            if (fn(p))
            {
                res.Append(Pop());
                return true;
            }

            return false;
        }


        public IEnumerable<T> PopWhile(params T[] elements)
        {
            if ((elements?.Length ?? 0) == 0) return new T[0];
            return PopWhile(elements.Contains);
        }

        public IEnumerable<T> PopUntil(Func<T, bool> fn)
        {
            var res = new List<T>();
            var p = Peek();
            while (!IsEof(p) && !fn(Peek()))
            {
                res.Add(Pop());
                p = Peek();
            }

            return res;
        }

        [SuppressMessage("ReSharper", "VirtualMemberCallInConstructor")]
        protected Buffer(IEnumerable<T> line)
        {
            buffer = line.Concat(new[] {EOFSymbol, EOFSymbol, EOFSymbol}).ToArray();
            Pos = 0;
        }
    }

    internal class CharBuffer : Buffer<char>
    {
        public int CurrentLine { get; private set; } = 0;
        public static readonly char[] Blanks = {' ', '\t'};
        public static readonly char[] EolSymbols = {'\n', '\r'};

        public static readonly string[] Symbols =
            {"//", "/*", "*/", "==", ">=", "<=", "=>", "||", "&&", "#+", "+#", "@(", "@{", "\\\""};

        protected override char EOFSymbol => '\0';
        protected override bool IsEof(char item) => item == EOFSymbol;

        public string Peek2Str() => new string(new[] {buffer[Pos], buffer[Pos + 1]});
        public string Peek3Str() => new string(new[] {buffer[Pos], buffer[Pos + 1], buffer[Pos + 2]});

        public string Pop2Str()
        {
            return new string(new[] {Pop(), Pop()});
        }

        public string PopWhileStr(Func<char, bool> fn)
        {
            var b = PopWhile(fn);
            return new string(b.ToArray());
        }

        public string PopWhileStr(params char[] elements)
        {
            var b = PopWhile(elements);
            return new string(b.ToArray());
        }

        public string PopBlanks()
        {
            return PopWhileStr(Blanks);
        }

        public string PopSymbol()
        {
            var cad = Peek2Str();
            if (Symbols.Contains(cad)) return Pop2Str();
            return Pop().ToString();
        }

        public string PopEOL()
        {
            return PopWhileStr(EolSymbols);
        }

        public string PopLiteral()
        {
            return PopWhileStr(c => char.IsLetterOrDigit(c) || (c == '_'));
        }

        public string PopUntilStr(Func<char, bool> fn)
        {
            return new string(PopUntil(fn).ToArray());
        }

        public CharBuffer(params IEnumerable<char>[] lines) : base(lines.SelectMany(l => l))
        {
        }

        public override char Pop()
        {
            var p=base.Pop();
            if (p == '\n') CurrentLine++;
            return p;
        }
    }
}