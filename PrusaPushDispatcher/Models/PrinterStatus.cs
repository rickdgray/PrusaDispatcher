using System.Text.Json.Serialization;

namespace PrusaPushDispatcher.Models
{
    internal class PrinterStatus
    {
        [JsonPropertyName("storage")]
        public Storage Storage { get; set; } = new Storage();

        [JsonPropertyName("printer")]
        public Printer Printer { get; set; } = new Printer();
    }
}
