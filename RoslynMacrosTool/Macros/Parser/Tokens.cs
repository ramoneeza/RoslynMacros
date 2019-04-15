using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LightInject;

namespace RoslynMacros.Parser
{
    public enum TokenType
    {
        // TOKEN0
        Blank,
        NewLine,
        Symbol,
        Literal,
        Variable,
        Expression,
        Quote,
        Pure,
        Error,
        EOF,

        // TOKEN1
        SubMacro,
        SuperComment,
        MultiComment,
        Preprocessor
    }

    public static class TokenParser
    {
        public static IList<Token> Parse(string data)
        {
            return Parse1(Parse0(data));
        }

        private static IList<Token> Parse0(string data)
        {
            var res = new List<Token>();
            var buffer = data.EndsWith(Environment.NewLine)
                ? new CharBuffer(data)
                : new CharBuffer(data, Environment.NewLine);
            while (!buffer.EOF)
            {
                var p = buffer.Peek();
                if (CharBuffer.Blanks.Contains(p))
                {
                    var bl = buffer.PopBlanks();
                    res.Add(Token0.Blank(bl));
                    continue;
                }

                if (CharBuffer.EolSymbols.Contains(p))
                {
                    var bnl = buffer.PopEOL();
                    res.Add(Token0.NewLine());
                    continue;
                }

                if (p == '@')
                {
                    var (_, p2) = buffer.Peek2();
                    if (char.IsLetter(p2))
                    {
                        buffer.Pop();
                        var lv = buffer.PopLiteral();
                        res.Add(Token0.Variable("@" + lv));
                        continue;
                    }
                }

                if (char.IsLetterOrDigit(p) || p == '_')
                {
                    var l = buffer.PopLiteral();
                    res.Add(Token0.Literal(l));
                    continue;
                }

                var s = buffer.PopSymbol();
                res.Add(Token0.Symbol(s));
            }

            return res;
        }

        private static IList<Token> Parse1(IEnumerable<Token> data)
        {
            var res = new List<Token>();
            var buffer = new TokenBuffer(data);
            var queue = new Queue<Token>();

            void Frush()
            {
                if (queue.Count > 0) res.Add(Token1.Pure(queue));
                queue.Clear();
            }

            void FrushLine()
            {
                res.Add(Token1.PureLine(queue));
                queue.Clear();
            }

            while (!buffer.EOF)
            {
                var p = buffer.Peek();
                switch (p.TokenType)
                {
                    case TokenType.Symbol:
                        var token = ParseSymbol(buffer);

                        if (token == null)
                        {
                            queue.Enqueue(buffer.Pop());
                        }
                        else
                        {
                            Frush();
                            res.Add(token);
                        }

                        break;
                    case TokenType.NewLine:
                        buffer.Pop();
                        FrushLine();
                        continue;
                    case TokenType.Variable:
                        Frush();
                        res.Add(buffer.Pop());
                        break;
                    default:
                        queue.Enqueue(buffer.Pop());
                        break;
                }
            }

            if (queue.Count > 0)
                FrushLine();
            return res;
        }

        private static Token ParseSymbol(TokenBuffer buffer)
        {
            var (p, p2) = buffer.Peek2();
            if (p.Value == "#" && buffer.Col == 0 && p2.TokenType == TokenType.Literal)
                return ParsePreprocessor(buffer);
            if (p.Value == "//" && p2.Value == "!")
                return ParseSupperComment(buffer);
            if (p.Value == "/*")
                return ParseMultiComment(buffer);
            if (p.Value == "@{" && buffer.Col == 0)
                return ParseSubMacro(buffer);
            if (p.Value == "@(")
                return ParseExpression(buffer);
            return null;
        }

