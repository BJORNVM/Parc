using System;

namespace Parc.Models
{
    public class ImportResult
    {
        public int Id { get; set; }
        public DateTime Timestamp { get; set; } = DateTime.Now;
        public string Database { get; set; }
        public bool Success { get; set; }
        public string Result { get; set; }
    }
}
