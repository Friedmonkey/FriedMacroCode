namespace FriedMacroCode.LooseParser;

public partial class Parser
{
    public void Parse()
    {
        List<string> rawValues = new List<string>();
        var text = ParseText(ref rawValues);
        if (includeOnly || expandMacros)
        { 
            Path.GetDirectoryName(text);
            File.WriteAllText(compileOutput ?? throw new Exception("No compile name specified!"), text);
        }
        else
            RunLua(text, rawValues);
    }
}