        private static Token ParseExpression(TokenBuffer buffer)
        {
            var nivel = 1;
            var tks = new List<Token>();
            tks.Add(buffer.Pop());
            while (!buffer.EOF && nivel > 0)
            {
                var p = buffer.PeekStr();
                if (p == "'")
                {
                    var p3 = buffer.Peek3Str();
                    if (p3 == "'('" || p3 == "')'") tks.Add(Token0.Literal(buffer.Pop3Str()));
                    continue;
                }

                if (p == "\"")
                {
                    var v = ParseQuote(buffer);
                    tks.Add(v);
                    continue;
                }

                if (p == "(") nivel++;
                if (p == ")") nivel--;
                tks.Add(buffer.Pop());
            }

            return Token1.Expression(tks);
        }

        private static Token ParseQuote(TokenBuffer buffer)
        {
            var res = new StringBuilder();
            var p = buffer.PopStr();
            if (p != "\"") throw new Exception("\" Expected");
            res.Append(p);
            var tks = buffer.PopWhile(t => t.Value != "\"").Select(t => t.Value);
            res.Append(string.Concat(tks));
            if (buffer.PeekStr() == "\"") res.Append(buffer.PopStr());
            return Token0.Quote(res.ToString());
        }

        private static Token ParseSubMacro(TokenBuffer buffer)
        {
            var nivel = 1;
            var res = new List<Token>();
            res.Add(buffer.Pop());
            if (buffer.Peek().TokenType == TokenType.NewLine) buffer.Pop();
            while (!buffer.EOF && nivel > 0)
            {
                var p = buffer.PeekStr();
                if (p == "'")
                {
                    var p3 = buffer.Peek3Str();
                    if (p3 == "'{'" || p3 == "'}'")
                    {
                        res.Add(buffer.Pop());
                        res.Add(buffer.Pop());
                        res.Add(buffer.Pop());
                    }
                }

                if (p == "\"")
                {
                    var v = ParseQuote(buffer);
                    res.Add(v);
                    continue;
                }

                if (p == "}") nivel--;
                if (p == "{") nivel++;
                res.Add(buffer.Pop());
            }

            // ReSharper disable once IteratorMethodResultIsIgnored
            buffer.PopWhile(t => t.TokenType == TokenType.Blank);
            if (buffer.Peek().TokenType == TokenType.NewLine) buffer.Pop();
            return Token1.SubMacro(res);
        }

        private static Token ParseSupperComment(TokenBuffer buffer)
        {
            var tks = buffer.PopUntil(t => t.TokenType == TokenType.NewLine);
            if (buffer.Peek().TokenType == TokenType.NewLine) buffer.Pop();
            return Token1.SuperComment(tks);
        }

        private static Token ParseMultiComment(TokenBuffer buffer)
        {
            if (!buffer.PopUntilPop(t => t.Value == "*/", out var tks)) return Token1.Error(tks);
            var p = buffer.Peek();
            if (p.TokenType == TokenType.NewLine) buffer.Pop();
            return Token1.MultiComment(tks);
        }

        private static Token ParsePreprocessor(TokenBuffer buffer)
        {
            var tks = buffer.PopUntil(t => t.TokenType == TokenType.NewLine);
            if (buffer.Peek().TokenType == TokenType.NewLine) buffer.Pop();

            return Token1.Preprocessor(tks);
        }
    }

    public abstract class Token
    {
        public bool HasEndLine { get; internal set; }
        public TokenType TokenType { get; private set; }
        public string Value { get; protected set; }
        public virtual string InnerValue() => Value;

        protected Token(TokenType type, string value, bool endline = false)
        {
            TokenType = type;
            Value = value;
            HasEndLine = endline;
        }

        public override string ToString()
        {
            return Value;
        }

        public static string Escape(string cad) => cad.Replace("\\", "\\\\").Replace("\"", "\\\"")
            .Replace(Environment.NewLine, Environment.NewLine.Length == 2 ? "\\r\\n" : "\\n");

        public virtual string Render() => Escape(InnerValue());

        public virtual string[] RenderLines()
        {
            var lines = InnerValue().Split(new[] {Environment.NewLine}, StringSplitOptions.None).Select(Escape);
            return lines.ToArray();
        }

