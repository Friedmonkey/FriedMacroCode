namespace FriedMacroCode.Parser;

public partial class Parser
{
    public string Parse()
    {
        var text = ParseText();
        return RunLua(text);
    }
}
