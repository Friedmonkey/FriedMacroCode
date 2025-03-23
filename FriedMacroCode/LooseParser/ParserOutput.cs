using System.Text;

namespace FriedMacroCode.LooseParser;

public partial class Parser
{
    public string ParseOutput(ref List<string> rawValues)
    {
        context.Scope = "output";
        var output = new StringBuilder();
        var raw = new StringBuilder();
        while (Safe)
        {
            if (Find("<rawdata>")) //if indentation and whitespace matters
            {
                raw.Clear();
                while (Safe && !Find("</rawdata>"))
                {
                    raw.Append(Current);
                    Position++;
                }
                rawValues.Add(raw.ToString());
                AddData(rawValues.Count());
            }
            else if (Find("<raw>")) //if indentation and whitespace dont really matter
            {
                raw.Clear();
                SkipWhitespace();
                while (Safe && !Find("</raw>"))
                {
                    if (Current.IsEnter())
                    {
                        raw.Append('\n');
                        SkipWhitespace();
                    }
                    else
                    { 
                        raw.Append(Current);
                        Position++;
                    }
                }
                rawValues.Add(raw.ToString());
                AddData(rawValues.Count());
            }
            else
            {
                output.Append(Current);
                Position++;
            }
        }
        //return output.ToString();

        string code = output.ToString().Replace("\n\r", "\n").Replace("\r", "").Replace("\n\n", "\n");
        output.Clear();
        string[] lines = code.Split('\n');
        for (int i = 0; i < lines.Length; i++)
        {
            string line = lines[i].Trim();
            if (string.IsNullOrWhiteSpace(line)) continue;
            if (line.StartsWith("//")) continue; //remove comments
            //filter out any leftover ignores
            if (line.StartsWith("!ignore", StringComparison.OrdinalIgnoreCase)) continue;
            if (line.StartsWith("!endignore", StringComparison.OrdinalIgnoreCase)) continue;

            //handle singleline rawdata
            if (!line.StartsWith('#'))
            {
                output.AppendLine(line);
                continue;
            }

            var rawValue = new StringBuilder();
            rawValue.Append(line.Substring(1));
            int lookahead = 1;
            string tmpVal = string.Empty;
            while ((tmpVal = lines[lookahead + i].Trim()).StartsWith('-'))
            {
                lookahead++;
                rawValue.Append(tmpVal.Substring(1));
            }

            i += --lookahead;

            rawValues.Add(rawValue.ToString());
            AddData(rawValues.Count());
        }
        return output.ToString();
        void AddData(int count)
        {
            output.AppendLine($"_currentBuffer = _currentBuffer .. _rawValues[{count}]");
        }
    }
}
