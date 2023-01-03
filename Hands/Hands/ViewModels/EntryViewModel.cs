using System;
using System.Reactive;
using System.Threading.Tasks;
using ReactiveUI;
using Xamarin.Forms;
using Hands.Views;
using Hands.Models;
using Hands.Services;
using Splat;
using System.Collections.ObjectModel;
using DynamicData;
using System.Reactive.Linq;
using System.Reactive.Disposables;
using DynamicData.Binding;
using DynamicData.Kernel;

namespace Hands.ViewModels
{
    public class TransactionWithAccount
    {
        public TTransaction Transaction { get; set; }

        public TAccount Account { get; set; }
        public string AccountName { get; set; }

        public TransactionWithAccount() { }
        public TransactionWithAccount(TTransaction transaction, TAccount account)
        {
            Transaction = transaction;
            Account = account;
            AccountName = account.Name;
        }
        public TransactionWithAccount(TTransaction transaction, Optional<TAccount> account)
        {
            Transaction = transaction;
            Account = account.HasValue ? account.Value : null;
            AccountName = account.HasValue ? account.Value.Name : "❓ Nowhere";
        }
    }

    public class TransactionWithAccountWithCategory : TransactionWithAccount
    {
        public TCategory Category { get; set; }
        public string CategoryName { get; set; }

        public TransactionWithAccountWithCategory() { }
        public TransactionWithAccountWithCategory(
            TransactionWithAccount transaction, Optional<TCategory> category)
            : base(transaction.Transaction, transaction.Account)
        {
            Category = category.HasValue ? category.Value : null;
            CategoryName = category.HasValue ? category.Value.Name : "❓ Uncategorised";
        }
    }

    public class EntryViewModel : ReactiveObject, IDisposable
    {
        private readonly ISettingsService settingsService;
        private readonly ITransactionService transactionService;

        public EntryViewModel()
        {
            settingsService = Locator.Current.GetService<ISettingsService>();
            transactionService = Locator.Current.GetService<ITransactionService>();

            var accountsObservable = settingsService
                .ConnectAccountsSetting()
                .RefCount()
                .ObserveOn(RxApp.MainThreadScheduler);

            var categoriesObservable = settingsService
                .ConnectCategoriesSetting()
                .RefCount()
                .ObserveOn(RxApp.MainThreadScheduler);

            var transactionsObservable = transactionService
                .Connect()
                .RefCount()
                .ObserveOn(RxApp.MainThreadScheduler);

            var transactionsWithAccountsObservable = accountsObservable
                .RightJoin(
                    transactionsObservable, tx => tx.AccountId,
                    (account, transaction) => new TransactionWithAccount(transaction, account))
                .ChangeKey(tx => tx.Transaction.Id);

            var transactionsDisposable = categoriesObservable
                .RightJoin(
                    transactionsWithAccountsObservable, tx => tx.Transaction.CategoryId,
                    (category, transaction) => new TransactionWithAccountWithCategory(transaction, category))
                .ChangeKey(tx => tx.Transaction.Id)
                .Sort(SortExpressionComparer<TransactionWithAccountWithCategory>
                    .Descending(tx => tx.Transaction.CreatedAt))
                .Bind(out transactions)
                .DisposeMany()
                .Subscribe();

            AddCommand = ReactiveCommand.CreateFromTask(ExecuteAddCommand);
            EditCommand = ReactiveCommand.CreateFromTask<TransactionWithAccountWithCategory>(ExecuteEditCommand);

            _cleanUp = new CompositeDisposable(transactionsDisposable);
        }

        private readonly ReadOnlyObservableCollection<TransactionWithAccountWithCategory> transactions;
        public ReadOnlyObservableCollection<TransactionWithAccountWithCategory> Transactions => transactions;

        public ReactiveCommand<Unit, Unit> AddCommand { get; set; }

        public ReactiveCommand<TransactionWithAccountWithCategory, Unit> EditCommand { get; set; }

        private async Task ExecuteAddCommand()
            => await Shell.Current.Navigation.PushModalAsync(
                new NavigationPage(new EntryDetailPage()));

        private async Task ExecuteEditCommand(TransactionWithAccountWithCategory transaction)
            => await Shell.Current.Navigation.PushModalAsync(
                new NavigationPage(new EntryDetailPage(transaction)));

        private readonly IDisposable _cleanUp;
        public void Dispose() { _cleanUp.Dispose(); }
    }
}
