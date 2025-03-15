﻿using FriedLexer;
using System.Runtime.CompilerServices;
using System.Text;

namespace FriedMacroCode.LooseParser;

public partial class Parser
{
    public record Macro(string name, List<string> args, string macroBody, bool doubleDefine = false);
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
                            List<string> arguments = new List<string>();

                            ParseMacroArguments(ref arguments, macro.doubleDefine);

                            logger?.LogDetail($"Expanding macro:{macro.name}");
                            ExpandMacro(macro, arguments);
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

    private void ParseMacroArguments(ref List<string> arguments, bool doubleDefine)
    {
        var argBuffer = new StringBuilder();
        while (Safe && Current != ')')
        {
            argBuffer.Clear();
            while (Current != ',' && Current != ')')
            {
                SkipWhitespace();
                argBuffer.Append(Current);
                Position++;
            }
            if (doubleDefine && Current == ',') throw new Exception("DoubleDefined macros does not support multiple arguments");
            arguments.Add(argBuffer.ToString());
            if (Current == ')') break;
            Consume(',');
        }
        Consume(')');
        if (doubleDefine && Current == ')') Consume(')');
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
        bool doubleDefine = false;

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
            if (Current == '(')
            {   //defined with double ( meaning we need double ( to pass arguments so we can pass arguments that contain ) without closing
                Consume('(');
                doubleDefine = true;
            }

            ParseMacroDefinitionArguments(ref macroArgs, doubleDefine);

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

        return new Macro(macroName.ToString(), macroArgs, macroBody.ToString(), doubleDefine);
    }

    private void ParseMacroDefinitionArguments(ref List<string> macroArgs, bool doubleDefine)
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
                throw new Exception("Macro definition failed, got empty argument!");

            macroArgs.Add(argument);
            if (Current == ')') break;
            Consume(',');
        }
        Consume(')');

        if (Current == ')' && doubleDefine) Consume(')');
    }
}
