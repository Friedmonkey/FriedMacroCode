using FriedLexer;

namespace FriedMacroCode;

public class Parser : FLexer<Token>
{
    private Ilogger? logger;
    public Parser(ParserOptions options) : base(options.Text, Token.BadToken, Token.EOF)
    {
        this.logger = options.Logger;
    }

    public void Parse()
    { 
        
    }
}
