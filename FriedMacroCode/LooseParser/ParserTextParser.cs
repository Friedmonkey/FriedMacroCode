using FriedLexer;

namespace FriedMacroCode.LooseParser;

public partial class Parser
{
    public string ParseText(ref List<string> rawValues)
    {
        string code = string.Empty;

        try
        {
            code = ParseIncludes();
            UpdateAnalizableAndResetIndex();

            Console.WriteLine(code);
        }
        catch
        { 
        
        }
        void UpdateAnalizableAndResetIndex()
        {
            this.Analizable = code.ToList();
            this.Position = 0;
        }
        return code;
    }
    //private string GetKeyword() => StringValue(Token.Keyword).ToLower();
    //private string GetIdentifier() => StringValue(Token.Identifier);
    //private string GetString() => StringValue(Token.String);

    //private string StringValue(Token type)
    //{
    //    var token = Consume(type);
    //    if (token.Value is null)
    //        throw newException("value was null?", token);
    //    return ((string)token.Value);
    //}
    //private FToken<Token> Consume(Token type, string extraInfo = "")
    //{
    //    if (Current.Type != type)
    //    {
    //        throw newException($"Expected a {type} but got {Current.Type} instead! "+extraInfo);
    //    }
    //    else
    //    {
    //        Position++;
    //        return Peek(-1);
    //    }
    //}
}
