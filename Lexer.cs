using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace bangla;
internal class Lexer(string code)
{
    private readonly string code = code;
    public List<Token> getTokens()
    {
        var line = 1;
        List<Token> tokens = new List<Token>();
        for (var i = 0; i < this.code.Length; i++)
        {
            var temp = "";
            var j = i;
            if (code[i] == '"')
            {
                temp += code[i++];
                while (i < this.code.Length && code[i] != '"') temp += code[i++];
                temp += code[i++];
            }
            else
                while (i < this.code.Length && code[i] != ' ') temp += code[i++];
            if (temp == "") continue;
            if (temp == "\n") line++;
            //Operators
            else if (temp == "+") tokens.Add(new Token(Global.PLUS, "", "", j, i, line));
            else if (temp == "-") tokens.Add(new Token(Global.MINUS, "", "", j, i, line));
            else if (temp == "*") tokens.Add(new Token(Global.MULTIPLY, "", "", j, i, line));
            else if (temp == "/") tokens.Add(new Token(Global.DIVIDE, "", "", j, i, line));
            else if (temp == "%") tokens.Add(new Token(Global.MOD, "", "", j, i, line));
            else if (temp == "^") tokens.Add(new Token(Global.POW, "", "", j, i, line));
            else if (temp == "++") tokens.Add(new Token(Global.INCREMENT, "", "", j, i, line));
            else if (temp == "--") tokens.Add(new Token(Global.DECREMENT, "", "", j, i, line));
            else if (temp == "=") tokens.Add(new Token(Global.ASSIGNMENT, "", "", j, i, line));
            //logicalOperators
            else if (temp == "==") tokens.Add(new Token(Global.EQUAL, "", "", j, i, line));
            else if (temp == "<") tokens.Add(new Token(Global.LESS_THEN, "", "", j, i, line));
            else if (temp == ">") tokens.Add(new Token(Global.GREATER_THEN, "", "", j, i, line));
            else if (temp == "<=") tokens.Add(new Token(Global.LESS_THEN_EQUAL, "", "", j, i, line));
            else if (temp == ">=") tokens.Add(new Token(Global.GREATER_THEN_EQUAL, "", "", j, i, line));
            else if (temp == "এবং") tokens.Add(new Token(Global.AND, "", "", j, i, line));
            else if (temp == "অথবা") tokens.Add(new Token(Global.OR, "", "", j, i, line));
            //brackets
            else if (temp == "(") tokens.Add(new Token(Global.LEFT_FIRST, "", "", j, i, line));
            else if (temp == ")") tokens.Add(new Token(Global.RIGHT_FIRST, "", "", j, i, line));
            else if (temp == "{") tokens.Add(new Token(Global.LEFT_SECOND, "", "", j, i, line));
            else if (temp == "}") tokens.Add(new Token(Global.RIGHT_SECOND, "", "", j, i, line));
            else if (temp == "[") tokens.Add(new Token(Global.LEFT_THIRD, "", "", j, i, line));
            else if (temp == "]") tokens.Add(new Token(Global.RIGHT_THIRD, "", "", j, i, line));
            //Symbols
            else if (temp == ",") tokens.Add(new Token(Global.COMMA, "", "", j, i, line));
            else if (temp == ";") tokens.Add(new Token(Global.SEMI_COLON, "", "", j, i, line));
            //Keywords
            else if (temp == "দেখাও") tokens.Add(new Token(Global.WRITE, "", "", j, i, line));
            else if (temp == "পড়") tokens.Add(new Token(Global.READ, "", "", j, i, line));
            else if (temp == "যতক্ষণ") tokens.Add(new Token(Global.WHILE, "", "", j, i, line));
            else if (temp == "যদি") tokens.Add(new Token(Global.IF, "", "", j, i, line));
            else if (temp == "পূর্ণ") tokens.Add(new Token(Global.INTEGER_INITIALIZER, "", "", j, i, line));
            else if (temp == "বাস্তব") tokens.Add(new Token(Global.REAL_INITIALIZER, "", "", j, i, line));
            else if (temp == "বাক্য") tokens.Add(new Token(Global.STRING_INITIALIZER, "", "", j, i, line));
            else if (temp == "ফেরত") tokens.Add(new Token(Global.RETURN, "", "", j, i, line));
            //else if(temp=="")
            //DataTypes
            else if (temp.Length > 1 && temp[0] == '\"' && temp[temp.Length - 1] == '\"') tokens.Add(new Token(Global.STRING, temp.Trim('\"'), "", j, i, line));
            else if (decimal.TryParse(temp, out var _)) tokens.Add(new Token(Global.REAL, temp, "", j, i, line));
            else if (Global.isVariable(temp)) tokens.Add(new Token(Global.VARIABLE, "", temp, j, i, line));
            //TODO
        }
        return tokens;
    }
}