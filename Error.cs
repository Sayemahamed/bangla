using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace bangla;
internal class Error(Token token, string error)
{
    private int line = 1;
    private readonly string code = Global.getCode();
    public void Execute()
    {
        for (var i = 0; i < code.Length; i++)
        {
            if (line > token.getLine())break;
            if (code[i] == '\n') line++;
            if (line == token.getLine()) Console.Write(code[i]);
            if (i >= token.getStart() - 1 && i < token.getEnd()) Console.ForegroundColor = ConsoleColor.Red;
            if (i >= token.getEnd()) Console.ResetColor();
        }
        Console.WriteLine();
        Console.WriteLine(error);
        Environment.Exit(-1);
    }
}
