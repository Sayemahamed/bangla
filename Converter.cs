using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace bangla;
internal class Converter
{
    public static string IN(string str)
    {
        var temp = "";
        for (var i = 0; i < str.Length; i++)
        {
            while (i < str.Length - 1 && str[i] == ' ' && str[i + 1] == str[i]) i++;
            if (str[i] == '১') temp += '1';
            else if (str[i] == '২') temp += '2';
            else if (str[i] == '৩') temp += '3';
            else if (str[i] == '৪') temp += '4';
            else if (str[i] == '৫') temp += '5';
            else if (str[i] == '৬') temp += '6';
            else if (str[i] == '৭') temp += '7';
            else if (str[i] == '৮') temp += '8';
            else if (str[i] == '৯') temp += '9';
            else if (str[i] == '০') temp += '0';
            else if (str[i] == '”') temp += '\"';
            else temp += str[i];
        }
        return temp;
    }
    public static string OUT(string str)
    {
        var temp = "";
        for (var i = 0; i < str.Length; i++)
        {
            if (str[i] == '1') temp += '১';
            else if (str[i] == '2') temp += '২';
            else if (str[i] == '3') temp += '৩';
            else if (str[i] == '4') temp += '৪';
            else if (str[i] == '5') temp += '৫';
            else if (str[i] == '6') temp += '৬';
            else if (str[i] == '7') temp += '৭';
            else if (str[i] == '8') temp += '৮';
            else if (str[i] == '9') temp += '৯';
            else if (str[i] == '0') temp += '০';
            else if (str[i] == '\"') temp += '”';
            else temp += str[i];
        }
        return temp;
    }
}
