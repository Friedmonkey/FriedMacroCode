using FriedLexer;

namespace FriedMacroCode.ParserFiles;

public partial class Parser
{
    public string ParseText(ref List<string> rawValues)
    {
        string finalText = string.Empty;
        while (Safe)
        {
            if (Current.Type == Token.Bang)
            {
                Position++;
                string keyword = GetKeyword();
                switch (keyword)
                {
                    case "include": ParseInclude(); break;
                    case "try_define":
                        {
                            string defineName = GetIdentifier();
                            var macro = ParseDefine(defineName);
                            if (!TryGetMacro(defineName, out _))
                                Macros.Add(macro);
                        }
                        break;
                    case "define":
                        {
                            string defineName = GetIdentifier();
                            Macros.Add(ParseDefine(defineName));
                        }
                        break;
                    default: throw newException($"Unhandled keyword {keyword}", Peek(-1));
                }
            }
            else if (Current.Type == Token.ApeTail)
            {
                ParseExpandMacro();
            }
            else if (Current.Type == Token.Embed || Current.Type == Token.XMLRaw)
            {
                if (Current.Value is null)
                    throw newException("Embed has no value!");
                rawValues.Add((string)Current.Value);
                finalText += $"{CurrentBuffer} = {CurrentBuffer} .. {RawValues}[{rawValues.Count()}]\n";
                Position++;
            }
            else if (Current.Type == Token.XMLCodeLua)
            {
                if (Current.Value is null)
                    throw newException("Lua has no value!");
                finalText += (string)Current.Value;
                Position++;
            }
            else if (Current.Type == Token.Comment)
            {
                Position++;
            }
            else if (Current.Type == Token.EOF)
            {
                break;
            }
            else
            {
                if (Current.Value is null)
                    throw newException($"Unexpected token at start of expression! type: {Current.Type} text:{Current.Text}");

                Console.WriteLine($"adding atom expression type:{Current.Type} with value: {Current.Value}");
                finalText += (string)Current.Value;
                Position++;
            }
        }
        return finalText;
    }
    private string GetKeyword() => StringValue(Token.Keyword).ToLower();
    private string GetIdentifier() => StringValue(Token.Identifier);
    private string GetString() => StringValue(Token.String);

    private string StringValue(Token type)
    {
        var token = Consume(type);
        if (token.Value is null)
            throw newException("value was null?", token);
        return ((string)token.Value);
    }
    private FToken<Token> Consume(Token type, string extraInfo = "")
    {
        if (Current.Type != type)
        {
            throw newException($"Expected a {type} but got {Current.Type} instead! "+extraInfo);
        }
        else
        {
            Position++;
            return Peek(-1);
        }
    }
}
