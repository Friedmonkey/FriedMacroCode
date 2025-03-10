namespace FriedMacroCode.Parser;

public partial class Parser
{
    public void Parse()
    {
        while (Safe)
        {
            if (Current.Type == Token.Bang)
            { 
                
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
    }
}
