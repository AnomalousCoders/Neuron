using System;

namespace Neuron.Modules.Commands.Command;

public enum CommandStatusCode
{
    Ok = 200,
    BadSyntax = 400,
    Forbidden = 403,
    NotFound = 404,
    Error = 499
}

public static class CommandStatusCodeHelper
{
    public static CommandStatusCode Parse(int code)
    {
        foreach (var status in Enum.GetValues(typeof(CommandStatusCode)))
        {
            if ((int) status == code) return (CommandStatusCode)status;
        }

        return IsSuccessful(code) ? CommandStatusCode.Ok : CommandStatusCode.Error;
    }
        
    public static bool IsSuccessful(int code) 
        => code is >= 200 and < 300;
}