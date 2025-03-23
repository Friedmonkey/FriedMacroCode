using System.Text;

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
    public bool FindStartAny(out string? found, params string[] find)
    {
        foreach (var findable in find)
        {
            if (FindStart(findable))
            {
                found = findable;
                return true;
            }
        }
        found = null;
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
    private string ConsumeTag()
    {
        Consume('<');
        string str = ConsumeUntil('>');
        Consume('>');
        return str;
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
        var output = new StringBuilder();
        while (Safe && Current != end)
        {
            output.Append(Current);
            Position++;
        }
        return output.ToString();
    }
    //public string ConsumeUntil(string end)
    //{
    //    var output = new StringBuilder();
    //    while (Safe && !Find(end))
    //    {
    //        output.Append(Current);
    //        Position++;
    //    }
    //    return output.ToString();
    //}
    public string ConsumeUntilEnter()
    {
        var output = new StringBuilder();
        while (Safe && !Current.IsEnter())
        {
            output.Append(Current);
            Position++;
        }
        return output.ToString();
    }
    public string ConsumeUntilWhitespace()
    {
        var output = new StringBuilder();
        while (Safe && !char.IsWhiteSpace(Current))
        {
            output.Append(Current);
            Position++;
        }
        return output.ToString();
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
