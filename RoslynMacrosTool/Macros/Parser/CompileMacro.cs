using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RoslynMacros.Parser;

namespace RoslynMacros.CompileMacro
{
    public class CompileCsMacro
    {
        public List<Token> Tokens { get; } = new List<Token>();
        public int Line { get; private set; }
        public string Result { get; private set; } = "";
        public string Error { get; private set; } = "";

        public string Compile(string macro)
        {
            Result = "";
            Tokens.Clear();
            Error = "";
            var tokens = TokenParser.Parse(macro);
            var terr = tokens.FirstOrDefault(t => t.TokenType == TokenType.Error);
            if (terr != null)
            {
                Error = $"Error in {terr.Value}";
                return "";
            }

            return Compile(tokens);
        }

        private string Compile(IList<Token> tokens)
        {
            var res = new StringBuilder();
            Line = 1;
            foreach (var t in tokens)
                switch (t.TokenType)
                {
                    case TokenType.Expression:
                        WriteExpresion(t, res);
                        break;
                    case TokenType.Preprocessor:
                        WritePreProcessor(t, res);
                        break;
                    case TokenType.SubMacro:
                        WriteSubMacro(t, res);
                        break;
                    case TokenType.SuperComment: break;
                    case TokenType.Variable:
                        WriteVariable(t, res);
                        break;
                    default:
                        WritePure(t, res);
                        break;
                }

            res.AppendLine("return true;");
            return res.ToString();
        }

        private void WriteVariable(Token t, StringBuilder res)
        {
            var cad = t.Render().Replace("_", ".");
            res.AppendLine($"Output.Append({cad});");
        }

        private void WriteSubMacro(Token t, StringBuilder res)
        {
            var lines = t.RenderLines();
            if (lines.Length == 0) return;
            if (lines.Length == 1)
            {
                res.AppendLine($"{lines[0]}");
                return;
            }

            res.AppendLine("// Begin Code Block");
            foreach (var l in lines)
            {
                res.AppendLine($"#line {Line}");
                res.AppendLine($"{l}");
                Line++;
            }

            res.AppendLine("// End Code Block");
        }

        private void WritePreProcessor(Token t, StringBuilder res)
        {
            var cad = t.Render();
            res.AppendLine($"#line {Line}");
            res.AppendLine(cad);
            Line++;
        }

        private void WriteExpresion(Token t, StringBuilder res)
        {
            var cad = t.Render();
            res.AppendLine($"#line {Line}");
            res.AppendLine($"Output.Append({cad});");
            if (cad.Contains(Environment.NewLine)) Line++;
        }

        private void WritePure(Token t, StringBuilder res)
        {
            var lines = t.RenderLines();
            if (lines.Length == 0) return;
            for (var f = 0; f < lines.Length - 1; f++)
            {
                res.AppendLine($"#line {Line}");
                res.AppendLine($"Output.AppendLine(\"{lines[f]}\");");
                Line++;
            }

            var last = lines.Last();
            res.AppendLine(t.HasEndLine ? $"Output.AppendLine(\"{last}\");" : $"Output.Append(\"{last}\");");
            if (t.HasEndLine) Line++;
        }
    }
}