using System;

namespace Hands.Models
{
    public enum CategoryType { Income, Expense };

    public class TCategory
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public CategoryType Type { get; set; }
    }
}
