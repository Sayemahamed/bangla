using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace bangla;
internal class Interpreter(List<Token> tokens, int level)
{
    private Token functionCleanUp(ref int i)
    {
        var j = i; var k = i + 1;
        var balance = 0;
        List<Token> list = new List<Token>();
        while (k < tokens.Count)
        {
            if (tokens[k].getType() == Global.LEFT_FIRST) balance++;
            if (tokens[k].getType() == Global.RIGHT_FIRST) balance--;
            if (!allowedInWrite(tokens[k]))
            {
                Error error = new Error(tokens[k], "Illegal Character  ");
                error.Execute();
            }
            if (balance == 0) break;
            k++;
        }
        if (i + 1 != k) i += 2;
        while (i < k)
        {
            List<Token> temp = [new Token(Global.LEFT_FIRST, "", "", 0, 0, 0)];
            while (i < k && tokens[i].getType() != Global.COMMA)
            {
                if (tokens[i].getType() == Global.FUNCTION)
                    temp.Add(functionCleanUp(ref i));
                else temp.Add(tokens[i]);
                i++;
            }
            temp.Add(new Token(Global.RIGHT_FIRST, "", "", 0, 0, 0));
            i++;
            if (temp.Count == 3 && Global.isDataType(temp[1].getType()))
            {
                list.Add(temp[1]);
            }
            else
            {
                Expressions expressions = new Expressions(temp);
                var a = expressions.evaluate();
                if (decimal.TryParse(a, out _)) list.Add(new Token(Global.REAL, a, "", 0, 0, 0));
                else list.Add(new Token(Global.STRING, a, "", 0, 0, 0));
                Global.cleanUp();
            }
        }
        tokens[j].setArg(list);
        i--;
        return tokens[j];
    }
    private readonly List<Token> tokens = tokens;
    private readonly List<Token> variableInitialized = new();
    private void clean()
    {
        foreach (Token token in variableInitialized)
        {
            Global.memory.Peek().Remove(token.getName());
        }
    }
    private readonly int Level = level;
    private bool allowedInWrite(Token token) => token.getType() == Global.REAL || token.getType() == Global.STRING || Global.isOperators(token.getType()) || token.getType() == Global.FUNCTION || Global.isDataType(token.getType()) || Global.isBrackets(token.getType()) || token.getType() == Global.COMMA;
    private bool allowedInRead(Token token) => Global.isDataType(token.getType()) || Global.COMMA == token.getType();
    private bool allowedInWhile(Token token) => token.getType() == Global.REAL || token.getType() == Global.STRING || Global.isOperators(token.getType()) || token.getType() == Global.FUNCTION || Global.isDataType(token.getType()) || Global.isBrackets(token.getType());
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
                            if (item.Length > 0)
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
                var addCondition = true;
                var addCode = true;
                List<List<Token>> conditions = new List<List<Token>>();
                List<List<Token>> code = new List<List<Token>>();
                while ((addCondition || addCode) && i < tokens.Count)
                {
                    if (addCondition)
                    {
                        List<Token> list = new List<Token>();
                        i++;
                        list.Add(new Token(Global.LEFT_FIRST, "", "", 0, 0, 0));
                        var balance = 1;
                        if (i < tokens.Count && tokens[i].getType() == Global.LEFT_FIRST)
                        {
                            i++;
                            while (i < tokens.Count)
                            {
                                if (tokens[i].getType() == Global.RIGHT_FIRST) balance--;
                                else if (tokens[i].getType() == Global.LEFT_FIRST) balance++;
                                list.Add(tokens[i]);
                                if (balance == 0 && tokens[i].getType() == Global.RIGHT_FIRST) break;
                                i++;
                            }
                            if (i == tokens.Count)
                            {
                                Error error = new Error(tokens[i], "Finish Your Code");
                                error.Execute();
                            }
                            conditions.Add(list);
                            addCondition = false;
                        }
                        else
                        {
                            Error error = new Error(tokens[i + 1], "Expected (");
                            error.Execute();
                        }
                    }
                    if (addCode)
                    {
                        if (tokens[++i].getType() != Global.LEFT_SECOND)
                        {
                            Error error = new Error(tokens[i], "Expected {");
                            error.Execute();
                        }
                        var balance = 1;
                        List<Token> list = new List<Token>();
                        i++;
                        while (i < tokens.Count)
                        {
                            if (tokens[i].getType() == Global.RIGHT_SECOND) balance--;
                            else if (tokens[i].getType() == Global.LEFT_SECOND) balance++;
                            list.Add(tokens[i]);
                            if (balance == 0 && tokens[i].getType() == Global.RIGHT_SECOND) break;
                            i++;
                        }
                        if (balance < 0)
                        {
                            Error error = new Error(tokens[Math.Min(i, tokens.Count - 1)], "Expected }");
                            error.Execute();
                        }
                        list.RemoveAt(list.Count - 1);
                        code.Add(list);
                        addCode = false;
                    }
                    if (i < tokens.Count - 1 && tokens[i + 1].getType() == Global.OR)
                    {
                        if (code.Count > conditions.Count)
                        {
                            Error error = new Error(tokens[i + 1], "Illegal Character");
                            error.Execute();
                        }
                        if (!(tokens[i + 2].getType() == Global.IF || tokens[i + 2].getType() == Global.LEFT_SECOND))
                        {
                            Error error = new Error(tokens[i + 2], "Expected {");
                            error.Execute();
                        }
                        if (i < tokens.Count - 2 && tokens[i + 2].getType() == Global.IF)
                        {
                            addCondition = true;
                            i += 2;
                            addCode = true;
                        }
                        else
                        {
                            i++;
                            addCode = true;
                        }
                    }
                }
                for (var it = 0; it < code.Count; it++)
                {
                    Expressions condition;
                    if (it < conditions.Count)
                        condition = new Expressions(conditions[it]);
                    else
                        condition = new Expressions(conditions[0]);
                    Interpreter work = new Interpreter(code[it], 1);
                    if (it == conditions.Count || condition.evaluate() != "0")
                    {
                        Global.cleanUp();
                        var temp = work.Evaluate();
                        Global.cleanUp();
                        if (temp.getType() == "3") return temp;
                        if (temp.getType() == "2") return temp;
                        if (temp.getType() == "1") return temp;
                        break;
                    }
                    Global.cleanUp();
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
                list.Add(new Token(Global.LEFT_FIRST, "", "", 0, 0, 0));
                for (; i < tokens.Count && tokens[i].getType() != Global.SEMI_COLON; i++)
                {
                    if (allowedInWhile(tokens[i]))
                    {
                        if (tokens[i].getType() == Global.FUNCTION)
                            list.Add(functionCleanUp(ref i));
                        else
                            list.Add(tokens[i]);
                    }
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
        return new Token("", "0","", 0, 0, 0);
    }
}
