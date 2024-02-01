using System.Diagnostics.Metrics;
using System.Globalization;
using System.Text;

namespace bangla;
public  class NODE
{
    public string type ="";
    public List<string> data = new  ();
    public NODE(string type, int size)
    {
        this.type = type;
        for(var i=0; i<size; i++) { this.data.Add("0"); }
    }
}
public class Functions(List<Token>arguments, List<Token>code)
{
    private readonly List<Token> arguments = arguments;
    private readonly List<Token> Code = code;
    public List<Token> getCode ()=> Code;
    public List<Token> getArguments() => arguments;
}
public class Global
{
    //<--Memory-->
    public static Stack<SortedDictionary<string, NODE>> memory = new();

    //Functions
    public static SortedDictionary<string, Functions> functions = new();

    //<--Code-->
    private static string code = "";
    public static void setCode(string cod) => code = cod;
    public static string getCode() => code;
    //<--Global State-->
    public static Stack<Token> post_increment = new();
    public static Stack<Token> post_decrement = new();
    private const string CHARACTERS = "অআইঈউঊঋএঐওঔকখগঘঙচছজঝঞটঠডঢণতথদধনপফবভমযরলশষসহয়ড়ঢ়ৎংঃঁ্ােিীুূোৃৈৌঃ‌";
    private const string NUMBERS = "1234567890";
    //<--Global Mechanism-->
    public static bool isVariable(string name)
    {
        var i = 0;
        while (i < name.Length && CHARACTERS.IndexOf(name[i]) != -1) i++;
        while (i < name.Length && NUMBERS.IndexOf(name[i]) != -1) i++;
        return i >= name.Length;
    }
    public static void cleanUp()
    {
        while (post_decrement.Count > 0) post_decrement.Peek().setValue((decimal.Parse(post_decrement.Pop().getValue()) - 1).ToString());
        while (post_increment.Count > 0) post_increment.Peek().setValue((decimal.Parse(post_increment.Pop().getValue()) + 1).ToString());
    }
    //<--keywords-->

    //Operators
    public const string PLUS = "plus";
    public const string MINUS = "minus";
    public const string MULTIPLY = "multiply";
    public const string DIVIDE = "divide";
    public const string MOD = "mod";
    public const string POW = "pow";
    public const string INCREMENT = "increment";
    public const string PRE_INCREMENT = "pre_increment";
    public const string POST_INCREMENT = "post_increment";
    public const string DECREMENT = "decrement";
    public const string PRE_DECREMENT = "pre_decrement";
    public const string POST_DECREMENT = "post_decrement";
    public const string ASSIGNMENT = "assignment";
    //logicalOperators
    public const string EQUAL = "equal";
    public const string LESS_THEN = "less_then";
    public const string GREATER_THEN = "greater_then";
    public const string LESS_THEN_EQUAL = "less_then_equal";
    public const string GREATER_THEN_EQUAL = "greater_then_equal";
    public const string AND = "and";
    public const string OR = "or";
    public static bool isOperators(string str) => str == PLUS || str == MINUS || str == MULTIPLY || str == DIVIDE || str == MOD || str == INCREMENT || str == PRE_INCREMENT
            || str == POST_INCREMENT || str == DECREMENT || str == PRE_DECREMENT || str == POST_DECREMENT || str == ASSIGNMENT || str == EQUAL
            || str == LESS_THEN || str == GREATER_THEN || str == LESS_THEN_EQUAL || str == GREATER_THEN_EQUAL || str == AND || str == OR;
    //Brackets
    public const string LEFT_FIRST = "left(";
    public const string RIGHT_FIRST = "right)";
    public const string LEFT_SECOND = "left{";
    public const string RIGHT_SECOND = "right}";
    public const string LEFT_THIRD = "left[";
    public const string RIGHT_THIRD = "right]";
    public static bool isBrackets(string str) => str == LEFT_FIRST || str == RIGHT_FIRST || str == LEFT_THIRD || str == RIGHT_THIRD;

    //Symbols
    public const string COMMA = "comma";
    public const string SEMI_COLON = "semicolon";
    //DataTypes
    public const string INTEGER_VARIABLE = "integer_variable";
    public const string REAL_VARIABLE = "real_variable";
    public const string STRING_VARIABLE = "string_variable";
    public const string INTEGER_ARRAY = "integer_array";
    public const string REAL_ARRAY = "real_array";
    public const string STRING_ARRAY = "string_array";
    public static bool isDataType(string str) => str == INTEGER_VARIABLE || str == REAL_VARIABLE ||str== STRING_VARIABLE||str==INTEGER_ARRAY||str==REAL_ARRAY||str==STRING_ARRAY;
    //Keywords
    public const string WRITE = "write";
    public const string READ = "read";
    public const string WHILE = "while";
    public const string IF = "if";
    public const string ELSE = "else";
    public const string INTEGER_INITIALIZER = "integer_initializer";
    public const string REAL_INITIALIZER = "real_initializer";
    public const string STRING_INITIALIZER = "string_initializer";
    public const string RETURN = "return";
    public const string BREAK = "break";
    public const string CONTINUE = "continue";
    public static bool isKeywords(string str) => str == WRITE || str == READ || str == WHILE || str == IF || str == ELSE || str == INTEGER_INITIALIZER
        || str == REAL_INITIALIZER || str == STRING_INITIALIZER || str == RETURN;

    //Internals
    public const string INTEGER = "integer";
    public const string REAL = "real";
    public const string STRING = "string";
    public const string VARIABLE = "variable";
    public const string FUNCTION = "function";
    public const string FUNCTION_PARAMETER = "function_parameter";
    public const string TEMPORARY = "temporary";
}


internal class Program
{
    static void Main(string[] args)
    {
        Global.memory.Push(new SortedDictionary<string, NODE>());
        //if (args.Length == 0)
        //{
        //    Console.WriteLine("ENTER FILE PATH");
        //    return;
        //}
        Console.OutputEncoding = Encoding.Unicode;
        Console.InputEncoding = Encoding.Unicode;
        Optimizer optimizer = new(File.ReadAllText("C:\\Users\\sayem\\Desktop\\log.txt"));
        Global.setCode(Converter.IN(optimizer.getCode()));
        var code = Global.getCode();
        Lexer lexer = new Lexer(code);
        foreach (Token t in lexer.getTokens()) Console.WriteLine(t.getType());
        FunctionGenerator generator = new FunctionGenerator(lexer.getTokens());
        generator.Generate();
        var startPoint = new Token(Global.VARIABLE, "", "প্রধান", 0, 0, 0);
        if(startPoint.getValue()=="0"){
            Console.WriteLine("Your Program Executed properly");
        }
        Global.memory.Pop();
    }
}   