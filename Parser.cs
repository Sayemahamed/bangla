namespace bangla;
internal class Parser(List<Token> code)
{
    private readonly List<Token> code = code;
    private readonly Stack<Token> stack = new();
    private int getPricision(string str)
    {
        if (str == Global.ASSIGNMENT) return -2;
        if (str == Global.EQUAL || str == Global.LESS_THEN || str == Global.GREATER_THEN || str == Global.LESS_THEN_EQUAL || str == Global.GREATER_THEN_EQUAL || str == Global.AND || str == Global.OR) return 0;
        if (str == Global.PLUS || str == Global.MINUS) return 1;
        if (str == Global.MULTIPLY || str == Global.DIVIDE) return 2;
        if (str == Global.MOD || str == Global.POW) return 3;
        if (str == Global.POST_INCREMENT || str == Global.PRE_INCREMENT || str == Global.POST_DECREMENT || str == Global.PRE_DECREMENT) return 4;
        return -1;
    }
    public List<Token> ToPostfix()
    {
        List<Token> postFix = new();
        for (var i = 0; i < this.code.Count; i++)
        {
            if (code[i].getType() == Global.LEFT_THIRD)
            {
                if (!(code[i + 1].getType() == Global.REAL||Global.isDataType(code[i + 1].getType())))
                {
                    Error error = new Error(code[i + 1], "Illegal Character"); error.Execute();
                }
                if (code[i + 2].getType() != Global.RIGHT_THIRD)
                {
                    Error error = new Error(code[i + 2], "Illegal Character"); error.Execute();
                }
                postFix[postFix.Count - 1].setIndex(int.Parse(code[i + 1].getValue()));
                i += 2;
                continue;
            }
            if (code[i].getType() == Global.MINUS && (code[i - 1].getType() == Global.LEFT_FIRST || Global.isOperators(code[i - 1].getType())))
            {
                i++;
                var count = 1;
                while (i < this.code.Count - 1 && code[i].getType() == Global.MINUS) { count++; i++; }
                if (count % 2 == 1)
                    postFix.Add(new Token(Global.REAL, '-' + code[i].getValue(), "", code[i].getStart(), code[i].getEnd(), code[i].getLine()));
                else postFix.Add(code[i]);
                //Console.WriteLine(count);
                //Console.WriteLine(count);
                continue;
            }
            if (code[i].getType() == Global.INCREMENT)
            {
                if ((i > 0 && Global.isDataType(code[i - 1].getType())) || (i > 4 && Global.isDataType(code[i - 4].getType())))
                    code[i].setType(Global.POST_INCREMENT);
                else if (i < this.code.Count - 1 && Global.isDataType(code[i + 1].getType()))
                    code[i].setType(Global.PRE_INCREMENT);
                else
                {
                    Error error = new Error(code[i], "Can only operate on Variable");
                    error.Execute();
                }
            }
            if (code[i].getType() == Global.DECREMENT)
            {
                if ((i > 0 && Global.isDataType(code[i - 1].getType())) || (i > 4 && Global.isDataType(code[i - 4].getType())))
                    code[i].setType(Global.POST_DECREMENT);
                else if (i < this.code.Count - 1 && Global.isDataType(code[i + 1].getType()))
                    code[i].setType(Global.PRE_DECREMENT);
                else
                {
                    Error error = new Error(code[i], "Can only operate on Variable");
                    error.Execute();
                }
            }
            if (code[i].getType() == Global.LEFT_FIRST) stack.Push(code[i]);
            else if (getPricision(code[i].getType()) != -1)
            {
                while (stack.Count > 0 && getPricision(code[i].getType()) <= getPricision(stack.Peek().getType()) && stack.Peek().getType() != Global.LEFT_FIRST)
                {
                    postFix.Add(stack.Pop());
                }
                stack.Push(code[i]);
            }
            else if (code[i].getType() == Global.RIGHT_FIRST) { while (stack.Count > 0 && stack.Peek().getType() != Global.LEFT_FIRST) postFix.Add(stack.Pop()); stack.Pop(); }
            else postFix.Add(code[i]);
        }
        return postFix;
    }
}