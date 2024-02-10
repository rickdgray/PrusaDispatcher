namespace PrusaPushDispatcher
{
    internal class Settings
    {
        public string PushoverUserKey { get; set; } = string.Empty;
        public string PushoverAppKey { get; set; } = string.Empty;
        public string PrinterUsername {  get; set; } = string.Empty;
        public string PrinterApiKey { get; set; } = string.Empty;
        public string PrinterUrl {  get; set; } = string.Empty;
        public int PollRateInSeconds { get; set; }
    }
}
