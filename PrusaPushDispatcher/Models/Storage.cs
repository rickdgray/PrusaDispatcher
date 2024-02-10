namespace PrusaPushDispatcher.Models
{
    internal class Storage
    {
        public string Path { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public bool ReadOnly { get; set; }
    }
}
