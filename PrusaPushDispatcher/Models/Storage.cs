using System.Text.Json.Serialization;

namespace PrusaPushDispatcher.Models
{
    internal class Storage
    {
        [JsonPropertyName("path")]
        public string Path { get; set; } = string.Empty;

        [JsonPropertyName("name")]
        public string Name { get; set; } = string.Empty;

        [JsonPropertyName("read_only")]
        public bool ReadOnly { get; set; }
    }
}