        public static string ToString(IEnumerable<Token> tokens)
        {
            return string.Concat(tokens.Select(t => t.Value + (t.HasEndLine ? Environment.NewLine : "")));
        }

        public static string ToStringRenderVar(IEnumerable<Token> tokens)
        {
            return string.Concat(tokens.Select(t =>
                (t is Token0.TokenVariable v)
                    ? v.RenderVariable()
                    : t.Value + (t.HasEndLine ? Environment.NewLine : "")));
        }
    }

    public class Token0 : Token
    {
        protected Token0(TokenType tt, string tk, bool endline = false) : base(tt, tk, endline)
        {
        }

        public static Token0 TokenEOF { get; } = new Token0(TokenType.EOF, "");
        public static Token0 Blank(string v) => new Token0(TokenType.Blank, v);
        public static Token0 NewLine() => new Token0(TokenType.NewLine, Environment.NewLine);
        public static Token0 Symbol(string v) => new Token0(TokenType.Symbol, v);
        public static Token0 Literal(string v) => new Token0(TokenType.Literal, v);
        public static Token0 Quote(string v) => new Token0(TokenType.Quote, v);
        public static Token0 Variable(string v) => TokenVariable.Factory(v);

        public class TokenVariable : Token0
        {
            private TokenVariable(string name) : base(TokenType.Variable, name, false)
            {
            }

            public static TokenVariable Factory(Token a, Token n)
            {
                var name = a.Value + n.Value;
                return Factory(name);
            }

            public static TokenVariable Factory(string name)
            {
                if (!name.StartsWith("@")) throw new Exception("Esperaba prefijo variable");
                return new TokenVariable(name);
            }

            public string RenderVariable() => Value.Replace("_", ".");
        }
    }

    internal class TokenBuffer : Buffer<Token>
    {
        protected override Token EOFSymbol => Token0.TokenEOF;
        protected override bool IsEof(Token item) => item.TokenType == TokenType.EOF;

        public int Line { get; private set; }
        public int Col { get; private set; }

        public string PeekStr() => Peek().Value;

        public string Peek2Str()
        {
            var (p1, p2) = Peek2();
            return p1.Value + p2.Value;
        }

        public string Peek3Str()
        {
            var (p1, p2, p3) = Peek3();
            return p1.Value + p2.Value + p3.Value;
        }

        public override Token Pop()
        {
            var t = base.Pop();
            if (t.TokenType == TokenType.NewLine)
            {
                Line++;
                Col = 0;
            }
            else
            {
                if (t.TokenType != TokenType.Blank) Col++;
            }

            return t;
        }


        public string PopStr() => Pop().Value;

        public string Pop2Str()
        {
            var (p1, p2) = Pop2();
            return p1.Value + p2.Value;
        }

        public string Pop3Str()
        {
            var (p1, p2, p3) = Pop3();
            return p1.Value + p2.Value + p3.Value;
        }

        public TokenBuffer(IEnumerable<Token> data) : base(data)
        {
        }
    }

    public class Token1 : Token
    {
        public List<Token> SubTokens { get; }
        public IEnumerable<Token> InnerTokens { get; protected set; }
        public override string InnerValue() => ToString(InnerTokens);


        protected Token1(TokenType tt, IEnumerable<Token> subtokens, bool endline) : base(tt, "", endline)
        {
            SubTokens = subtokens.ToList();
            if (endline && SubTokens.LastOrDefault()?.TokenType == TokenType.NewLine)
                SubTokens.RemoveAt(SubTokens.Count - 1);
            var last = SubTokens.LastOrDefault();
            if (endline && (last?.HasEndLine ?? false)) last.HasEndLine = false;
            InnerTokens = SubTokens;
            Value = ToString(SubTokens);
        }

