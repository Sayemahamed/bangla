using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace bangla;
internal class Optimizer
{
    private readonly string optimizedCode;
    public Optimizer(string code)
    {
        var temp = "";
        for (var i = 0; i < code.Length; i++)
        {
            if (code[i] == '\n' || code[i] == ';' || code[i] == '*' || code[i] == '/' || code[i] == '%' || code[i] == '^' || code[i] == '(' || code[i] == ')'
                || code[i] == '{' || code[i] == '}' || code[i] == '[' || code[i] == ']' || code[i] == ','
                ) temp += " " + code[i] + " ";
            else if (code[i] == '+')
            {
                if (i<code.Length&&code[i + 1] == '+')
                {
                    temp += " ++ ";
                    i++;
                }
                else
                    temp += " + ";
            }
            else if (code[i] == '-')
            {
                if (i < code.Length-1 && code[i + 1] == '-')
                {
                    temp += " -- ";
                    i++;
                }
                else
                    temp += " - ";
            }
            else if (code[i] == '=')
            {
                if (i < code.Length-1 && code[i + 1] == '=')
                {
                    temp += " == ";
                    i++;
                }
                else
                    temp += " = ";
            }
            else if (code[i] == '<')
            {
                if (i < code.Length - 1 && code[i + 1] == '=')
                {
                    temp += " <= ";
                    i++;
                }
                else
                    temp += " < ";
            }
            else if (code[i] == '>')
            {
                if (i < code.Length - 1 && code[i + 1] == '=')
                {
                    temp += " >= ";
                    i++;
                }
                else
                    temp += " > ";
            }
            else if (code[i] == '\"')
            {
                temp += " \"";
                i++;
                while (i < code.Length && code[i] != '\"') temp += code[i++];
                temp += "\" ";
            }
            else temp += code[i];
        }
        optimizedCode = temp.Trim();
    }
    public string getCode()
    {
        return optimizedCode;
    }
}
