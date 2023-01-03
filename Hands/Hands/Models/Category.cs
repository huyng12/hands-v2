using System;

namespace Hands.Models
{
    public static class CategoryType
    {
        public static string Income = "Income";
        public static string Expense = "Expense";
    }

    public class TCategory : IEquatable<TCategory>
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Type { get; set; }

        public override int GetHashCode() => (Id, Name, Type).GetHashCode();

        public override bool Equals(object obj) => this.Equals(obj as TCategory);

        public bool Equals(TCategory other)
        {
            if (other is null) return false;
            return Id == other.Id && Name == other.Name && Type == other.Type;
        }
    }
}
