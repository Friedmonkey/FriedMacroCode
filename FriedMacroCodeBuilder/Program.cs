using FriedMacroCode;
using FriedMacroCode.LooseParser;
using static System.Net.Mime.MediaTypeNames;

namespace FriedMacroCodeBuilder;

internal class Program
{
    static void Main(string[] args)
    {
        Console.WriteLine(string.Join(' ', args));
        ParserSettings options = new ParserSettings();
#if DEBUG
        string? filename = "../../../../Examples/DatapackTest/main.fmc";
#else
        string? filename = null;

        List<string> newArgs = args.ToList();
        while (newArgs.Count() > 0)
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

                case "-macros":
                case "-macro":
                case "-m":
                    options.ExpandMacros = true;
                    break;

                case "-output":
                case "-o":
                    options.CompileOutput = getValue();
                    break;
                default:
                    filename = newArgs.FirstOrDefault();
                    break;

            }
            newArgs = newArgs.Skip(2).ToList();
            string getValue() => newArgs.Skip(1).FirstOrDefault() ?? throw new Exception("Expected a value after "+type);
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
            options.CompileOutput = dirName + ".fmh";
        }
        else if (options.ExpandMacros)
        {
            string ext = Path.GetExtension(filename);
            options.CompileOutput = filename.Substring(0, filename.Length - ext.Length)+"_expanded"+ext;
        }

        options.Text = text;
        options.Logger = null; //not yet made
        Parser parser = new Parser(options);
        parser.Parse();
    }
}
