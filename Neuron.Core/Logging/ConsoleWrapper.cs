using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Neuron.Core.Logging;

public static class ConsoleWrapper
{
    public static int WidthOverride = -1;
    private static int ConsoleColumns => Console.WindowWidth;
    private static int Columns => (WidthOverride == -1 ? ConsoleColumns : WidthOverride) - 1; 

    public static void Main()
    {
        var lines = WrapText(8, "Lorem ipsum dolor sit amet, consetetur sadipscing elitr, sed diam nonumy eirmod tempor invidunt ut labore et dolore magna aliquyam erat, sed diam voluptua.  At vero eos et accusam et justo duo dolores et ea rebum. Stet clita kasd gubergren, no sea takimata sanctus est Lorem ipsum dolor sit amet. Lorem ipsum dolor sit amet, consetetur sadipscing elitr, sed diam nonumy eirmod tempor invidunt ut labore et dolore magna aliquyam erat, sed diam voluptua. At vero eos et accusam et justo duo dolores et ea rebum. Stet clita kasd gubergren, no sea takimata sanctus est Lorem ipsum dolor sit amet.");
        Console.WriteLine(string.Join("\n", lines));
    }

    public static string Header(string name) => PadRight($"   ===== {name} ", "=");

    public static string PadRight(string message, string pad)
    {
        var len = message.Length;
        var filling = Columns - len;
        return $"{message}{StringHelper.Repeat(filling, pad)}";
    }

    public static string WrapTextToString(int indent, string message) => string.Join("\n", WrapText(indent, message));

    public static List<string> WrapText(int indent, string message)
    {
        var availableWidth = Columns - indent;
        var words = message.Replace("\n", " \n ").Split(' ');
        var lines = new List<string>();
        var buffer = new StringBuilder();
        for (var i = 0; i < words.Length; i++)
        {
            if (words[i] == "\n")
            {
                lines.Add(buffer.ToString());
                buffer.Clear();
                continue;
            }
            
            var length = words[i].Length;
            var expandedWidth = buffer.Length + length + 1;
            if (expandedWidth > availableWidth)
            {
                lines.Add(buffer.ToString());
                buffer.Clear();
                if (length > availableWidth)
                {
                    var hardWrapped = HardWrap(availableWidth, words[i]);
                    for (var i1 = 0; i1 < hardWrapped.Count - 1; i1++)
                    {
                        lines.Add(hardWrapped[i1]);
                    }
                    buffer.Append(hardWrapped.Last());
                }
                else
                {
                    buffer.Append(words[i]);
                }
            }
            else
            {
                buffer.Append($" {words[i]}");
            }
        }
        
        if (buffer.Length > 0) lines.Add(buffer.ToString());
        
        var indentString = StringHelper.Repeat(indent, " ");
        lines = lines.Select(x => $"{indentString}{x.TrimStart()}").ToList();
        
        return lines;
    }

    public static List<string> HardWrap(int width, string message)
    {
        var lines = new List<string>();
        var buffer = new StringBuilder();
        for (var i = 0; i < message.Length; i++)
        {
            if (buffer.Length >= width)
            {
                lines.Add(buffer.ToString());
                buffer.Clear();
            }
            
            if (buffer.Length < width)
            {
                buffer.Append(message[i]);
            }
        }
        if (buffer.Length > 0) lines.Add(buffer.ToString());

        return lines;
    }
}

public static class StringHelper {

    public static string Repeat(int amount, string str)
    {
        var indentBuilder = new StringBuilder();
        for (var i = 0; i < amount; i++)
        {
            indentBuilder.Append(str);
        }
        return indentBuilder.ToString();
    }

    public static string TrimIndent(string str) => string.Join("\n", str.Split('\n').Select(x => x.Trim()));

}