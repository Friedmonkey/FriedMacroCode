using FriedMacroCode;

namespace FriedMacroCodeBuilder;

internal class Program
{
    static void Main(string[] args)
    {
        Console.WriteLine("Enter file name \n>");
        string filename = Console.ReadLine();
        //string filename = args[0];

        if (!File.Exists(filename))
            throw new FileNotFoundException(filename);

        var text = File.ReadAllText(filename);
        ParserOptions options = new ParserOptions() 
        {
            Text = text,
            Logger = null //not yet made
        };
        Parser parser = new Parser(options);
    }
}
