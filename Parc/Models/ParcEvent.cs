using System;

namespace Parc.Models
{
    public class ParcEvent : IEquatable<ParcEvent>
    {
        public int Id { get; set; }
        public DateTime Timestamp { get; set; }
        public string Source { get; set; }
        public string Type { get; set; }
        public string Category { get; set; }
        public string Priority { get; set; }
        public string Status { get; set; }
        public string Origin { get; set; }
        public string Message { get; set; }
 
        public string UniqueIdentifier => Timestamp.Ticks + Source;

        public bool Equals(ParcEvent other)
        {
            // Check whether the compared object is null
            if (Object.ReferenceEquals(other, null)) return false;

            // Check whether the compared object references the same data
            if (Object.ReferenceEquals(this, other)) return true;

            // Check whether the properties are equal
            return UniqueIdentifier.Equals(other.UniqueIdentifier);
        }

        // If Equals() returns true for a pair of objects then GetHashCode() must return the same value for these objects
        public override int GetHashCode() => UniqueIdentifier == null ? 0 : UniqueIdentifier.GetHashCode();
    }
}
