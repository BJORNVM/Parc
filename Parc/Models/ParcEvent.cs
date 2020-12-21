using System;

namespace Parc.Models
{
    public class ParcEvent
    {
        public int Id { get; set; }
        public DateTime Timestamp { get; set; }
        public string Type { get; set; }
        public string Category { get; set; }
        public string Priority { get; set; }
        public string Status { get; set; }
        public string Origin { get; set; }
        public string Message { get; set; }
    }
}
