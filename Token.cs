using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace bangla;
public unsafe class Token
{
    private string type;
    private readonly string value;
    private readonly string name;
    private readonly int start;
    private readonly int end;
    private readonly int line;
    private int index = 0;
    private List<string>*[] arg = [];
    public Token(string type, string value, string name, int start, int end, int line)
    {
        this.type = type;
        this.value = value;
        this.name = name;
        this.start = start;
        this.end = end;
        this.line = line;
    }
    //Set
    public void setArg(List<Token>data)
    {
        arg = new List<string>*[data.Count];
    }
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