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
                    case "define": ParseDefine(); break;
                    default: throw newException($"Unhandled keyword {keyword}", Peek(-1));
                }
            }
            if (Current.Type == Token.ApeTail)
            {
                //macro expand!
                ParseExpandMacro();
            }
            if (Current.Type == Token.Embed || Current.Type == Token.XMLRaw)
            {
                if (Current.Value is null)
                    throw newException("Embed has no value!");
                rawValues.Add((string)Current.Value);
                finalText += $"{CurrentBuffer} = {CurrentBuffer} .. {RawValues}[{rawValues.Count()}]\n";
            }
            ///switch (Current.Type)
            ///{
            ///    case Token.Bang:
            ///        break;
            ///    case Token.ApeTail:
            ///        break;
            ///    case Token.lPar:
            ///        break;
            ///    case Token.rPar:
            ///        break;
            ///    case Token.lBrace:
            ///        break;
            ///    case Token.rBrace:
            ///        break;
            ///    case Token.String:
            ///        break;
            ///    case Token.Embed:
            ///        break;
            ///    case Token.Comment:
            ///        break;
            ///    case Token.Keyword:
            ///        break;
            ///    case Token.Identifier:
            ///        break;
            ///    case Token.BadToken:
            ///        break;
            ///    case Token.EOF:
            ///        break;
            ///    default:
            ///        break;
            ///}
            Position++;
        }
        return finalText;
    }
    private string GetKeyword() => StringValue(Token.Keyword).ToLower();
    private string GetIdentifier() => StringValue(Token.Identifier);
    private string GetString() => StringValue(Token.String);

    //{
    //    var keyword = Consume(Token.Keyword);
    //    if (keyword.Value is null)
    //        throw new Exception("value of keyword was null?");
    //    return ((string)keyword.Value).ToLower();
    //}
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
