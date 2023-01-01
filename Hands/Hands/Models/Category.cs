using System;

namespace Hands.Models
{
    public static class CategoryType
    {
        public static string Income = "Income";
        public static string Expense = "Expense";
    }

    public class TCategory
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Type { get; set; }
    }
}
