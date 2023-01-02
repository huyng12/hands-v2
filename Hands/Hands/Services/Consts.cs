using System;
using System.Collections.Generic;
using Hands.Models;

namespace Hands.Services
{
    public static class ServiceConsts
    {
        public static TSettings defaultSettings = new TSettings
        {
            Notification = NotificationSetting.EveryMorning,
            Accounts = new List<TAccount> {
                new TAccount{ Id = Guid.NewGuid().ToString(), Name = "💰 Cash" },
                new TAccount{ Id = Guid.NewGuid().ToString(), Name = "💳 Credit Card" },
            },
            Categories = new List<TCategory> {
                // Incomes
                new TCategory{ Id = Guid.NewGuid().ToString(), Type = CategoryType.Income, Name = "Salary" },
                new TCategory{ Id = Guid.NewGuid().ToString(), Type = CategoryType.Income, Name = "Business" },
                new TCategory{ Id = Guid.NewGuid().ToString(), Type = CategoryType.Income, Name = "Other" },
                // Expenses
                new TCategory{ Id = Guid.NewGuid().ToString(), Type = CategoryType.Expense, Name = "Groceries" },
                new TCategory{ Id = Guid.NewGuid().ToString(), Type = CategoryType.Expense, Name = "Snacks" },
                new TCategory{ Id = Guid.NewGuid().ToString(), Type = CategoryType.Expense, Name = "Eating Out" },
                new TCategory{ Id = Guid.NewGuid().ToString(), Type = CategoryType.Expense, Name = "Coffee" },
                new TCategory{ Id = Guid.NewGuid().ToString(), Type = CategoryType.Expense, Name = "Drinks" },
                new TCategory{ Id = Guid.NewGuid().ToString(), Type = CategoryType.Expense, Name = "Beauty" },
                new TCategory{ Id = Guid.NewGuid().ToString(), Type = CategoryType.Expense, Name = "Clothing" },
                new TCategory{ Id = Guid.NewGuid().ToString(), Type = CategoryType.Expense, Name = "Home" },
                new TCategory{ Id = Guid.NewGuid().ToString(), Type = CategoryType.Expense, Name = "Education" },
                new TCategory{ Id = Guid.NewGuid().ToString(), Type = CategoryType.Expense, Name = "Health" },
                new TCategory{ Id = Guid.NewGuid().ToString(), Type = CategoryType.Expense, Name = "Travel" },
                new TCategory{ Id = Guid.NewGuid().ToString(), Type = CategoryType.Expense, Name = "Miscellaneous" },
            }
        };
    }
}
