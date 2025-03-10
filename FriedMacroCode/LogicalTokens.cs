using FriedLexer;

namespace FriedMacroCode;

public class EmbedLogicalToken : LogicalToken<Token>
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
