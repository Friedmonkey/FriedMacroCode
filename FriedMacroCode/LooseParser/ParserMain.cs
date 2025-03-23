using FriedLexer;

namespace FriedMacroCode.LooseParser;

public partial class Parser : AnalizerBase<char>
{
    private Ilogger? logger;
    private ParseContext context;
    private bool includeOnly;
    private string? compileOutput;

    public Parser(ParserSettings settings) : base('\0')
    {
        this.includeOnly = settings.IncludeOnly;
        this.compileOutput = settings.CompileOutput;

        this.logger = settings.Logger;
        this.context = new ParseContext();

        //add padding for easy parsing
        this.Analizable = $"\n{settings.Text}\n".ToList();
    }
    public class ParseContext
    {
        public string Scope = "init";
        public string ExtraInfo = "init";

        internal Exception ExpectedException(char current, char expected)
        {
            return new Exception($"Error while parsing {Scope}. Expected '{expected}' but got '{current}' instead!"+ExtraInfo);
        }
    }
}
