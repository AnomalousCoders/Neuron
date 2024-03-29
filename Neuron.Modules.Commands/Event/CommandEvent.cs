﻿using Neuron.Core.Events;

namespace Neuron.Modules.Commands.Event;

public class CommandEvent : IEvent
{
    public ICommandContext Context { get; set; }

    public CommandResult Result { get; set; } = new();

    public bool IsHandled { get; set; } = false;
}