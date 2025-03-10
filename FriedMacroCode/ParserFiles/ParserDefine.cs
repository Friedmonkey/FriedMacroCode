using FriedLexer;

namespace FriedMacroCode.ParserFiles;

public partial class Parser
{
    public record Macro(string name, List<string> args, FToken<Token> macroBody);
    List<Macro> Macros = new List<Macro>();
    public Macro ParseDefine(string defineName)
    {
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

            if (Current.Type == Token.XMLCodeLua)
                macroBody = Consume(Token.XMLCodeLua);
            else
                macroBody = Consume(Token.XMLMacro);
        }
        else
        {
            macroBody = Current;
            Position++;
        }
        return new Macro(defineName, parameters, macroBody);
    }
}
