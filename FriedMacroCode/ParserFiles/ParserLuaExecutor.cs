using NLua;

namespace FriedMacroCode.ParserFiles;

public partial class Parser
{
    const string CurrentFile = "_currentFile";
    const string CurrentBuffer = "_currentBuffer";
    const string RawValues = "_rawValues";
    const string Interop = "_luaInterop";
    public class LuaInterop
    {
        public static void WriteFile(string path, string content)
        {
            Console.WriteLine($"writing to file \"{path}\" with content:");
            Console.WriteLine(content);
        }
    }
    public string RunLua(string code, List<string> rawValues)
    {
        try
        {
            using (Lua lua = new Lua())
            {
                lua.DoString("import = function () end");
                // Initialize Lua variables
                lua[CurrentBuffer] = string.Empty;
                lua[CurrentFile] = string.Empty;

                // Convert the C# List to a Lua table
                lua.NewTable(RawValues);
                var luaTable = lua.GetTable(RawValues);
                for (int i = 0; i < rawValues.Count; i++)
                {
                    luaTable[i + 1] = rawValues[i]; // Lua tables are 1-indexed
                }

                // Pass the Lua table to Lua
                lua[RawValues] = luaTable;

                LuaInterop interop = new LuaInterop();
                lua[Interop] = interop;

                //// Debugging: Print the Lua array size
                //lua.DoString("print('RawValues count:', #_rawValues)");

                //// Your Lua code
                //code = "for i, v in ipairs(_rawValues) do\r\n    print(i, v)\r\nend\n\n\n";

                // Execute Lua code
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
        }
        catch (Exception ex)
        {
            // Log or inspect the exception message
            Console.WriteLine($"Error executing Lua: {ex.Message}");
            return $"Error: {ex.Message}";
        }
    }
}



