namespace FriedMacroCode.LooseParser;

public partial class Parser
{
    public bool Find(string find)
    {
        for (int i = 0; i < find.Length; i++)
        {
            if (Peek(i) == find[i])
            {
                continue;
            }
            else return false;
        }
        Position += find.Length;
        return true;
    }
    public bool FindStart(string find)
    {
        if (Find(find))
        {
            int length = find.Length + 1;
            if (Peek(-length).IsEnter())
            {
                return true;
            }
            else
            {
                Console.WriteLine($"`{find}` found, but was not at the start of a line");
                Position -= find.Length;
                return false;
            }
        }
        return false;
    }
    public char Consume(char chr)
    {
        if (Current == chr)
        {
            Position++;
            return chr;
        }
        throw this.context.ExpectedException(Current, chr);
    }
    private string ConsumeString()
    {
        Consume('"');
        string str = ConsumeUntil('"');
        Consume('"');
        return str;
    }
    public string ConsumeUntil(char end)
    {
        string output = string.Empty;
        while (Safe && Current != end)
        {
            output += Current;
            Position++;
        }
        return output;
    }
    public string ConsumeUntilEnter()
    {
        string output = string.Empty;
        while (Safe && !Current.IsEnter())
        {
            output += Current;
            Position++;
        }
        return output;
    }
    public string ConsumeUntilWhitespace()
    {
        string output = string.Empty;
        while (Safe && !char.IsWhiteSpace(Current))
        {
            output += Current;
            Position++;
        }
        return output;
    }
    private void ConsumeComment()
    {
        var comment = ConsumeUntilEnter();
        logger?.LogDetail("Comment removed:" + comment);
        SkipWhitespace();
    }
    public void SkipWhitespace()
    {
        while (Safe && char.IsWhiteSpace(Current))
        {
            Position++;
        }
    }
    public void SkipWhitespaceAndComments()
    {
        SkipWhitespace();
        if (Find("//"))
        {
            ConsumeComment();
            SkipWhitespaceAndComments();
        }
    }
}
