namespace PrusaPushDispatcher.Models
{
    internal class PrinterInfo
    {
        public Storage Storage { get; set; } = new Storage();
        public Printer Printer { get; set; } = new Printer();
    }
}
