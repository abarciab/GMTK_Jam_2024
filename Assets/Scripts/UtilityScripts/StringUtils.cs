using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text.RegularExpressions;
using Unity.VisualScripting.FullSerializer;
using UnityEngine;

public static class StringUtils 
{
    private const string TagRegex = @"\[(.*?)\]";

    public static List<string> GetStringTags(string input) => StripStringTags(ref input);
    public static List<int> GetIntTags(string input) => StripIntTags(ref input);
    public static string Format(string input) => input.ToUpper().Trim();
    public static List<string> GetConditionsFromTags(List<string> Tags) => Tags.Where(x => isCondition(x)).ToList();
    public static List<string> GetEffectsFromTags(List<string> Tags) => Tags.Where(x => isEffect(x)).ToList();

    public static string GetLabelFromTags(List<string> tags)
    {
        foreach (var t in tags) {
            var current = Format(t);
            if (!current.Contains("LABEL")) continue;

            var parts = current.Split('|');
            return parts[1];
        }

        return "";
    }

    private static bool isEffect(string input)
    {
        input = Format(input);
        if (input.Contains("LABEL")) return false;
        if (input.Contains('|')) return true;
        if (input == "CEND") return true;

        return false;
    }

    private static bool isCondition(string input)
    {
        if (input.Contains('|')) return false;
        if (string.Equals(input.ToUpper().Trim(), "C")) return false;
        if (string.Equals(input.ToUpper().Trim(), "R")) return false;

        return true;
    }

    public static List<string> StripStringTags(ref string input)
    {
        var tags = Regex.Matches(input, TagRegex).Cast<Match>();
        var conditions = new List<string>();

        foreach (var match in tags) {
            var content = match.Groups[1].Value;

            if (!int.TryParse(content, out var parsedInt)) {
                conditions.Add(content);
                input = input.Replace(match.Value, "");
            }
        }

        return conditions;
    }

    public static List<int> StripIntTags(ref string input)
    {
        var tags = Regex.Matches(input, TagRegex).Cast<Match>();
        var ints = new List<int>();

        foreach (var match in tags) {
            var content = match.Groups[1].Value;
            if (int.TryParse(content, out var parsedInt)) {
                ints.Add(parsedInt);
                input = input.Replace(match.Value, "");
            }

        }

        return ints;
    }    
}