        public static Token1 Error(IEnumerable<Token> subtokens) => new TokenError(subtokens, "");
        public static Token1 Expression(IEnumerable<Token> subtokens) => new TokenExpression(subtokens);
        public static Token1 SubMacro(IEnumerable<Token> subtokens) => new TokenSubMacro(subtokens);

        public static Token1 SuperComment(IEnumerable<Token> subtokens) =>
            new Token1(TokenType.SuperComment, subtokens, true);

        public static Token1 MultiComment(IEnumerable<Token> subtokens) =>
            new Token1(TokenType.MultiComment, subtokens, true);

        public static Token1 Preprocessor(IEnumerable<Token> subtokens) =>
            TokenPreProcessor.Factory(subtokens.ToList());

        public static Token1 Pure(IEnumerable<Token> subtokens) => new Token1(TokenType.Pure, subtokens, false);
        public static Token1 PureLine(IEnumerable<Token> subtokens) => new Token1(TokenType.Pure, subtokens, true);

        private class TokenExpression : Token1
        {
            public TokenExpression(IEnumerable<Token> subtokens) : base(TokenType.Expression, subtokens, false)
            {
                if (SubTokens.First().Value != "@(" || SubTokens.Last().Value != ")")
                    throw new Exception("Esperaba @(...)");
                InnerTokens = SubTokens.GetRange(1, SubTokens.Count - 2);
            }

            public override string InnerValue()
            {
                return ToStringRenderVar(InnerTokens);
            }
        }

        private class TokenSubMacro : Token1
        {
            public TokenSubMacro(IEnumerable<Token> subtokens) : base(TokenType.SubMacro, subtokens, true)
            {
                if (SubTokens.First().Value != "@{" || SubTokens.Last().Value != "}")
                    throw new Exception("Esperaba @{...}");
                InnerTokens = SubTokens.GetRange(1, SubTokens.Count - 2);
            }

            public override string InnerValue()
            {
                return ToStringRenderVar(InnerTokens);
            }

            public override string[] RenderLines()
            {
                var lines = InnerValue().Split(new[] {Environment.NewLine}, StringSplitOptions.None);
                return lines.ToArray();
            }
        }

        public class TokenPreProcessor : Token1
        {
            public string PreprocessorType { get; }

            public string LineRendered { get; }

            protected TokenPreProcessor(IEnumerable<Token> subtokens, string type, string line) : base(
                TokenType.Preprocessor, subtokens, true)
            {
                PreprocessorType = type;
                LineRendered = line;
            }

            public static Token1 Factory(List<Token> subTokens)
            {
                Token1 Error(string e) => new TokenError(subTokens, e);
                if (subTokens.Count == 0) return Error("Error no tokens");
                var res = new StringBuilder();
                if (subTokens[0].TokenType == TokenType.Blank)
                {
                    res.Append(subTokens[0].Value);
                    subTokens.RemoveAt(0);
                }

                if (subTokens.Count < 2) return Error("Error esperaba #..");
                if (subTokens[0].Value != "#" || subTokens[1].TokenType != TokenType.Literal)
                    return Error("Error esperaba #..");
                var type = subTokens[1].Value.ToLower();
                var inner = subTokens.Skip(2).Where(t => t.TokenType != TokenType.Blank).TakeWhile(t => t.Value != "//")
                    .ToList();
                var line = Render(type, inner, subTokens, out var error);
                if (error != "") return Error(error);
                if (line == "") line = string.Concat(subTokens.Select(t => t.Render()));
                return new TokenPreProcessor(subTokens, type, line);
            }

            private static string Render(string type, IList<Token> inner, IList<Token> tokens, out string error)
            {
                error = "";
                switch (type)
                {
                    case "for": return RenderFor(inner, tokens, out error);
                    case "foreach": return RenderForEach(inner, tokens, out error);
                    case "if": return RenderIf(inner, tokens, out error);

                    case "endif": return RenderEndIf();
                    case "else": return RenderElse();
                    case "endforeach": return RenderEndForEach();
                    case "endfor": return RenderEndFor();
                    case "set": return RenderSet(inner, tokens, out error);
                    case "unset": return RenderUnset(inner, tokens, out error);
                    case "definevar": return RenderDefineVar(inner, tokens, out error);
                    case "using": return RenderUsing(inner, tokens, out error);
                    case "echo": return RenderEcho(inner, tokens, out error);
                    case "log": return RenderLog(inner, tokens, out error);
                    default:
                        error = "";
                        return "";
                }
            }

