namespace FriedMacroCode;

public class ParserSettings
{
    public string Text { get; set; } = string.Empty;
    public Ilogger? Logger { get; set; } = null;
    public bool IncludeOnly { get; set; }
    public bool ExpandMacros { get; set; }
    public string? CompileOutput { get; set; } = null;
}
