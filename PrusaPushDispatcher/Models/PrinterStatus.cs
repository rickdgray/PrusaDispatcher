namespace PrusaPushDispatcher.Models
{
    internal class PrinterStatus
    {
        // See here for openapi spec
        // https://raw.githubusercontent.com/prusa3d/Prusa-Link-Web/master/spec/openapi.yaml

        private PrinterStatus(string value)
        {
            Value = value;
        }

        public string Value { get; private set; }

        public static PrinterStatus FromValue(string value)
        {
            return new PrinterStatus(value);
        }

        public static PrinterStatus Unknown { get { return new PrinterStatus("UNKNOWN"); } }

        public static PrinterStatus Idle { get { return new PrinterStatus("IDLE"); } }
        public static PrinterStatus Busy { get { return new PrinterStatus("BUSY"); } }
        public static PrinterStatus Printing { get { return new PrinterStatus("PRINTING"); } }
        public static PrinterStatus Paused { get { return new PrinterStatus("PAUSED"); } }
        public static PrinterStatus Finished { get { return new PrinterStatus("FINISHED"); } }
        public static PrinterStatus Stopped { get { return new PrinterStatus("STOPPED"); } }
        public static PrinterStatus Error { get { return new PrinterStatus("ERROR"); } }
        public static PrinterStatus Attention { get { return new PrinterStatus("ATTENTION"); } }
        public static PrinterStatus Ready { get { return new PrinterStatus("READY"); } }

        public override string ToString()
        {
            return Value;
        }

        public override int GetHashCode()
        {
            return Value.GetHashCode();
        }

        public override bool Equals(object? obj)
        {
            if (obj == null)
            {
                return false;
            }

            if (ReferenceEquals(this, obj))
            {
                return true;
            }

            var item = obj as PrinterStatus;

            if (item == (PrinterStatus)null)
            {
                return false;
            }

            return Value.Equals(item.Value);
        }

        public static bool operator == (PrinterStatus a, PrinterStatus b)
        {
            if (a is null)
            {
                return b is null;
            }

            return a.Equals(b);
        }

        public static bool operator != (PrinterStatus a, PrinterStatus b)
        {
            return !(a == b);
        }

        public static bool operator == (string a, PrinterStatus b)
        {
            if (a is null)
            {
                return b is null;
            }

            return a.Equals(b.Value);
        }

        public static bool operator != (string a, PrinterStatus b)
        {
            return !(a == b.Value);
        }

        public static bool operator == (PrinterStatus a, string b)
        {
            if (a is null)
            {
                return b is null;
            }

            return a.Value.Equals(b);
        }

        public static bool operator != (PrinterStatus a, string b)
        {
            return !(a.Value == b);
        }
    }
}
