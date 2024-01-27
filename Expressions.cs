using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace bangla;
internal class Expressions
{
    private readonly List<Token> tokens;
    private readonly Stack<Token> stack = new();
    public Expressions(List<Token> tokens)
    {
        Parser parser = new Parser(tokens);
        this.tokens = parser.ToPostfix();
    }
    public string evaluate()
    {
        foreach (Token token in tokens)
        {
            //Operators
            if (token.getType() == Global.PLUS)
            {
                Token a = stack.Pop();
                if (stack.Count == 0)
                {
                    Error error = new Error(a, "Incorrect Expression");
                    error.Execute();
                }
                Token b = stack.Pop();
                if (a.getType() == Global.STRING || a.getType() == Global.STRING_VARIABLE || b.getType() == Global.STRING || b.getType() == Global.STRING_VARIABLE)
                {
                    stack.Push(new Token(
                    Global.STRING,
                    (b.getValue()) + a.getValue(), "", b.getStart(), a.getEnd(), b.getLine()
                    ));
                }
                else
                    stack.Push(new Token(
                    Global.TEMPORARY,
                    (decimal.Parse(b.getValue()) + decimal.Parse(a.getValue())).ToString(), "", b.getStart(), a.getEnd(), b.getLine()
                    )
                );
            }
            else if (token.getType() == Global.MINUS)
            {
                Token a = stack.Pop();
                if (stack.Count == 0)
                {
                    Error erro = new Error(a, "Incorrect Expression");
                    erro.Execute();
                }
                Token b = stack.Pop();
                Error error = new Error(new Token(Global.VARIABLE, "", "", b.getStart(), a.getEnd(), b.getLine()), "Illegal Operation");
                if (!(decimal.TryParse(a.getValue(), out var A))) error.Execute();
                if (!(decimal.TryParse(b.getValue(), out var B))) error.Execute();
                stack.Push(new Token(
                Global.TEMPORARY,
                (B - A).ToString(), "", b.getStart(), a.getEnd(), b.getLine()
                )
            );
            }
            else if (token.getType() == Global.MULTIPLY)
            {
                Token a = stack.Pop();
                if (stack.Count == 0)
                {
                    Error erro = new Error(a, "Incorrect Expression");
                    erro.Execute();
                }
                Token b = stack.Pop();
                Error error = new Error(new Token(Global.VARIABLE, "", "", b.getStart(), a.getEnd(), b.getLine()), "Illegal Operation");
                if (!(decimal.TryParse(a.getValue(), out var A))) error.Execute();
                if (!(decimal.TryParse(b.getValue(), out var B))) error.Execute();
                stack.Push(new Token(
                Global.TEMPORARY,
                (B * A).ToString(), "", b.getStart(), a.getEnd(), b.getLine()
                )
            );
            }
            else if (token.getType() == Global.DIVIDE)
            {
                Token a = stack.Pop();
                if (a.getValue() == "0")
                {
                    Error erro = new Error(a, "The Expression is giving DIVIDE_BY ZERO error ");
                    erro.Execute();
                }
                if (stack.Count == 0)
                {
                    Error erro = new Error(a, "Incorrect Expression");
                    erro.Execute();
                }
                Token b = stack.Pop();
                Error error = new Error(new Token(Global.VARIABLE, "", "", b.getStart(), a.getEnd(), b.getLine()), "Illegal Operation");
                if (!(decimal.TryParse(a.getValue(), out var A))) error.Execute();
                if (!(decimal.TryParse(b.getValue(), out var B))) error.Execute();
                stack.Push(new Token(
                Global.TEMPORARY,
                (B / A).ToString(), "", b.getStart(), a.getEnd(), b.getLine()
                )
            );
            }
            else if (token.getType() == Global.MOD)
            {
                Token a = stack.Pop();
                if (a.getValue() == "0")
                {
                    Error erro = new Error(a, "The Expression is giving DIVIDE_BY ZERO error ");
                    erro.Execute();
                }
                if (stack.Count == 0)
                {
                    Error erro = new Error(a, "Incorrect Expression");
                    erro.Execute();
                }
                Token b = stack.Pop();
                Error error = new Error(new Token(Global.VARIABLE, "", "", b.getStart(), a.getEnd(), b.getLine()), "Illegal Operation");
                if (!(decimal.TryParse(a.getValue(), out var A))) error.Execute();
                if (!(decimal.TryParse(b.getValue(), out var B))) error.Execute();
                stack.Push(new Token(
                Global.TEMPORARY,
                (B % A).ToString(), "", b.getStart(), a.getEnd(), b.getLine()
                )
            );
            }
            else if (token.getType() == Global.POW)
            {
                Token a = stack.Pop();
                if (stack.Count == 0)
                {
                    Error erro = new Error(a, "Incorrect Expression");
                    erro.Execute();
                }
                Token b = stack.Pop();
                Error error = new Error(new Token(Global.VARIABLE, "", "", b.getStart(), a.getEnd(), b.getLine()), "Illegal Operation");
                if (!(double.TryParse(a.getValue(), out var A))) error.Execute();
                if (!(double.TryParse(b.getValue(), out var B))) error.Execute();
                stack.Push(new Token(
                Global.TEMPORARY,
                (Math.Pow(B, A)).ToString(), "", b.getStart(), a.getEnd(), b.getLine()
                )
            );
            }
            else if (token.getType() == Global.INCREMENT || token.getType() == Global.DECREMENT)
            {
                Error error = new Error(token, "can only operate on variable");
                error.Execute();
            }
            else if (token.getType() == Global.PRE_INCREMENT)
            {
                Error error = new Error(new Token(Global.VARIABLE, "", "", stack.Peek().getStart(), token.getEnd(), stack.Peek().getLine()), "Illegal Operation");
                if (!(double.TryParse(stack.Peek().getValue(), out var A))) error.Execute();
                stack.Peek().setValue((A + 1).ToString());
            }
            else if (token.getType() == Global.POST_INCREMENT)
            {
                Global.post_increment.Push(stack.Peek());
            }
            else if (token.getType() == Global.PRE_DECREMENT)
            {
                Error error = new Error(new Token(Global.VARIABLE, "", "", stack.Peek().getStart(), token.getEnd(), stack.Peek().getLine()), "Illegal Operation");
                if (!(double.TryParse(stack.Peek().getValue(), out var A))) error.Execute();
                stack.Peek().setValue((A - 1).ToString());
            }
            else if (token.getType() == Global.POST_DECREMENT)
            {
                Global.post_decrement.Push(stack.Peek());
            }
            else if (token.getType() == Global.ASSIGNMENT)
            {
                Token a = stack.Pop();
                stack.Peek().setValue(a.getValue());
            }
            //LogicalOperators
            else if (token.getType() == Global.EQUAL)
            {
                Token a = stack.Pop();
                Token b = stack.Pop();
                Error error = new Error(new Token(Global.VARIABLE, "", "", b.getStart(), a.getEnd(), b.getLine()), "Illegal Operation");
                if (!(double.TryParse(a.getValue(), out var A))) error.Execute();
                if (!(double.TryParse(b.getValue(), out var B))) error.Execute();
                if (A == B)
                    stack.Push(new Token(
                    Global.TEMPORARY,
                    "1", "", b.getStart(), a.getEnd(), b.getLine()
                    )
                );
                else
                    stack.Push(new Token(
                    Global.TEMPORARY,
                    "0", "", b.getStart(), a.getEnd(), b.getLine()
                    )
                );
            }
            else if (token.getType() == Global.LESS_THEN)
            {
                Token a = stack.Pop();
                Token b = stack.Pop();
                Error error = new Error(new Token(Global.VARIABLE, "", "", b.getStart(), a.getEnd(), b.getLine()), "Illegal Operation");
                if (!(double.TryParse(a.getValue(), out var A))) error.Execute();
                if (!(double.TryParse(b.getValue(), out var B))) error.Execute();
                if (A > B)
                    stack.Push(new Token(
                    Global.TEMPORARY,
                    "1", "", b.getStart(), a.getEnd(), b.getLine()
                    )
                );
                else
                    stack.Push(new Token(
                    Global.TEMPORARY,
                    "0", "", b.getStart(), a.getEnd(), b.getLine()
                    )
                );
            }
            else if (token.getType() == Global.GREATER_THEN)
            {
                Token a = stack.Pop();
                Token b = stack.Pop();
                Error error = new Error(new Token(Global.VARIABLE, "", "", b.getStart(), a.getEnd(), b.getLine()), "Illegal Operation");
                if (!(double.TryParse(a.getValue(), out var A))) error.Execute();
                if (!(double.TryParse(b.getValue(), out var B))) error.Execute();
                if (A < B)
                    stack.Push(new Token(
                    Global.TEMPORARY,
                    "1", "", b.getStart(), a.getEnd(), b.getLine()
                    )
                );
                else
                    stack.Push(new Token(
                    Global.TEMPORARY,
                    "0", "", b.getStart(), a.getEnd(), b.getLine()
                    )
                );
            }
            else if (token.getType() == Global.LESS_THEN_EQUAL)
            {
                Token a = stack.Pop();
                Token b = stack.Pop();
                Error error = new Error(new Token(Global.VARIABLE, "", "", b.getStart(), a.getEnd(), b.getLine()), "Illegal Operation");
                if (!(double.TryParse(a.getValue(), out var A))) error.Execute();
                if (!(double.TryParse(b.getValue(), out var B))) error.Execute();
                if (A >= B)
                    stack.Push(new Token(
                    Global.TEMPORARY,
                    "1", "", b.getStart(), a.getEnd(), b.getLine()
                    )
                );
                else
                    stack.Push(new Token(
                    Global.TEMPORARY,
                    "0", "", b.getStart(), a.getEnd(), b.getLine()
                    )
                );
            }
            else if (token.getType() == Global.GREATER_THEN_EQUAL)
            {
                Token a = stack.Pop();
                Token b = stack.Pop();
                Error error = new Error(new Token(Global.VARIABLE, "", "", b.getStart(), a.getEnd(), b.getLine()), "Illegal Operation");
                if (!(double.TryParse(a.getValue(), out var A))) error.Execute();
                if (!(double.TryParse(b.getValue(), out var B))) error.Execute();
                if (A <= B)
                    stack.Push(new Token(
                    Global.TEMPORARY,
                    "1", "", b.getStart(), a.getEnd(), b.getLine()
                    )
                );
                else
                    stack.Push(new Token(
                    Global.TEMPORARY,
                    "0", "", b.getStart(), a.getEnd(), b.getLine()
                    )
                );
            }
            else if (token.getType() == Global.AND)
            {
                Token a = stack.Pop();
                Token b = stack.Pop();
                if (decimal.Parse(a.getValue()) != 0 && decimal.Parse(b.getValue()) != 0)
                    stack.Push(new Token(
                    Global.TEMPORARY,
                    "1", "", b.getStart(), a.getEnd(), b.getLine()
                    )
                );
                else
                    stack.Push(new Token(
                    Global.TEMPORARY,
                    "0", "", b.getStart(), a.getEnd(), b.getLine()
                    )
                );
            }
            else if (token.getType() == Global.OR)
            {
                Token a = stack.Pop();
                Token b = stack.Pop();
                if (decimal.Parse(a.getValue()) != 0 || decimal.Parse(b.getValue()) != 0)
                    stack.Push(new Token(
                    Global.TEMPORARY,
                    "1", "", b.getStart(), a.getEnd(), b.getLine()
                    )
                );
                else
                    stack.Push(new Token(
                    Global.TEMPORARY,
                    "0", "", b.getStart(), a.getEnd(), b.getLine()
                    )
                );
            }
            else if (token.getType() == Global.ASSIGNMENT)
            {
                Token a = stack.Pop();
                stack.Peek().setValue(a.getValue());
            }
            else stack.Push(token);
        }
        if (stack.Count > 1)
        {
            var a = stack.Pop();
            var b = stack.Pop();
            Error error = new Error(new Token("", "", "", b.getStart(), a.getEnd(), b.getLine()), "Illegal Expression");
            error.Execute();
        }
        var v = stack.Peek().getValue().ToString();
        return v;
    }
}