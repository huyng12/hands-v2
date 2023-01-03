using System;
using System.Collections.Generic;
using DynamicData;
using Hands.Models;

namespace Hands.Services
{
    public interface ITransactionService
    {
        IObservable<IChangeSet<TTransaction, string>> Connect();
        IObservable<List<TTransaction>> GetTransactionsFromStorageObservable();
        void AddNewTransaction(Int64 amount, TAccount account, TCategory category);
        void UpdateTransaction(TTransaction transaction);
        void RemoveTransaction(TTransaction transaction);
    }
}
