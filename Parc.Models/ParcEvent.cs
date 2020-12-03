using System;

namespace Parc.Models
{
    public class ParcEvent
    {
        public int Id { get; set; }
        public int SourceId { get; set; }
        public DateTime Time { get; set; }
        public string Type { get; set; }
        public string Priority { get; set; }
        public string Status { get; set; }
        public string Origin { get; set; }
        public string OriginDescription { get; set; }
        public string Message { get; set; }
    }
}
