using NLua;
//using System.Text.Json;
using Newtonsoft.Json;

namespace FriedMacroCode.LooseParser;

public partial class Parser
{
    const string CurrentBuffer = "_currentBuffer";
    const string RawValues = "_rawValues";
    const string Interop = "_luaInterop";
    public class LuaInterop
    {
        public void WriteFile(string path, string content)
        {
            var dir = Path.GetDirectoryName(path);
            if (!Directory.Exists(dir))
                Directory.CreateDirectory(dir);
            
            File.WriteAllText(path, content);
        }
        public string Escape(string input)
        {
            return JsonConvert.ToString(input)![1..^1]; // Removes extra quotes
        }
        public string FormatJson(string uglyJson)
        {
            return JsonConvert.SerializeObject(JsonConvert.DeserializeObject(uglyJson), Formatting.Indented);
        }
    }
    public Lua GetLua()
    {
        Lua lua = new Lua();

        lua.DoString("import = function () end");
        LuaInterop interop = new LuaInterop();
        lua[Interop] = interop;

        return lua;
    }
    public string ExecuteLuaOutput(Lua lua, string code)
    {
        var output = lua.DoString(code);

        // Return the Lua result as a string
        if (output != null && output.Count() > 0)
        {
            return output[0].ToString(); // Get the first result of the Lua execution
        }
        else
        {
            return string.Empty; // Return empty if no output from Lua
        }
    }
    public string RunLua(string code, List<string> rawValues)
    {
        try
        {
            using (Lua lua = GetLua())
            {
                lua[CurrentBuffer] = string.Empty;

                // Convert the C# List to a Lua table
                lua.NewTable(RawValues);
                var luaTable = lua.GetTable(RawValues);
                for (int i = 0; i < rawValues.Count; i++)
                {
                    luaTable[i + 1] = rawValues[i]; // Lua tables are 1-indexed
                }

                // Pass the Lua table to Lua
                lua[RawValues] = luaTable;

                return ExecuteLuaOutput(lua, code);
            }
        }
        catch (Exception ex)
        {
            // Log or inspect the exception message
            Console.WriteLine($"Error executing Lua: {ex.Message}");
            return $"Error: {ex.Message}";
        }
    }
}
