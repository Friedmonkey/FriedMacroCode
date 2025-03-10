using NLua;

namespace FriedMacroCode.Parser;

public partial class Parser
{
    public string RunLua(string code)
    {
        Lua lua = new Lua();
        var output = lua.DoString(code);
        return output.ToString();
    }
}
