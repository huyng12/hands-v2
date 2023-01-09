using System;
using System.Collections.Generic;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Threading.Tasks;
using Akavache;
using DynamicData;
using Hands.Models;
using Newtonsoft.Json;
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
                .ObserveOn(RxApp.TaskpoolScheduler)
                .Do(items => transactions.AddOrUpdate(items))
                //.Do(transactions => Console.WriteLine(JsonConvert.SerializeObject(transactions)))
                .Subscribe();

            var saveToStorageOnChange = this.transactions
                .Connect()
                .ObserveOn(RxApp.TaskpoolScheduler)
                .Throttle(TimeSpan.FromMilliseconds(500))
                .Select(_ => this.transactions.Items)
                //.Do(transactions => Console.WriteLine(JsonConvert.SerializeObject(transactions)))
                .Do(async (transactions) => await store
                    .InsertObject<IEnumerable<TTransaction>>(storeKey, transactions))
                .Subscribe();

            _cleanUp = new CompositeDisposable(loadFromStorage, saveToStorageOnChange);
        }

        public void Reset()
        {
            transactions.Edit((source) =>
            {
                source.Clear();
                //source.AddOrUpdate(new List<TTransaction>());
                source.AddOrUpdate(ServiceConsts.sampleTransactions);
            });
        }

        public IObservable<List<TTransaction>> GetTransactionsFromStorageObservable()
            => store.GetOrCreateObject<List<TTransaction>>(
                storeKey, () => new List<TTransaction>());

        public IObservable<IChangeSet<TTransaction, string>> Connect()
            => transactions.Connect();

        public void AddNewTransaction(long amount, TAccount account,
            TCategory category, string note)
            => transactions.AddOrUpdate(new TTransaction
            {
                Id = Guid.NewGuid().ToString(),
                CreatedAt = DateTimeOffset.Now,
                Type = category.Type,
                Note = note,
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
