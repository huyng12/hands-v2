using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DynamicData;
using Hands.Models;

namespace Hands.Services
{
    public interface ITransactionService
    {
        void Reset();
        IObservable<IChangeSet<TTransaction, string>> Connect();
        IObservable<List<TTransaction>> GetTransactionsFromStorageObservable();
        void AddNewTransaction(Int64 amount, TAccount account,
            TCategory category, string note);
        void UpdateTransaction(TTransaction transaction);
        void RemoveTransaction(TTransaction transaction);
    }
}
