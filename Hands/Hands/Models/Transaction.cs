using System;

namespace Hands.Models
{
    public class Transaction
    {
        public string Id { get; set; }
        public Int64 Amount { get; set; }
        public string Note { get; set; }
        public string AccountId { get; set; }
        public string CategoryId { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
