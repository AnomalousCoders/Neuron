namespace Neuron.Core.Logging.Utils
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

        public static LogBox Of(string s) => new LogBox(s);
    }
}