            public override string Render() => LineRendered;

            private static string Error(string e, out string err)
            {
                err = e;
                return "";
            }

            private static string RenderUnset(IList<Token> inner, IList<Token> tokens, out string error)
            {
                error = "";
                if (!(inner.FirstOrDefault() is Token0.TokenVariable v)) return Error("#unset No variable", out error);
                return $"Flags[\"{v.RenderVariable()}\"]=false;";
            }

            private static string RenderSet(IList<Token> inner, IList<Token> tokens, out string error)
            {
                error = "";
                // FLAGS
                if (!(inner.FirstOrDefault() is Token0.TokenVariable v)) return Error("#set No variable", out error);
                if (inner.Count <= 1) return $"Flags[\"{v.RenderVariable()}\"]=true;";

                var s = inner[1];
                if (s.Value != "=") return Error("#set no assignacion", out error);
                if (inner.Count < 3) return Error("#set no right parameter", out error);
                var tks = tokens.SkipWhile(t => t.Value != "=");
                var rvalue =
                    string.Concat(tks.Select(t => (t is Token0.TokenVariable vv) ? vv.RenderVariable() : t.Render()));
                return Escape($"{v.RenderVariable()} {rvalue};");
            }

            private static string RenderDefineVar(IList<Token> inner, IList<Token> tokens, out string error)
            {
                error = "";
                // FLAGS
                if (!(inner.FirstOrDefault() is Token0.TokenVariable v))
                    return Error("#definevar No variable", out error);
                if (inner.Count <= 1) return $"dynamic {v.RenderVariable()};";

                var s = inner[1];
                if (s.Value != "=") return Error("#definevar no assignacion", out error);
                if (inner.Count < 3) return Error("#definevar no right parameter", out error);
                var tks = tokens.SkipWhile(t => t.Value != "=");
                var rvalue =
                    string.Concat(tks.Select(t => (t is Token0.TokenVariable vv) ? vv.RenderVariable() : t.Render()));
                return $"dynamic {v.RenderVariable()} {rvalue};";
            }

            private static string RenderEndFor()
            {
                return "}; // EndFor";
            }

            private static string RenderEndForEach()
            {
                return "}; // EndForEach";
            }

            private static string RenderElse()
            {
                return "} else {";
            }

            private static string RenderEndIf()
            {
                return "}; // EndIf";
            }

            private static string RenderIf(IList<Token> inner, IList<Token> tokens, out string error)
            {
                error = "";
                var not = false;
                if (inner.Count < 1) return Error("Empty If", out error);
                var pt = inner.First();
                tokens = tokens.SkipWhile(t => t != pt).ToList();
                if (pt.Value == "!")
                {
                    not = true;
                    inner.RemoveAt(0);
                    tokens.RemoveAt(0);
                    pt = inner.FirstOrDefault();
                    if (pt == null) return Error("Empty If !", out error);
                }

                if (pt is Token0.TokenVariable v)
                {
                    // FLAGS #if var
                    if (inner.Count == 1 && !v.Value.Contains('_'))
                    {
                        if (not)
                            return $"if (!(Flags[\"{v.RenderVariable()}\"])){{";
                        return $"if (Flags[\"{v.RenderVariable()}\"]){{";
                    }

                    if (inner.Count > 1)
                    {
                        // FLAGS #if var IN collection
                        var incol = inner[1];
                        if (string.Equals(incol.Value, "in", StringComparison.OrdinalIgnoreCase))
                        {
                            var rvalue = tokens.SkipWhile(t => t != incol).Skip(1).ToArray();
                            var liner = ToStringRenderVar(rvalue);
                            if (not)
                                return $"if (!(new []{{{liner}}}).Contains({v.RenderVariable()})){{";
                            return $"if ((new []{{{liner}}}).Contains({v.RenderVariable()})){{";
                        }
                    }
                }

                if (pt.Value != "(")
                {
                    var p0 = Token0.Symbol("(");
                    var p1 = Token0.Symbol(")");
                    inner.Insert(0, p0);
                    inner.Add(p1);
                    tokens.Insert(0, p0);
                    tokens.Add(p1);
                }

                var line = ToStringRenderVar(tokens);
                if (not)
                    return $"if (!{line}){{";
                return $"if {line}{{";
            }

