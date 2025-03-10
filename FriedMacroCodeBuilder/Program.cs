using FriedMacroCode;
using FriedMacroCode.ParserFiles;

namespace FriedMacroCodeBuilder;

internal class Program
{
    static void Main(string[] args)
    {
#if DEBUG
        string filename = "datapack\\main.fmc";
#else
        Console.Write("Enter file name \n>");
        string filename = Console.ReadLine();
#endif
        //string filename = args[0];

        if (!File.Exists(filename))
            throw new FileNotFoundException(filename);

        var text = File.ReadAllText(filename);
        ParserOptions options = new ParserOptions() 
        {
            Text = text,
            Origin = filename,
            ShowTokens = true,
            Logger = null //not yet made
        };
        Parser parser = new Parser(options);
        parser.Parse();
    }
}
