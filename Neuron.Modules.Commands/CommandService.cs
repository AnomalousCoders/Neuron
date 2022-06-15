using System;
using System.Collections.Generic;
using System.Linq;
using Neuron.Core.Meta;

namespace Neuron.Modules.Commands
{
    public class CommandService : Service
    {
        public readonly CommandReactor GlobalCommandReactor = new CommandReactor();

        public readonly Dictionary<Type, CommandReactor> CommandReactors =
            new Dictionary<Type, CommandReactor>();

        public CommandResult Raise<TContext>(string message)
            where TContext : ICommandContext
        {
            var ev = new CommandEvent()
            {
                Context = (ICommandContext)Activator.CreateInstance(typeof(TContext))
            };

            ev.Context.FullCommand = message;
            var args = message.Split(' ').ToList();
            ev.Context.Command = args[0];
            args.RemoveAt(0);
            ev.Context.Arguments = args.ToArray();
            
            CommandReactors[typeof(TContext)].Raise(ev);

            return ev.Result;
        }
        
        public CommandResult Raise<TContext>(TContext context)
            where TContext : ICommandContext
        {
            var ev = new CommandEvent()
            {
                Context = context
            };
            
            CommandReactors[typeof(TContext)].Raise(ev);

            return ev.Result;
        }

        public CommandReactor CreateCommandReactor<TContext>()
            where TContext : ICommandContext
        {
            var reactor = new CommandReactor();
            CommandReactors[typeof(TContext)] = reactor;
            reactor.Subscribe(CallGlobalReactor);
            
            return reactor;
        }

        private void CallGlobalReactor(CommandEvent context)
        {
            GlobalCommandReactor.Raise(context);
        }
    }
}