            private static string RenderUsing(IList<Token> inner, IList<Token> tokens, out string error)
            {
                error = "";
                var line = ToStringRenderVar(inner);
                return string.Join(Environment.NewLine, "Output.AppendLine(\"#pragma warning disable CS0105\");",
                    $"Output.AppendLine(\"using {line};\");", "Output.AppendLine(\"#pragma warning restore CS0105\");");
            }
            private static string RenderEcho(IList<Token> inner, IList<Token> tokens, out string error)
            {
                error = "";
                var line = ToStringRenderVar(inner);
                return string.Join(Environment.NewLine, $"Echo({line});");
            }
            private static string RenderLog(IList<Token> inner, IList<Token> tokens, out string error)
            {
                error = "";
                var line = ToStringRenderVar(inner);
                return string.Join(Environment.NewLine,$"Echo(\"LOG: \"+{line});", $"Log.AppendLine({line});");
            }
            private static string RenderFor(IList<Token> inner, IList<Token> tokens, out string error)
            {
                error = "";
                // #for @var = 1 to/downto 5
                if (inner.Count < 5) return Error("Error #for no items", out error);
                if (!(inner.FirstOrDefault() is Token0.TokenVariable v)) return Error("Error no variable", out error);
                if (inner[1].Value != "=") return Error("Error no assign", out error);
                tokens = tokens.SkipWhile(t => t.Value != "=").ToList();
                var dir = inner.FirstOrDefault(t => t.Value.ToLower() == "to" || t.Value.ToLower() == "downto")?.Value
                    .ToLower();
                if (dir == null) return Error("Error no direction", out error);
                var lvalue = tokens.TakeWhile(t => t.Value != dir).ToArray();
                var rvalue = tokens.SkipWhile(t => t.Value != dir).Skip(1).ToArray();
                if (lvalue.Length == 0) return Error("Error no lvalue", out error);
                if (rvalue.Length == 0) return Error("Error no rvalue", out error);
                var vv = v.RenderVariable();
                if (dir == "to")
                    return $"for(var @{vv}={ToStringRenderVar(lvalue)};@{vv}<=({ToStringRenderVar(rvalue)});{vv}++){{";
                return $"for(var @{vv}={ToStringRenderVar(lvalue)};@{vv}>=({ToStringRenderVar(rvalue)});{vv}--){{";
            }

            private static string RenderForEach(IList<Token> inner, IList<Token> tokens, out string error)
            {
                error = "";
                // #foreach @var in xxx
                if (inner.Count < 3) return Error("Error #foreach no items", out error);
                if (!(inner.FirstOrDefault() is Token0.TokenVariable v)) return Error("Error no variable", out error);
                var instr = inner[1].Value;
                if (!instr.Equals("in", StringComparison.OrdinalIgnoreCase)) return Error("Error no IN", out error);
                tokens = tokens.Skip(2).ToList();
                var value = ToStringRenderVar(tokens);
                return $"foreach(var {value} ){{";
            }
        }

        public class TokenError : Token1
        {
            public new string Error { get; }


            public TokenError(IEnumerable<Token> subTokens, string error) : base(TokenType.Error, subTokens, true)
            {
                Error = error;
            }
        }
    }
}