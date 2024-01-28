using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace bangla;
public class Token(string type, string value, string name, int start, int end, int line)
{
    private string type = type;
    private readonly string value = value;
    private readonly string name = name;
    private readonly int start = start;
    private readonly int end = end;
    private readonly int line = line;
    private int index = 0;
    private List<Token> data;

    //Set
    public void setArg(List<Token> data) => this.data = data;
    public void setType(string type)
    {
        if (Global.memory.Peek().ContainsKey(this.name))
        {
            Global.memory.Peek()[this.name].type = type;
        }
        else if (this.type != Global.VARIABLE)
            this.type = type;
    }
    public void setValue(string value)
    {
        if (getType() == Global.TEMPORARY)
        {
            Error error = new Error(this, "Illegal Assignment Expression");
            error.Execute();
        }
        if (!Global.isDataType(getType()))
        {
            Error error = new Error(this, "Initialize the variable first");
            error.Execute();
        }
        var data = value;
        Error erro = new Error(this, "Illegal Assignment");
        if (Global.memory.Peek()[this.name].type == Global.REAL_VARIABLE)
        {
            //Console.WriteLine("daddy was here");
            if (!decimal.TryParse(value, out var A)) erro.Execute();
            else data = A.ToString();
        }
        else if (Global.memory.Peek()[this.name].type == Global.INTEGER_VARIABLE)
        {
            //Console.WriteLine("daddy was here");
            if (!decimal.TryParse(value, out var A)) erro.Execute();
            else data = Math.Floor(A).ToString();
        }
        Global.memory.Peek()[this.name].data[index] = data;
    }
    public void setIndex(int index)
    {
        if (getType() == Global.INTEGER_ARRAY || getType() == Global.REAL_ARRAY || getType() == Global.STRING_ARRAY)
            this.index = index;
        else
        {
            Error error = new Error(this, "Illegal Access");
            error.Execute();
        }
        if (Global.memory.Peek()[this.name].data.Count <= index || index < 0)
        {
            Error error = new Error(this, "Out of array Boundary");
            error.Execute();
        }
    }
    //Get
    public string getType()
    {
        if (Global.functions.ContainsKey(this.name)) return Global.FUNCTION;
        if (type == Global.VARIABLE && Global.memory.Peek().ContainsKey(this.name)) return Global.memory.Peek()[this.name].type;
        return type;
    }
    public string getValue()
    {
        Error error = new Error(this, "Initialize the variable first");
        if (this.type == Global.VARIABLE && Global.functions.ContainsKey(getName()))
        {
            if (getName() != "প্রধান")
                Global.memory.Push(new SortedDictionary<string, NODE>());
            var arguments = Global.functions[getName()].getArguments();
            if (arguments.Count != 0)
            {
                Error error1;
                var j = 0;
                for (var i = 0; i < arguments.Count; i++)
                {
                    if (arguments[i].getType() == Global.INTEGER_INITIALIZER)
                    {
                        if (i >= arguments.Count || arguments[i + 1].getType() != Global.VARIABLE)
                        {
                            error1 = new Error(arguments[i], "Invalid Argument");
                            error1.Execute();
                        }
                        else
                        {
                            Global.memory.Peek().Add(arguments[i + 1].getName(), new NODE(Global.INTEGER_VARIABLE, 1));
                        }
                        if (j == data.Count)
                        {
                            error1 = new Error(data[j - 1], "Insufficient Arguments");
                            error1.Execute();
                        }
                        else
                        {
                            arguments[i + 1].setValue(data[j].getValue());
                            j++;
                        }
                        if (i < arguments.Count - 2)
                        {
                            if (arguments[i + 2].getType() == Global.COMMA) i += 2;
                            else
                            {
                                error1 = new Error(arguments[i + 2], "Insufficient Arguments");
                                error1.Execute();
                            }
                        }
                    }
                    else if (arguments[i].getType() == Global.REAL_INITIALIZER)
                    {
                        if (i >= arguments.Count || arguments[i + 1].getType() != Global.VARIABLE)
                        {
                            error1 = new Error(arguments[i], "Invalid Argument");
                            error1.Execute();
                        }
                        else
                        {
                            Global.memory.Peek().Add(arguments[i + 1].getName(), new NODE(Global.REAL_VARIABLE, 1));
                        }
                        if (j == data.Count)
                        {
                            error1 = new Error(data[j - 1], "Insufficient Arguments");
                            error1.Execute();
                        }
                        else
                        {
                            arguments[i + 1].setValue(data[j].getValue());
                            j++;
                        }
                        if (i < arguments.Count - 2)
                        {
                            if (arguments[i + 2].getType() == Global.COMMA) i += 2;
                            else
                            {
                                error1 = new Error(arguments[i + 2], "Insufficient Arguments");
                                error1.Execute();
                            }
                        }
                    }
                    else if (arguments[i].getType() == Global.STRING_INITIALIZER)
                    {
                        if (i >= arguments.Count || arguments[i + 1].getType() != Global.VARIABLE)
                        {
                            error1 = new Error(arguments[i], "Invalid Argument");
                            error1.Execute();
                        }
                        else
                        {
                            Global.memory.Peek().Add(arguments[i + 1].getName(), new NODE(Global.STRING_VARIABLE, 1));
                        }
                        if (j == data.Count)
                        {
                            error1 = new Error(data[j - 1], "Insufficient Arguments");
                            error1.Execute();
                        }
                        else
                        {
                            arguments[i + 1].setValue(data[j].getValue());
                            j++;
                        }
                        if (i < arguments.Count - 2)
                        {
                            if (arguments[i + 2].getType() == Global.COMMA) i += 2;
                            else
                            {
                                error1 = new Error(arguments[i + 2], "Insufficient Arguments");
                                error1.Execute();
                            }
                        }
                    }
                    else
                    {
                        error1 = new Error(arguments[i], "Invalid Arguments");
                        error1.Execute();
                    }
                }
                if (j < data.Count)
                {
                    error1 = new Error(data[j], "Extra Arguments");
                    error1.Execute();
                }
            }
            if (arguments.Count == 0 && data.Count != 0)
            {
                Error error1 = new Error(data[0], "Invalid Argument");
                error1.Execute();
            }
            Interpreter interpreter = new Interpreter(Global.functions[getName()].getCode(), 3);
            var a = interpreter.Evaluate().getValue();
            if (getName() != "প্রধান")
                Global.memory.Pop();
            return a;
        }
        if (this.type == Global.VARIABLE)
            if (Global.memory.Peek().ContainsKey(this.name)) return Global.memory.Peek()[this.name].data[index];
            else error.Execute();
        return value;
    }
    public string getName() => name;
    public int getStart() => start;
    public int getEnd() => end;
    public int getLine() => line;
}