using FriedLexer;
using System.Runtime.CompilerServices;
using System.Text;

namespace FriedMacroCode.LooseParser;

public partial class Parser
{
    public record Macro(string name, List<string> args, string macroBody);
    List<Macro> Macros = new List<Macro>();
    public string ParseMacros()
    {
        context.Scope = "macros";
        StringBuilder sb = new StringBuilder(Analizable.Count());

        while (Safe) 
        {
            if (FindStartAny(out string? keyword, "!define ", "!redefine ", "!trydefine ", "!ifdefine "))
            {
                var macro = ParseMacroDefinition();
                var existingMacro = Macros.FirstOrDefault(m => m.name == macro.name);
                if (existingMacro is null)
                {
                    if (keyword == "!ifdefine ")
                    {   //somewhat add pre processing stuff using this ifdefine
                        macro = macro with { macroBody = string.Empty };
                    }
                    Macros.Add(macro);
                    logger?.LogDetail($"Macro {macro.name} has been added!");
                }
                else
                {
                    if (keyword is "!ifdefine " or "!redefine ")
                    {   //we already found it but these keyword allow this
                        Macros.Remove(macro);
                        Macros.Add(macro);
                    }
                    else if (keyword != "!trydefine ")
                        logger?.LogWarning($"Macro {macro.name} already exists and hasnt been added!");
                }

                continue;
            }
            else
            {
                bool foundMacro = false;
                if (Current == '@' && char.IsLetter(Peek(1)))
                {
                    Consume('@');
                    foreach (var macro in Macros)
                    {
                        if (Find(macro.name + '('))
                        {
                            List<string> arguments = ParseMacroArguments();

                            logger?.LogDetail($"Expanding macro:{macro.name}");
                            ExpandMacro(macro, arguments);
                            foundMacro = true;
                            break;
                        }
                    }
                    if (!foundMacro)
                    {
                        Position--;
                    }
                }

                if (!foundMacro)
                { 
                    sb.Append(Current);
                    Position++;
                }
            }
        }

        return sb.ToString();
    }

    private List<string> ParseMacroArguments()
    {
        List<string> arguments = new List<string>();
        var argBuffer = new StringBuilder();
        while (Safe && Current != ')')
        {
            argBuffer.Clear();
            if (Find($"<arg"))
            {
                string num = ConsumeUntil('>');
                Consume('>');
                while (!Find($"</arg{num}>"))
                {
                    argBuffer.Append(Current);
                    Position++;
                }
            }
            else
            { 
                while (Current != ',' && Current != ')')
                {
                    SkipWhitespace();
                    argBuffer.Append(Current);
                    Position++;
                }
            }
            arguments.Add(argBuffer.ToString());
            if (Current == ')') break;
            Consume(',');
        }
        Consume(')');
        return arguments;
    }

    private void ExpandMacro(Macro macro, List<string> parameters)
    {
        int definitionArgCount = macro.args.Count;
        int parameterCount = parameters.Count;

        if (definitionArgCount != parameterCount)
            throw new ArgumentException($"Error on macro {macro.name}. Expected {definitionArgCount} arguments but got {parameterCount}");

        string expandedMacro = macro.macroBody;

        for (int i = 0; i < macro.args.Count; i++) 
        {
            string? arg = macro.args[i];
            expandedMacro = expandedMacro.Replace(arg, parameters[i]);
        }

        this.Analizable.InsertRange(Position, expandedMacro);
    }

    public Macro ParseMacroDefinition()
    {
        context.Scope = "macro definition";
        List<string> macroArgs = new List<string>();
        var macroName = new StringBuilder();
        var macroBody = new StringBuilder();

        //get Name
        while (Safe && !(Current == '(' || char.IsWhiteSpace(Current)))
        { 
            macroName.Append(Current);
            Position++;
        }
        context.ExtraInfo = $"name:{macroName}";

        //if it has a body
        if (Current == '(')
        {
            Consume('(');

            ParseMacroDefinitionArguments(ref macroArgs);

            SkipWhitespaceAndComments();

            //macro can either be single line or multiline {{ anything here even enters \n }}
            if (Find("{{"))
            {   //its multiline parse until closing
                while (Safe && !Find("}}"))
                {
                    macroBody.Append(Current);
                    Position++;
                }
            }
            else
            {
                macroBody.Append(ConsumeUntilEnter());
            }
        }
        else
        {
            if (Current == ' ')
                Consume(' ');
            if (!Current.IsEnter())
                macroBody.Append(ConsumeUntilEnter());
        }

        //for (int i = 0; i < macroArgs.Count; i++) 
        //{
        //    string arg = macroArgs[i];
        //    macroBody.Replace(arg, $"{{{i}}}");
        //}

        return new Macro(macroName.ToString(), macroArgs, macroBody.ToString());
    }

    private void ParseMacroDefinitionArguments(ref List<string> macroArgs)
    {
        var argBuffer = new StringBuilder();
        while (Safe && Current != ')')
        {
            argBuffer.Clear();
            while (Safe && Current != ')' && Current != ',')
            {
                SkipWhitespace();
                argBuffer.Append(Current);
                Position++;
            }
            string argument = argBuffer.ToString();
            if (string.IsNullOrWhiteSpace(argument))
                throw new Exception("Macro definition failed, got empty argument name!");

            macroArgs.Add(argument);
            if (Current == ')') break;
            Consume(',');
        }
        Consume(')');
    }
}
