using FriedLexer;
using static FriedLexer.LogicalTokens;

namespace FriedMacroCode.ParserFiles;

public partial class Parser : AnalizerBase<FToken<Token>>
{
    private Ilogger? logger;
    public void SetMacroContext(string? macroName)
    { 
        FToken<Token>.CurrentInternalMacroContext = macroName;
    }
    public void SetOrginContext(string newOrgin)
    {
        var fullPath = Path.GetFullPath(newOrgin);
        FToken<Token>.CurrentOriginContext = fullPath;
    }
    public string GetOrginContext() => FToken<Token>.CurrentOriginContext;
    private static Dictionary<string, string[]> inputCache = new();
    public Parser(ParserOptions options) : base(new FToken<Token>(Token.EOF))
    {
        SetMacroContext(options.InternalMacroName);
        SetOrginContext(options.Origin);
        var origin = GetOrginContext();
        if (options.InternalMacroName is not null)
            origin += " on macro:"+options.InternalMacroName;
        if (!inputCache.ContainsKey(origin))
            inputCache.Add(origin, options.Text.Split('\n').ToArray());

        logger = options.Logger;
        FLexer<Token> tokenizer = new FLexer<Token>(options.Text, Token.BadToken, Token.EOF);
        tokenizer.DefinedTokens = new Dictionary<string, Token>
        {
            { "@",Token.ApeTail},
            { "(",Token.lPar},
            { ")",Token.rPar},
            { "!",Token.Bang},
            { "%",Token.Percentage},
            { ",",Token.Comma},
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

        var macroXMLToken = new XMLToken("macro", Token.XMLMacro);
        var luaXMLToken = new XMLToken("lua", Token.XMLCodeLua);
        var embedXMLToken = new XMLToken("raw", Token.XMLRaw);

        tokenizer.AddLogicalToken<EmbedToken>();
        tokenizer.AddLogicalToken(macroXMLToken);
        tokenizer.AddLogicalToken(luaXMLToken);
        tokenizer.AddLogicalToken(embedXMLToken);

        var TokenResult = tokenizer.Lex();

        if (options.ShowTokens)
        {
            foreach (var token in TokenResult)
            {
                if (token.Type.Equals(Token.BadToken))
                {
                    throw newException($"Bad token: text:{token.Text}", token);
                    //Console.WriteLine($"bad token:	in file: \"{token.Origin,-3}\" on pos:{token.Position,-3} token:{token.Text,-20} with text:{token.Text,-25} with val:{token.Value ?? "Null",-25}");
                }
                else
                {
                    //Console.WriteLine($"good token:	in file: \"{token.Origin,-3}\" on pos:{token.Position,-3} token:{token.Type.GetName(),-20} with text:{token.Text,-25} with val:{token.Value ?? "Null",-25}");
                }
            }
            //Console.ReadLine();
        }

        Analizable = TokenResult;
    }
    public static List<string> Keywords = new List<string>()
    {
        "define",
        "try_define",
        "include",
    };
}
