namespace PrusaPushDispatcher.Models
{
    internal class Printer
    {
        public string State { get; set; } = string.Empty;
        public double TempBed { get; set; }
        public double TargetBed { get; set; }
        public double TempNozzle { get; set; }
        public double TargetNozzle { get; set; }
        public double AxisZ { get; set; }
        public double AxisX { get; set; }
        public double AxisY { get; set; }
        public int Flow { get; set; }
        public int Speed { get; set; }
        public int FanHotend { get; set; }
        public int FanPrint { get; set; }
    }
}
