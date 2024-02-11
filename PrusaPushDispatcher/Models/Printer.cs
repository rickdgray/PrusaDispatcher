using System.Text.Json.Serialization;

namespace PrusaPushDispatcher.Models
{
    internal class Printer
    {
        [JsonPropertyName("state")]
        public string State { get; set; } = string.Empty;

        [JsonPropertyName("temp_bed")]
        public double CurrentBedTemperature { get; set; }

        [JsonPropertyName("target_bed")]
        public double TargetBedTemperature { get; set; }

        [JsonPropertyName("temp_nozzle")]
        public double CurrentNozzleTemperature { get; set; }

        [JsonPropertyName("target_nozzle")]
        public double TargetNozzleTemperature { get; set; }

        [JsonPropertyName("axis_z")]
        public double CurrentAxisZ { get; set; }

        [JsonPropertyName("axis_x")]
        public double CurrentAxisX { get; set; }

        [JsonPropertyName("axis_y")]
        public double CurrentAxisY { get; set; }

        [JsonPropertyName("flow")]
        public int Flow { get; set; }

        [JsonPropertyName("speed")]
        public int Speed { get; set; }

        [JsonPropertyName("fan_hotend")]
        public int FanHotend { get; set; }

        [JsonPropertyName("fan_print")]
        public int FanPrint { get; set; }
    }
}
