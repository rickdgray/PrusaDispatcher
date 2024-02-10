using System.ComponentModel.DataAnnotations;

namespace PrusaPushDispatcher
{
    internal class Settings
    {
        [Required] public string PushoverUserKey { get; set; } = string.Empty;
        [Required] public string PushoverAppKey { get; set; } = string.Empty;
        public string PrinterUsername { get; set; } = "maker";
        [Required] public string PrinterApiKey { get; set; } = string.Empty;
        [Required] public string PrinterUrl {  get; set; } = string.Empty;
        public int PollRateInSeconds { get; set; } = 1;
    }
}
