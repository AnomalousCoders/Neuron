namespace Neuron.Core.Logging.Utils
{
    public class LogBoxes
    {
        public static readonly LogBox Successful = new("[✓]");
        public static readonly LogBox Failed = new("[✖]");
        public static readonly LogBox Waiting = new("[⏳]");
        public static readonly LogBox Time = new("[🕘]");
        public static readonly LogBox Info = new("[⚹]");
        public static readonly LogBox Magic = new("[✨]");
    }

    public class LogBox
    {
        private readonly string _value;

        public LogBox(string value)
        {
            this._value = value;
        }

        public override string ToString()
            => _value;

        public static LogBox Of(string s) 
            => new LogBox(s);
    }
}