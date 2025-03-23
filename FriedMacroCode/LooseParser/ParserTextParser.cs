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

            if (includeOnly)
                return code;

            code = ParseMacros();
            UpdateAnalizableAndResetIndex();

            if (expandMacros)
                return code;

            code = ParseOutput(ref rawValues);
            UpdateAnalizableAndResetIndex();

            Console.WriteLine(code);
        }
        catch (Exception ex)
        {
            throw ex;
        }
        void UpdateAnalizableAndResetIndex()
        {
            this.Analizable = code.ToList();
            this.Position = 0;
        }
        return code;
    }
}
