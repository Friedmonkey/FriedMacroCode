using FriedLexer;
using static FriedLexer.LogicalTokens;

namespace FriedMacroCode.Parser;

public partial class Parser : AnalizerBase<FToken<Token>>
{
    private Ilogger? logger;
    public Parser(ParserOptions options) : base(new FToken<Token>(Token.EOF))
    {
        logger = options.Logger;
        FLexer<Token> tokenizer = new FLexer<Token>(options.Text, Token.BadToken, Token.EOF);
        tokenizer.DefinedTokens = new Dictionary<string, Token>
        {
            { "@",Token.ApeTail},
            { "(",Token.lPar},
            { ")",Token.rPar},
            { "{",Token.lBrace},
            { "}",Token.rBrace},
            { "!",Token.Bang},
        };

        var strToken = new StringToken<Token>(Token.String);
        var singleCommentToken = new SinglelineCommentToken<Token>(Token.Comment);
        var MultiCommentToken = new MultilineCommentToken<Token>(Token.Comment);
        var KeywordToken = new IdentifierOrKeywordToken<Token>(Token.Keyword, Token.Identifier, Keywords);


        tokenizer.AddLogicalToken<WhitespaceToken<Token>>();
        tokenizer.AddLogicalToken(strToken);
        tokenizer.AddLogicalToken(singleCommentToken);
        tokenizer.AddLogicalToken(MultiCommentToken);
        tokenizer.AddLogicalToken(KeywordToken);

        tokenizer.AddLogicalToken<EmbedToken>();

        var TokenResult = tokenizer.Lex();

#if DEBUG
        foreach (var token in TokenResult)
        {
            if (token.Type.Equals(Token.BadToken))
            {
                Console.WriteLine($"bad token:	on pos:{token.Position,-3} token:{token.Text,-15} with text:{token.Text,-20} with val:{token.Value ?? "Null",-20}");
            }
            else
            {
                Console.WriteLine($"good token:	on pos:{token.Position,-3} token:{token.Type.GetName(),-15} with text:{token.Text,-20} with val:{token.Value ?? "Null",-20}");
            }
        }
        Console.ReadLine();
#endif


        Analizable = TokenResult;
    }
    public static List<string> Keywords = new List<string>()
    {
        "define",
        "include",
    };
}
