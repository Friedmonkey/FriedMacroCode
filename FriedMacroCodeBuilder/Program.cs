using FriedMacroCode;
using FriedMacroCode.LooseParser;

namespace FriedMacroCodeBuilder;

internal class Program
{
    static void Main(string[] args)
    {
#if DEBUG
        string filename = "../../../../Examples/Datapack1\\main.fmc";
#else
        string filename;
        if (args.Count() > 0)
        {
            filename = args[0];
        }
        else 
        {
            Console.Write("Enter file name \n>");
            filename = Console.ReadLine();
        }
#endif
        //string filename = args[0];

        if (!File.Exists(filename))
            throw new FileNotFoundException(filename);

        var text = File.ReadAllText(filename);
        Directory.SetCurrentDirectory(Path.GetDirectoryName(filename));
        //ParserOptions options = new ParserOptions() 
        //{
        //    Text = text,
        //    Origin = filename,
        //    ShowTokens = true,
        //    Logger = null //not yet made
        //};
        ParserSettings options = new ParserSettings()
        {
            Text = text,
            Logger = null //not yet made
        };
        Parser parser = new Parser(options);
        parser.Parse();
    }
}
