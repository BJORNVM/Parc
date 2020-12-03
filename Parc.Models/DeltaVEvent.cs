using System;

namespace Parc.Models
{
    public class DeltaVEvent
    {
        public DateTime Date_Time { get; set; }
        public short FracSec { get; set; }
        public string Event_Type { get; set; }
        public string Event_SubType { get; set; }
        public string Category { get; set; }
        public string Area { get; set; }
        public string Node { get; set; }
        public string Unit { get; set; }
        public string Module { get; set; }
        public string Module_Description { get; set; }
        public string Attribute { get; set; }
        public string State { get; set; }
        public string Event_Level { get; set; }
        public string Desc1 { get; set; }
        public string Desc2 { get; set; }
        public short IsArchived { get; set; }
        public int Ord { get; set; }
    }
}
