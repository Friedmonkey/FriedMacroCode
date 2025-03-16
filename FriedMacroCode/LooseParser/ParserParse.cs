namespace FriedMacroCode.LooseParser;

public partial class Parser
{
    public string Parse()
    {
        List<string> rawValues = new List<string>();
        var text = ParseText(ref rawValues);
        return RunLua(text, rawValues);
    }
}
