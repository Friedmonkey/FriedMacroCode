using FriedLexer;
using static System.Net.Mime.MediaTypeNames;

namespace FriedMacroCode.ParserFiles;

public partial class Parser
{
    private bool TryGetMacro(string name, out Macro? macro)
    {
        macro = Macros.FirstOrDefault(m => string.Equals(m.name, name, StringComparison.OrdinalIgnoreCase));
        return (macro is not null);
    }
    public void ParseExpandMacro()
    {
        Consume(Token.ApeTail);
        string defineName = GetIdentifier();
        if (!TryGetMacro(defineName, out Macro? macro))
            throw newException($"Macro with name {defineName} not found!", Peek(-1));

        List<string> arguments = new List<string>();
        if (Current.Type == Token.lPar) //it has parameters
        {
            Position++;
            while (Safe && Current.Type != Token.rPar)
            {
                string arg = string.Empty;
                if (Current.Type == Token.XMLRaw)
                {
                    arg = (string)(Current.Value ?? throw newException($"Macro argument value {macro!.args[arguments.Count]} was empty"));
                }
                //else if (Current.Type == Token.XMLMacro)
                //{
                //    arg = (string)(Current.Value ?? throw newException($"Macro argument value {macro!.args[arguments.Count]} was empty"));
                //    ParserOptions options = new ParserOptions()
                //    {
                //        Text = arg,
                //        Origin = Current.Origin,
                //        InternalMacroName = macro.name + " argument: " + macro!.args[arguments.Count],
                //        ShowTokens = true,
                //        Logger = this.logger
                //    };
                //    Parser parser = new Parser(options);
                //    parser.Analizable.RemoveAt(parser.Analizable.Count - 1); //remove EOF token
                //    this.Analizable.InsertRange(Position, parser.Analizable);

                //}
                else
                {
                    arg = Current.Text;
                }
                Position++;

                arguments.Add(arg);
                if (Current.Type == Token.Comma) //it has multiple args
                {
                    Consume(Token.Comma);
                }
                else if (Current.Type == Token.rPar)
                {
                    break;
                }
                else
                { 
                    throw newException("Macro argument can only be a single token, wrap in raw if you need multiple <raw>token1 token2</raw>");
                }
            }
            Consume(Token.rPar);
        }

        string GetMacroBodyTextTemplated()
        {
            string text = (string)(macro.macroBody.Value ?? throw newException("Macro body had no value!", macro.macroBody));
            if (macro.args.Count != arguments.Count)
                throw newException($"Macro count mismatch, macro {macro.name} expected {macro.args.Count} arguments but got {arguments.Count}");

            for (int i = 0; i < arguments.Count; i++)
            {
                string name = macro.args[i];
                string value = arguments[i];
                text = text.Replace(name, value);
            }
            return text;
        }
        if (macro.macroBody.Type == Token.XMLMacro)
        {
            string text = GetMacroBodyTextTemplated();
            ParserOptions options = new ParserOptions()
            {
                Text = text,
                Origin = macro.macroBody.Origin,
                InternalMacroName = macro.name,
                ShowTokens = true,
                Logger = this.logger
            };
            Parser parser = new Parser(options);
            parser.Analizable.RemoveAt(parser.Analizable.Count - 1); //remove EOF token
            this.Analizable.InsertRange(Position, parser.Analizable);
        }
        else if (macro.macroBody.Type == Token.XMLCodeLua)
        {
            string text = GetMacroBodyTextTemplated();
            var newbody = (macro.macroBody);
            newbody.Value = text;

            this.Analizable.Insert(Position, newbody);
            //macro = macro with { macroBody = newbody };
        }
        else
        {
            this.Analizable.Insert(Position, macro.macroBody);
        }
        List<string> parameters = new List<string>();
        FToken<Token> macroBody = new FToken<Token>(Token.BadToken);

        if (Current.Type == Token.lPar) //it has parameters
        {
            Position++;
            while (Safe && Current.Type != Token.rPar)
            {
                string error = "Macro parameters name need to start and end with a '%' to avoid accidental replacements";
                Consume(Token.Percentage, error);
                string identifer = GetIdentifier();
                Consume(Token.Percentage, error);

                parameters.Add(identifer);
                if (Current.Type == Token.Comma) //it has multiple args
                {
                    Consume(Token.Comma);
                }
            }
            Consume(Token.rPar);
            macroBody = Consume(Token.XMLMacro);
        }
        else
        {
            macroBody = Current;
        }
        Macros.Add(new Macro(defineName, parameters, macroBody));
    }
}
