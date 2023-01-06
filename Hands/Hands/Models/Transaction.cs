using System;

namespace Hands.Models
{
    public class TTransaction
    {
        public TTransaction() { }
        public TTransaction(TTransaction tx)
        {
            Id = tx.Id;
            CreatedAt = tx.CreatedAt;
            Type = tx.Type;
            Note = tx.Note;
            Amount = tx.Amount;
            AccountId = tx.AccountId;
            CategoryId = tx.CategoryId;
        }

        // should be filled programmtically
        public string Id { get; set; }
        public DateTimeOffset CreatedAt { get; set; }

        // should be lifted up from `Category`
        // in case the category was removed, we still can separate transactions
        public string Type { get; set; }

        public string Note { get; set; }
        public Int64 Amount { get; set; }
        public string AccountId { get; set; }
        public string CategoryId { get; set; }
    }
}
