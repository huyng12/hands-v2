using System;
using System.Collections.Generic;
using Hands.Models;

namespace Hands.Services
{
    public static class ServiceConsts
    {
        public static TSettings defaultSettings = new TSettings
        {
            Notification = NotificationSetting.EveryEvening,
            Accounts = new List<Account> {
                new Account{ Id = Guid.NewGuid().ToString(), Name = "Cash" },
                new Account{ Id = Guid.NewGuid().ToString(), Name = "Credit Card" },
            },
            Categories = new List<Category> {
                // Incomes
                new Category{ Id = Guid.NewGuid().ToString(), Type = CategoryType.Income, Name = "Salary" },
                new Category{ Id = Guid.NewGuid().ToString(), Type = CategoryType.Income, Name = "Business" },
                new Category{ Id = Guid.NewGuid().ToString(), Type = CategoryType.Income, Name = "Other" },
                // Expenses
                new Category{ Id = Guid.NewGuid().ToString(), Type = CategoryType.Expense, Name = "Groceries" },
                new Category{ Id = Guid.NewGuid().ToString(), Type = CategoryType.Expense, Name = "Snacks" },
                new Category{ Id = Guid.NewGuid().ToString(), Type = CategoryType.Expense, Name = "Eating Out" },
                new Category{ Id = Guid.NewGuid().ToString(), Type = CategoryType.Expense, Name = "Coffee" },
                new Category{ Id = Guid.NewGuid().ToString(), Type = CategoryType.Expense, Name = "Drinks" },
                new Category{ Id = Guid.NewGuid().ToString(), Type = CategoryType.Expense, Name = "Beauty" },
                new Category{ Id = Guid.NewGuid().ToString(), Type = CategoryType.Expense, Name = "Clothing" },
                new Category{ Id = Guid.NewGuid().ToString(), Type = CategoryType.Expense, Name = "Home" },
                new Category{ Id = Guid.NewGuid().ToString(), Type = CategoryType.Expense, Name = "Education" },
                new Category{ Id = Guid.NewGuid().ToString(), Type = CategoryType.Expense, Name = "Health" },
                new Category{ Id = Guid.NewGuid().ToString(), Type = CategoryType.Expense, Name = "Travel" },
                new Category{ Id = Guid.NewGuid().ToString(), Type = CategoryType.Expense, Name = "Miscellaneous" },
            }
        };
    }
}
