using FriedLexer;

namespace FriedMacroCode;

public class EmbedToken : LogicalToken<Token>
{
    string rawValue = string.Empty;
    public override bool IfMatch()
    {
        rawValue = string.Empty;
        str = string.Empty;
        startPos = Position;
        return (Current == '#');
    }

    public override FToken<Token>? ParseToken()
    {
        rawValue += Current; //#
        Position++;
        while (Safe)
        {
            if (Current == '\n')
            { 
                Position++;
                while (Safe && char.IsWhiteSpace(Current))
                {
                    Position++;
                }

                if (Current == '-') //keep going
                {
                    Position++;
                }
                else //we stop here
                {
                    Position--;
                    str += Current;
                    rawValue += Current;
                    return new FToken<Token>(Token.Embed, startPos, str, rawValue);
                }
            }
            str += Current;
            rawValue += Current;
            Position++;
        }
        rawValue += Current;
        return new FToken<Token>(Token.BadToken, startPos, str, rawValue);
    }
}

public class XMLToken : LogicalToken<Token>
{
    public XMLToken(string name, Token token)
    {
        Start = $"<{name}>";
        End = $"</{name}>";
        this.token = token;
    }

    string rawValue = string.Empty;
    private string Start = string.Empty;
    private string End = string.Empty;
    private Token token = Token.BadToken;
    public override bool IfMatch()
    {
        rawValue = string.Empty;
        str = string.Empty;
        startPos = Position;
        return PeekStr(Start);
    }

    public override FToken<Token>? ParseToken()
    {
        rawValue += Start;
        Position += Start.Length;

        while (Safe && !PeekStr(End))
        {
            str += Current;
            rawValue += Current;
            Position++;
        }
        rawValue += End;
        return new FToken<Token>(token, startPos, str, rawValue);
    }
    public bool PeekStr(string find)
    {
        for (int i = 0; i < find.Length; i++)
        {
            if (Peek(i) == find[i])
            {
                continue;
            }
            else return false;
        }
        return true;
    }
}