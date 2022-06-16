﻿using Neuron.Core.Events;

namespace Neuron.Modules.Commands.Event;

public class CommandEvent : IEvent
{
    public ICommandContext Context { get; set; }

    public CommandResult Result { get; set; } = new CommandResult();

    public bool IsHandled { get; set; } = false;

    public bool PreExecuteFailed { get; set; } = false;
}