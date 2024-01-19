using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace bangla;
internal class Interpreter(List<Token> tokens, int level)
{
    private readonly List<Token> tokens = tokens;
    private readonly List<Token>  variableInitialized = new();
    private void clean()
    {
        foreach (Token token in variableInitialized)
        {
            Global.memory.Peek().Remove(token.getName());
        }
    }
    private readonly int Level = level;
    private bool allowedInWrite(Token token) =>token.getType()==Global.STRING|| Global.isOperators(token.getType()) || token.getType() == Global.COMMA || token.getType() == Global.FUNCTION || Global.isDataType(token.getType()) || Global.isBrackets(token.getType());
    private bool allowedInRead(Token token) => Global.isDataType(token.getType()) || Global.COMMA == token.getType();
    private bool allowedInWhile(Token token) => Global.isOperators(token.getType()) || token.getType() == Global.FUNCTION || Global.isDataType(token.getType()) || Global.isBrackets(token.getType());
    public Token Evaluate()
    {
        for (var i = 0; i < tokens.Count; i++)
        {
            //<---Write--->
            if (tokens[i].getType() == Global.WRITE)
            {
                var j = i + 1;
                if (tokens[j].getType() != Global.LEFT_FIRST)
                {
                    Error error = new Error(tokens[j], "Expected (");
                    error.Execute();
                }
                for (; j < tokens.Count && tokens[j].getType() != Global.SEMI_COLON; j++)
                {
                    if (!allowedInWrite(tokens[j]))
                    {
                        Error error = new Error(tokens[j], "Illegal Character  ");
                        error.Execute();
                    }
                }
                if (j == tokens.Count)
                {
                    Error error = new Error(tokens[j - 1], "Finish Your code,OR Expected ;");
                    error.Execute();
                }
                if (tokens[j - 1].getType() != Global.RIGHT_FIRST)
                {
                    Error error = new Error(tokens[j - 1], "Expected )");
                    error.Execute();
                }
                i += 2;
                while (i < j - 1)
                {
                    List<Token> list = new List<Token>();
                    list.Add(new Token(Global.LEFT_FIRST, "", "", 0, 0, 0));
                    while (i < j - 1 && tokens[i].getType() != Global.COMMA) list.Add(tokens[i++]);
                    list.Add(new Token(Global.RIGHT_FIRST, "", "", 0, 0, 0));
                    i++;
                    Expressions expressions = new Expressions(list);
                    Console.Write(Converter.OUT(expressions.evaluate() + " "));
                    Global.cleanUp();
                }
                i = j;
                Console.WriteLine();
            }
            //<---READ--->
            else if (tokens[i].getType() == Global.READ)
            {
                List<Token> list = new List<Token>();
                var j = ++i;
                if (tokens[j++].getType() != Global.LEFT_FIRST)
                {
                    Error error = new Error(tokens[j], "Expected (");
                    error.Execute();
                }
                for (; j < tokens.Count && tokens[j].getType() != Global.RIGHT_FIRST; j++)
                {
                    if (tokens[j].getType() == Global.LEFT_THIRD)
                    {
                        if (tokens[j + 1].getType() == Global.REAL && tokens[j + 2].getType() == Global.RIGHT_THIRD)
                        {
                            list[list.Count - 1].setIndex(int.Parse(tokens[j + 1].getValue()));
                            j += 2;
                            continue;
                        }
                    }
                    if (!allowedInRead(tokens[j]))
                    {
                        Error error = new Error(tokens[j], "Illegal Character ");
                        error.Execute();
                    }
                    else if (tokens[j].getType() != Global.COMMA) list.Add(tokens[j]);
                }
                j++;
                if (j >= tokens.Count || tokens[j].getType() != Global.SEMI_COLON)
                {
                    Error error = new Error(tokens[j - 1], "Finish Your code,OR Expected ;");
                    error.Execute();
                }
                if (list.Count == 1 && (list[0].getType() == Global.STRING_VARIABLE || list[0].getType() == Global.STRING_ARRAY))
                {
                    list[0].setValue(Converter.IN(Console.ReadLine() + ""));
                }
                else if (list.Count >= 1)
                {
                    List<string> temp = new();
                    while (temp.Count < list.Count)
                    {
                        foreach (var item in Converter.IN(Console.ReadLine() + "").Split())
                        {
                            if(item.Length>0)
                            temp.Add(item);
                        }
                    }
                    for (var k = 0; k < list.Count; k++)
                    {
                        list[k].setValue(temp[k]);
                    }
                }
                i = j;
            }

            //<---While--->
            else if (tokens[i].getType() == Global.WHILE)
            {
                List<Token> list1 = new List<Token>();
                List<Token> list2 = new List<Token>();
                i++;
                list1.Add(new Token(Global.LEFT_FIRST, "", "", 0, 0, 0));
                var balance = 0;
                if (i < tokens.Count && tokens[i].getType() == Global.LEFT_FIRST)
                {
                    i++; balance++;
                    while (i < tokens.Count)
                    {
                        if (tokens[i].getType() == Global.RIGHT_FIRST) balance--;
                        else if (tokens[i].getType() == Global.LEFT_FIRST) balance++;
                        list1.Add(tokens[i]);
                        if (balance == 0 && tokens[i].getType() == Global.RIGHT_FIRST) break;
                        i++;
                    }
                    if (i == tokens.Count)
                    {
                        Error error = new Error(tokens[i], "Finish Your Code");
                        error.Execute();
                    }
                    if (tokens[++i].getType() != Global.LEFT_SECOND)
                    {
                        Error error = new Error(tokens[i], "Expected {");
                        error.Execute();
                    }
                    balance = 1;
                    i++;
                    while (i < tokens.Count)
                    {
                        if (tokens[i].getType() == Global.RIGHT_SECOND) balance--;
                        else if (tokens[i].getType() == Global.LEFT_SECOND) balance++;
                        list2.Add(tokens[i]);
                        if (balance == 0 && tokens[i].getType() == Global.RIGHT_SECOND) break;
                        i++;
                    }
                    list2.RemoveAt(list2.Count - 1);
                    Expressions expressions = new Expressions(list1);
                    Interpreter itr = new Interpreter(list2, 2);
                    while (expressions.evaluate() != "0")
                    {
                        Global.cleanUp();
                        var a = itr.Evaluate();
                        Global.cleanUp();
                        if (a.getType() == "3") return a;
                        if (a.getType() == "2") break;
                    }
                    Global.cleanUp();
                }
                else
                {
                    Error error = new Error(tokens[i + 1], "Expected (");
                    error.Execute();
                }
            }

            else if (tokens[i].getType() == Global.IF)
            {
                List<Token> list1 = new List<Token>();
                List<Token> list2 = new List<Token>();
                i++;
                list1.Add(new Token(Global.LEFT_FIRST, "", "", 0, 0, 0));
                var balance = 0;
                if (i < tokens.Count && tokens[i].getType() == Global.LEFT_FIRST)
                {
                    i++; balance++;
                    while (i < tokens.Count)
                    {
                        if (tokens[i].getType() == Global.RIGHT_FIRST) balance--;
                        else if (tokens[i].getType() == Global.LEFT_FIRST) balance++;
                        list1.Add(tokens[i]);
                        if (balance == 0 && tokens[i].getType() == Global.RIGHT_FIRST) break;
                        i++;
                    }
                    if (i == tokens.Count)
                    {
                        Error error = new Error(tokens[i], "Finish Your Code");
                        error.Execute();
                    }
                    if (tokens[++i].getType() != Global.LEFT_SECOND)
                    {
                        Error error = new Error(tokens[i], "Expected {");
                        error.Execute();
                    }
                    balance = 1;
                    i++;
                    while (i < tokens.Count)
                    {
                        if (tokens[i].getType() == Global.RIGHT_SECOND) balance--;
                        else if (tokens[i].getType() == Global.LEFT_SECOND) balance++;
                        list2.Add(tokens[i]);
                        if (balance == 0 && tokens[i].getType() == Global.RIGHT_SECOND) break;
                        i++;
                    }
                    list2.RemoveAt(list2.Count - 1);
                    Expressions expressions = new Expressions(list1);
                    Interpreter itr = new Interpreter(list2, 1);
                    if (expressions.evaluate() != "0")
                    {
                        Global.cleanUp();
                        var a = itr.Evaluate();
                        Global.cleanUp();
                        if (a.getType() == "3") return a;
                        if (a.getType() == "2") return a;
                        if (a.getType() == "1") return a;
                    }
                    Global.cleanUp();
                }
                else
                {
                    Error error = new Error(tokens[i + 1], "Expected (");
                    error.Execute();
                }
            }


            //<---Initializers--->

            //<---Integer--->
            else if (tokens[i].getType() == Global.INTEGER_INITIALIZER)
            {
                var j = ++i;
                if (tokens[j].getType() == Global.VARIABLE)
                {
                    if (tokens[j + 1].getType() == Global.LEFT_THIRD && tokens[j + 3].getType() == Global.RIGHT_THIRD)
                    {
                        tokens[j].setType(Global.INTEGER_ARRAY);
                        Global.memory.Peek().Add(tokens[j].getName(), new NODE(Global.INTEGER_ARRAY, int.Parse(tokens[j + 2].getValue() + 1)));
                        j += 4;
                    }
                    else
                    {
                        tokens[j].setType(Global.INTEGER_VARIABLE);
                        Global.memory.Peek().Add(tokens[j].getName(), new NODE(Global.INTEGER_VARIABLE, 1));
                    }
                }
                for (; j < tokens.Count && tokens[j].getType() != Global.SEMI_COLON; j++)
                {
                    if (tokens[j].getType() == Global.COMMA)
                    {
                        if (tokens[j + 2].getType() == Global.LEFT_THIRD && tokens[j + 4].getType() == Global.RIGHT_THIRD)
                        {
                            tokens[j + 1].setType(Global.INTEGER_ARRAY);
                            Global.memory.Peek().Add(tokens[j + 1].getName(), new NODE(Global.INTEGER_ARRAY, int.Parse(tokens[j + 3].getValue()) + 1));
                            j += 4;
                        }
                        else if (tokens[j + 1].getType() == Global.VARIABLE)
                        {
                            tokens[j + 1].setType(Global.INTEGER_VARIABLE);
                            Global.memory.Peek().Add(tokens[j + 1].getName(), new NODE(Global.INTEGER_VARIABLE, 1)); j++;
                        }
                    }
                }
                while (i < j - 1)
                {
                    List<Token> list = new List<Token>();
                    list.Add(new Token(Global.LEFT_FIRST, "", "", 0, 0, 0));
                    while (i < j && tokens[i].getType() != Global.COMMA) list.Add(tokens[i++]);
                    list.Add(new Token(Global.RIGHT_FIRST, "", "", 0, 0, 0));
                    i++;
                    Expressions expressions = new Expressions(list);
                    expressions.evaluate();
                    Global.cleanUp();
                }
                i = j;
            }

            //<---Float--->
            else if (tokens[i].getType() == Global.REAL_INITIALIZER)
            {
                var j = ++i;
                if (tokens[j].getType() == Global.VARIABLE)
                {
                    if (tokens[j + 1].getType() == Global.LEFT_THIRD && tokens[j + 3].getType() == Global.RIGHT_THIRD)
                    {
                        tokens[j].setType(Global.REAL_ARRAY);
                        Global.memory.Peek().Add(tokens[j].getName(), new NODE(Global.REAL_ARRAY, int.Parse(tokens[j + 2].getValue() + 1)));
                        j += 4;
                        variableInitialized.Add(tokens[j]);
                    }
                    else
                    {
                        tokens[j].setType(Global.REAL_VARIABLE);
                        variableInitialized.Add(tokens[j]);
                        Global.memory.Peek().Add(tokens[j].getName(), new NODE(Global.REAL_VARIABLE, 1));
                    }
                }
                for (; j < tokens.Count && tokens[j].getType() != Global.SEMI_COLON; j++)
                {
                    if (tokens[j].getType() == Global.COMMA)
                    {
                        if (tokens[j + 2].getType() == Global.LEFT_THIRD && tokens[j + 4].getType() == Global.RIGHT_THIRD)
                        {
                            tokens[j + 1].setType(Global.REAL_ARRAY);
                            Global.memory.Peek().Add(tokens[j + 1].getName(), new NODE(Global.REAL_ARRAY, int.Parse(tokens[j + 3].getValue()) + 1));
                            variableInitialized.Add(tokens[j + 1]);
                            j += 4;
                        }
                        else if (tokens[j + 1].getType() == Global.VARIABLE)
                        {
                            tokens[j + 1].setType(Global.REAL_VARIABLE);
                            Global.memory.Peek().Add(tokens[j + 1].getName(), new NODE(Global.REAL_VARIABLE, 1)); j++;
                            variableInitialized.Add(tokens[j + 1]);
                        }
                    }
                }
                while (i < j - 1)
                {
                    List<Token> list = new List<Token>();
                    list.Add(new Token(Global.LEFT_FIRST, "", "", 0, 0, 0));
                    while (i < j && tokens[i].getType() != Global.COMMA) list.Add(tokens[i++]);
                    list.Add(new Token(Global.RIGHT_FIRST, "", "", 0, 0, 0));
                    i++;
                    Expressions expressions = new Expressions(list);
                    expressions.evaluate();
                    Global.cleanUp();
                }
                i = j;
            }

            //<---String--->
            else if (tokens[i].getType() == Global.STRING_INITIALIZER)
            {
                var j = ++i;
                if (tokens[j].getType() == Global.VARIABLE)
                {
                    if (tokens[j + 1].getType() == Global.LEFT_THIRD && tokens[j + 3].getType() == Global.RIGHT_THIRD)
                    {
                        tokens[j].setType(Global.STRING_ARRAY);
                        Global.memory.Peek().Add(tokens[j].getName(), new NODE(Global.STRING_ARRAY, int.Parse(tokens[j + 2].getValue() + 1)));
                        j += 4;
                    }
                    else
                    {
                        tokens[j].setType(Global.STRING_VARIABLE);
                        Global.memory.Peek().Add(tokens[j].getName(), new NODE(Global.STRING_VARIABLE, 1));
                    }
                }
                for (; j < tokens.Count && tokens[j].getType() != Global.SEMI_COLON; j++)
                {
                    if (tokens[j].getType() == Global.COMMA)
                    {
                        if (tokens[j + 2].getType() == Global.LEFT_THIRD && tokens[j + 4].getType() == Global.RIGHT_THIRD)
                        {
                            tokens[j + 1].setType(Global.STRING_ARRAY);
                            Global.memory.Peek().Add(tokens[j + 1].getName(), new NODE(Global.STRING_ARRAY, int.Parse(tokens[j + 3].getValue()) + 1));
                            j += 4;
                        }
                        else if (tokens[j + 1].getType() == Global.VARIABLE)
                        {
                            tokens[j + 1].setType(Global.STRING_VARIABLE);
                            Global.memory.Peek().Add(tokens[j + 1].getName(), new NODE(Global.STRING_VARIABLE, 1)); j++;
                        }
                    }
                }
                while (i < j - 1)
                {
                    List<Token> list = new List<Token>();
                    list.Add(new Token(Global.LEFT_FIRST, "", "", 0, 0, 0));
                    while (i < j && tokens[i].getType() != Global.COMMA) list.Add(tokens[i++]);
                    list.Add(new Token(Global.RIGHT_FIRST, "", "", 0, 0, 0));
                    i++;
                    Expressions expressions = new Expressions(list);
                    expressions.evaluate();
                    Global.cleanUp();
                }
                i = j;
            }
            else if (tokens[i].getType() == Global.RETURN)
            {
                var j = i + 1;
                List<Token> token = new List<Token>();
                token.Add(new Token(Global.LEFT_FIRST, "", "", 0, 0, 0));
                while (j < tokens.Count && tokens[j].getType() != Global.SEMI_COLON) if (allowedInWhile(tokens[j])) token.Add(tokens[j++]);
                    else
                    {
                        Error error = new Error(tokens[j], "Illegal Character ,Or Expected ;"); error.Execute();
                    }
                if (j == tokens.Count) { Error error = new Error(tokens[j], "Finish Your Code"); error.Execute(); }
                token.Add(new Token(Global.RIGHT_FIRST, "", "", 0, 0, 0));
                Expressions expressions = new Expressions(token);
                clean();
                return new Token("3", expressions.evaluate(), "", tokens[i].getStart(), tokens[i].getEnd(), tokens[i].getLine());
            }
            else if (tokens[i].getType() == Global.BREAK)
            {

            }
            else
            {
                List<Token> list = new List<Token>();
                list.Add(new Token(Global.LEFT_FIRST,"","",0,0, 0));
                for (; i < tokens.Count && tokens[i].getType() != Global.SEMI_COLON; i++)
                {
                    if (allowedInWhile(tokens[i])) list.Add(tokens[i]);
                    else
                    {
                        Error error = new Error(tokens[i], "Illegal Character");
                        error.Execute();
                    }
                }
                list.Add(new Token(Global.RIGHT_FIRST, "", "", 0, 0, 0));
                Expressions expressions = new Expressions(list);
                expressions.evaluate();
                Global.cleanUp();
            }
        }
        clean();
        return new Token("", "", "", 0, 0, 0);
    }
}
