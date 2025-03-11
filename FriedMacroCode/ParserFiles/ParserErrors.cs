using FriedLexer;

namespace FriedMacroCode.ParserFiles;

public partial class Parser
{
    public (int line, int column) TokenToCoorinate(FToken<Token> token)
    {
        string origin = token.Origin;
        if (token.InternalMacroName is not null)
            origin += " on macro:" + token.InternalMacroName;

        string[] lines = inputCache[origin];
        int position = token.Position;

        int lineIndex = 0;
        int columnIndex = 0;
        foreach (string line in lines)
        {
            if (position < line.Length)
            {
                columnIndex = position;
                break;
            }
            else
            {
                position -= line.Length + 1; // +1 because split removes the \n
            }
            lineIndex++;
        }
        return (lineIndex, columnIndex);
    }
    public TokenException<Token> newException(string message) => newException(message, Current);
    public TokenException<Token> newException(string message, FToken<Token> token)
    {
        var (line, col) = TokenToCoorinate(token);
        if (token.InternalMacroName is null)
            message += $" in file: {token.Origin} on line:{line + 1} col:{col + 1}";
        else
            message += $" in file: {token.Origin} on macro {token.InternalMacroName} on line of macro_body:{line} col:{col + 1}";

        return new TokenException<Token>(message, token, line, col);
    }
}
