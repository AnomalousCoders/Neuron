using Serilog;
using Serilog.Core;
using Serilog.Events;

namespace Neuron.Core.Logging
{
    public class LogBoxes
    {
        public static LogBox Successful = new("[✓]");
        public static LogBox Failed = new("[✖]");
        public static LogBox Waiting = new("[⏳]");
        public static LogBox Time = new("[🕘]");
        public static LogBox Info = new("[⚹]");
        public static LogBox Magic = new("[✨]");
    }

    public class LogBox
    {
        public string s;

        public LogBox(string s)
        {
            this.s = s;
        }

        public override string ToString() => s;
    }
}