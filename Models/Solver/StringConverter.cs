using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;


public class StringConverter
{
    // private static Dictionary<string, string> _replacement_map = new Dictionary<string, string>
    //     {
    //         {"[aeiou]", string.Empty},
    //         {@"\s+", String.Empty},
    //         {"g", "6"},
    //         {"e", "3"}
            
    //     };
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
}
