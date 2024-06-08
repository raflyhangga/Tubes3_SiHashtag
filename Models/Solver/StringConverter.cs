using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;


public static class StringConverter
{
    private static Dictionary<string, string> _alayCompareMap = new Dictionary<string, string>
        {
            {"g", "6"},
            {"s", "5"},
            {"z", "2"},
            {"[aeiou4013]", "\0"},
            {@"\s+", "\0"},
        };
    public static string ToCompareAlay(this string input)
    {
        // Convert all uppercase to lowercase
        string convertedString = input.ToLower();

        foreach(var dic in _alayCompareMap){
            convertedString = Regex.Replace(convertedString, dic.Key, dic.Value);
        }

        return convertedString;
    }

    private static Dictionary<string, string> _alayMap = new Dictionary<string, string>
        {
            {"g", "6"},
            {"s", "5"},
            {"z", "2"},
            {"a", "4"},
            {"o", "0"},
            {"e", "3"},
            {"[aeiou]", String.Empty},
        };

    public static string ToAlay(this string input)
    {

        StringBuilder convertedsb = new StringBuilder(input);
        // Convert all uppercase to lowercase
        for(int i = 0; i < input.Length; i++)
            convertedsb[i] = (char)Random.Shared.NextDouble()>=0.5?char.ToUpper(convertedsb[i]) : char.ToLower(convertedsb[i]);

        string convertedString = convertedsb.ToString();
        foreach(var dic in _alayMap){
            convertedString = Regex.Replace(convertedString, dic.Key, dic.Value);
        }

        return convertedString;
    }
}
