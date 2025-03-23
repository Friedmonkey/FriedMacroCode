using System.Text;

namespace FriedMacroCode.LooseParser;

public partial class Parser
{
    //Syntax: !include "somefille.txt"
    HashSet<string> IncludedFiles = new HashSet<string>();
    public string ParseIncludes()
    {
        context.Scope = "includes";
        string currentPath = Directory.GetCurrentDirectory();
        StringBuilder sb = new StringBuilder(Analizable.Count());

        while (Safe) 
        {
            if (FindStart("!include "))
            {
                bool isLibray = (Current == '<');
                string filename = isLibray ? ConsumeTag() : ConsumeString();

                if (!File.Exists(filename))
                    throw new FileNotFoundException($"Trying to include file: \"{filename}\" but the file cannot be found!");
                filename = Path.GetFullPath(filename);

                if (IncludedFiles.Add(filename))
                {
                    string content = File.ReadAllText(filename);
                    content += $"\n!setpath \"{currentPath}\"\n";
                    currentPath = Path.GetDirectoryName(filename);
                    Directory.SetCurrentDirectory(currentPath);

                    this.Analizable.InsertRange(Position, content);
                    //sb.Append(content);
                }
                else
                {
                    logger?.LogInfo($"Skipped second include of file \"{filename}\". It was not included again, because it was already included!");
                }
            }
            else if (FindStart("!setpath "))
            {   //for internal use
                string pathname = ConsumeString();
                if (!Directory.Exists(pathname))
                    throw new FileNotFoundException($"Trying to set current path: \"{pathname}\" but the directory cannot be found!");

                currentPath = Path.GetFullPath(pathname);
                Directory.SetCurrentDirectory(currentPath);
            }
            else
            {
                sb.Append(Current);
                Position++;
            }
        }

        return sb.ToString();
    }
}
