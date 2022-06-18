using System;
using System.Collections.Generic;
using System.Linq;

namespace Neuron.Core.Dev;

public static class CasingUtils
{
    public static IEnumerable<string> SplitCasedWords(string src)
    {
        var camelSpliced = SplitCamelCase(src).ToList();
        Console.WriteLine(string.Join(" ", camelSpliced));
        var spliced = camelSpliced.SelectMany(x => x.Split(' ', '_', '-'));
        return spliced.Where(x => x.Length != 0);
    }

    public static IEnumerable<string> SplitCamelCase(string src)
    {
        var buffer = "";
        for (var i = 0; i < src.Length; i++)
        {
            var c = src[i];
            if (char.IsUpper(c))
            {
                yield return buffer;
                buffer = c.ToString();
            }
            else buffer += c;
        }

        if (buffer != "") yield return buffer;
    }

    public static string ToCase(StringCasing casing, IEnumerable<string> words)
    {
        switch (casing)
        {
            case StringCasing.Camel:
                var pascal = string.Join("", words.Select(x =>
                {
                    var aFirst = x[0];
                    var aRemoved = x.Remove(0, 1);
                    return char.ToUpper(aFirst) + aRemoved.ToLower();
                }));
                var bFirst = pascal[0];
                var bRemoved = pascal.Remove(0, 1);
                return char.ToLower(bFirst) + bRemoved;
            case StringCasing.Snake:
                return string.Join("_", words.Select(x => x.ToLower()));
            case StringCasing.Kebab:
                return string.Join("-", words.Select(x => x.ToLower()));
            case StringCasing.Pascal:
                return string.Join("", words.Select(x =>
                {
                    var cFirst = x[0];
                    var cRemoved = x.Remove(0, 1);
                    return char.ToUpper(cFirst) + cRemoved.ToLower();
                }));
            case StringCasing.SpacedPascal:
                return string.Join(" ", words.Select(x =>
                {
                    var dFirst = x[0];
                    var dRemoved = x.Remove(0, 1);
                    return char.ToUpper(dFirst) + dRemoved.ToLower();
                }));
            case StringCasing.UpperSnake:
                return string.Join("_", words.Select(x => x.ToUpper()));
            default:
                throw new ArgumentOutOfRangeException(nameof(casing), casing, null);
        }
    }

    /// <summary>
    /// Splits the string into words and rejoins them to match the defined casing.
    /// Splits space, underscore and minus seperated words, as well as pascal and
    /// camel cased string, also supporting mixed strings.
    /// Multiple separators are trimmed.
    /// </summary>
    /// <returns>the recased string</returns>
    public static string Recase(this string instance, StringCasing casing) => ToCase(casing, SplitCasedWords(instance));
}

public enum StringCasing
{
    /// <example>camelCase</example>
    Camel,
    /// <example>snake_case</example>
    Snake,
    /// <example>kebab-case</example>
    Kebab,
    /// <example>PascalCase</example>
    Pascal,
    /// <example>Spaced Pascal Case</example>
    SpacedPascal,
    /// <example>UPPER_SNAKE_CASE</example>
    UpperSnake
}