using System;
using System.Collections.Generic;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using Akavache;
using DynamicData;
using Hands.Models;
using ReactiveUI;

namespace Hands.Services
{
    public class TransactionService : ITransactionService, IDisposable
    {
        private readonly IBlobCache store = BlobCache.LocalMachine;

        private readonly SourceCache<TTransaction, string> transactions
            = new SourceCache<TTransaction, string>(tx => tx.Id);

        public TransactionService()
        {
            var loadFromStorage = this.GetTransactionsFromStorageObservable()
                .Do(items => transactions.AddOrUpdate(items))
                .Subscribe();

            var saveToStorageOnChange = this.transactions
                .Connect()
                .ObserveOn(RxApp.TaskpoolScheduler)
                .Throttle(TimeSpan.FromMilliseconds(500))
                .Select(_ => this.transactions.Items)
                .Do(async (transactions) => await store
                    .InsertObject<IEnumerable<TTransaction>>(storeKey, transactions))
                .Subscribe();

            _cleanUp = new CompositeDisposable(loadFromStorage, saveToStorageOnChange);
        }

        public IObservable<List<TTransaction>> GetTransactionsFromStorageObservable()
            => store.GetOrCreateObject<List<TTransaction>>(
                storeKey, () => new List<TTransaction>());

        public IObservable<IChangeSet<TTransaction, string>> Connect()
            => transactions.Connect();

        public void AddNewTransaction(long amount, TAccount account, TCategory category)
            => transactions.AddOrUpdate(new TTransaction
            {
                Id = Guid.NewGuid().ToString(),
                CreatedAt = DateTime.Now,
                Type = category.Type,
                Amount = amount,
                AccountId = account.Id,
                CategoryId = category.Id
            });

        public void RemoveTransaction(TTransaction transaction)
            => transactions.Remove(transaction);

        public void UpdateTransaction(TTransaction transaction)
            => transactions.AddOrUpdate(transaction);

        private readonly string storeKey = "transactions";

        private readonly IDisposable _cleanUp;
        public void Dispose() => _cleanUp.Dispose();
    }
}
