using FriedMacroCode;
using FriedMacroCode.LooseParser;
using static System.Net.Mime.MediaTypeNames;

namespace FriedMacroCodeBuilder;

internal class Program
{
    static void Main(string[] args)
    {
#if DEBUG
  //      string filename = "../../../../Examples\\build.fmc";
//#else
        string? filename = null;
        ParserSettings options = new ParserSettings();

        if (args.Count() == 1)
            filename = args[0];
        else if (args.Count() > 1)
        {
            List<string> newArgs = args.ToList();
            while (args.Count() > 0)
            {
                string type = newArgs.First().ToLower();
                switch (type)
                {
                    case "-file":
                    case "-f":
                        filename = getValue();
                        break;

                    case "-compile":
                    case "-c":
                        options.IncludeOnly = true;
                        break;

                    case "-compilename":
                    case "-cname":
                    case "-cn":
                        options.CompileOutput = getValue();
                        options.IncludeOnly = true;
                        break;

                }
                newArgs = newArgs.Skip(2).ToList();
                string getValue() => newArgs.Skip(1).FirstOrDefault() ?? throw new Exception("Expected a value after "+type);
            }
        }

#endif
        if (string.IsNullOrEmpty(filename))
        {
            Console.Write("Enter file name \n>");
            filename = Console.ReadLine();
        }

        if (!File.Exists(filename))
            throw new FileNotFoundException(filename);
        filename = Path.GetFullPath(filename);

        var text = File.ReadAllText(filename);
        var dirName = Path.GetDirectoryName(filename);
        if (string.IsNullOrEmpty(dirName))
            throw new Exception("unable to resolve directory name!");

        Directory.SetCurrentDirectory(dirName);

        if (options.IncludeOnly && string.IsNullOrEmpty(options.CompileOutput))
        {
            options.CompileOutput = dirName;
        }

        options.Text = text;
        options.Logger = null; //not yet made
        Parser parser = new Parser(options);
        parser.Parse();
    }
}
