using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;


public class StringConverter
{
    private static Dictionary<string, string> _replacement_map = new Dictionary<string, string>
        {
            {"g", "6"},
            {"s", "5"},
            {"z", "2"},
            {"[aeiou4013]", "\0"},
            {@"\s+", "\0"},
        };
    public static string StringConvert(string input)
    {
        // Convert all uppercase to lowercase
        string convertedString = input.ToLower();

        foreach(var dic in _replacement_map){
            convertedString = Regex.Replace(convertedString, dic.Key, dic.Value);
        }

        return convertedString;
    }
}
