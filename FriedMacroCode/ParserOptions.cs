namespace FriedMacroCode;

public class ParserOptions
{
    public string Text { get; set; } = string.Empty;
    public string Origin { get; set; } = "no_orgin";
    public string? InternalMacroName { get; set; } = null;
    public bool ShowTokens { get; set; }
    public Ilogger? Logger { get; set; } = null;
}
