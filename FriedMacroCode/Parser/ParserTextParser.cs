using FriedLexer;

namespace FriedMacroCode.Parser;

public partial class Parser
{
    public string ParseText()
    {
        string finalText = string.Empty;
        while (Safe)
        {
            if (Current.Type == Token.Bang)
            {
                Position++;
                string keyword = GetKeyword();
            }
            if (Current.Type == Token.Embed)
            {
                if (Current.Value is null)
                    throw new Exception("Embed has no value!");
                finalText += (string)Current.Value;
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
    private string GetKeyword()
    {
        var keyword = Consume(Token.Keyword);
        if (keyword.Value is null)
            throw new Exception("value of keyword was null?");
        return ((string)keyword.Value).ToLower();
    }
    private FToken<Token> Consume(Token type)
    {
        if (Current.Type != type)
        {
            throw new Exception($"Expected a {type} but got {Current.Type} instead!");
        }
        else
        {
            Position++;
            return Peek(-1);
        }
    }
}
