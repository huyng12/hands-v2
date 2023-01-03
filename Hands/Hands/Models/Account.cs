using System;

namespace Hands.Models
{
    public class TAccount : IEquatable<TAccount>
    {
        public string Id { get; set; }
        public string Name { get; set; }

        public override int GetHashCode() => (Id, Name).GetHashCode();

        public override bool Equals(object obj) => this.Equals(obj as TAccount);

        public bool Equals(TAccount other)
        {
            if (other is null) return false;
            return Id == other.Id && Name == other.Name;
        }
    }
}
