using System.Text;

namespace Neuron.Core.Dependencies;

/// <summary>
/// Utility for building dependency trees
/// </summary>
public class TreeBuilder
{
    private const string IndentString = "-";
    private int _level = 1;
    public StringBuilder StringBuilder = new StringBuilder();
    
    public void WriteLine(string payload)
    {
        if (_level == 1)
        {
            StringBuilder.Append(">");
            StringBuilder.AppendLine(payload);
        }
        else
        {
            for (int i = 0; i < _level; i++)
            {
                StringBuilder.Append(IndentString);
            }
            StringBuilder.AppendLine(payload);
        }
    }

    public void WriteLine()
    {
        StringBuilder.AppendLine();
    }
    
    public void Increment()
    {
        _level += 1;
    }

    public void Decrement()
    {
        _level -= 1;
    }

    public void Reset()
    {
        _level = 1;
    }
}