namespace FriedMacroCode.ParserFiles;

public partial class Parser
{
    public void ParseInclude()
    {
        Directory.SetCurrentDirectory(Path.GetDirectoryName(Current.Origin));
        var filename = GetString();
        if (!File.Exists(filename))
            throw new FileNotFoundException(filename);

        var oldOrgin = this.GetOrginContext();  
        var text = File.ReadAllText(filename);
        ParserOptions options = new ParserOptions()
        {
            Text = text,
            Origin = filename,
            ShowTokens = true,
            Logger = this.logger
        };
        Parser parser = new Parser(options);
        this.Analizable.InsertRange(Position, parser.Analizable);
        this.SetOrginContext(oldOrgin);
    }
}
