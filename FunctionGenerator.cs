using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace bangla;
internal class FunctionGenerator(List<Token> tokens)
{
    private List<Token> tokens = tokens;
    public void Generate()
    {
        for (var i = 0; i < this.tokens.Count; i++)
        {
            if (tokens[i].getType() == Global.INTEGER_INITIALIZER)
            {
                var j = ++i;
                if (tokens[j].getType() != Global.VARIABLE)
                {
                    Error error = new Error(tokens[j], "Illegal Character"); error.Execute();
                }
                List<Token> list1 = new List<Token>();
                List<Token> list2 = new List<Token>();
                i++;
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
                    if (balance < 0)
                    {
                        Error error = new Error(tokens[Math.Min(i, tokens.Count - 1)], "Expected }");
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
                    if (balance < 0)
                    {
                        Error error = new Error(tokens[Math.Min(i, tokens.Count - 1)], "Expected }");
                        error.Execute();
                    }
                    list1.RemoveAt(list1.Count - 1);
                    list2.RemoveAt(list2.Count - 1);
                    Global.functions.Add(tokens[j].getName(), new Functions(list1,list2));
                }
                else
                {
                    Error error = new Error(tokens[i + 1], "Expected (");
                    error.Execute();
                }
            }
        }
    }
}
