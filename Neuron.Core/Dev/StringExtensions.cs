namespace Neuron.Core.Dev;

public static class StringExtensions
{
    public static string Format(this string str, params object[] args) => string.Format(str, args);
    public static string Format(this string str, object arg0) => string.Format(str, arg0);
    public static string Format(this string str, object arg0, object arg1) => string.Format(str, arg0, arg1);
    public static string Format(this string str, object arg0, object arg1, object arg2) => string.Format(str, arg0, arg1, arg2);
}