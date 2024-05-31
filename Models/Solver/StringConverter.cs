using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;


public class StringConverter
{
    private static Dictionary<string, string> _replacement_map = new Dictionary<string, string>
        {
            {"g", "6"},
            {"e", "3"},
            {"i", "1"},
            {"o", "0"},
            {"s", "5"},
            {"z", "2"},
            {"a", "4"},
            {@"\s+", String.Empty},
            {"[aeiou]", string.Empty},
            
        };
    public static string StringConvert(string input)
    {
        // Convert all uppercase to lowercase
        string lowerCaseString = input.ToLower();

        // Delete all vowels
        string noVowelsString = Regex.Replace(lowerCaseString, "[aeiou]", string.Empty);

        // Delete all whitespace
        string finalString = Regex.Replace(noVowelsString, @"\s+", string.Empty);

        return finalString;
    }

    public static string AlayConvert(string input)
    {
        // Convert all uppercase to lowercase
        string convertedString = input;
        foreach (var dic in _replacement_map)
        {
            Regex.Replace(convertedString, dic.Key, dic.Value);
        }

        return convertedString;
    }
}
