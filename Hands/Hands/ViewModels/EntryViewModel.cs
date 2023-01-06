using System;
using System.Reactive;
using System.Threading.Tasks;
using ReactiveUI;
using Xamarin.Forms;
using Hands.Views;
using Hands.Models;
using Hands.Services;
using Hands.Controls;
using Splat;
using System.Collections.ObjectModel;
using DynamicData;
using System.Reactive.Linq;
using System.Reactive.Disposables;
using DynamicData.Binding;
using DynamicData.Kernel;
using System.Linq;
using System.Globalization;
using System.Text.RegularExpressions;

namespace Hands.ViewModels
{
    public class TransactionWithAccount
    {
        public TTransaction Transaction { get; set; }

        public string TxDisplayName { get; set; }

        public string FormattedAmount { get; set; }
        public Color FormattedAmountColor { get; set; }

        public string FormattedCreatedAt { get; set; }

        public TAccount Account { get; set; }
        public string AccountName { get; set; }

        private void ComputeFormattedProperties()
        {
            Func<Int64, string> formatMoney = n => String.Format(
                    CultureInfo.GetCultureInfo("en-US"), "{0:N0}", n);
            if (Transaction != null)
            {
                bool isExpense = Transaction.Type == CategoryType.Expense;
                FormattedAmount = formatMoney(Transaction.Amount);
                FormattedAmountColor = isExpense ? Color.Black : Color.FromHex("098D1D" /* green */);
                FormattedCreatedAt = Transaction.CreatedAt
                    .ToString("MMMM dd, yyyy", CultureInfo.GetCultureInfo("en-US"));
            }
        }

        public TransactionWithAccount() { this.ComputeFormattedProperties(); }
        public TransactionWithAccount(TTransaction transaction, TAccount account)
        {
            Transaction = transaction;
            Account = account;
            AccountName = account.Name;
            this.ComputeFormattedProperties();
        }

        public TransactionWithAccount(TTransaction transaction, Optional<TAccount> account)
        {
            Transaction = transaction;
            Account = account.HasValue ? account.Value : null;
            AccountName = account.HasValue ? account.Value.Name : "❓ Nowhere";
            this.ComputeFormattedProperties();
        }
    }

    public class TransactionWithAccountWithCategory : TransactionWithAccount
    {
        public TCategory Category { get; set; }

        public string Icon { get; set; }
        public string CategoryName { get; set; }

        private void ComputeDisplayProperties()
        {
            if (CategoryName == null) return;

            bool isExpense = Transaction.Type == CategoryType.Expense;
            bool isStartsWithEmoji = !Regex.IsMatch(
                CategoryName[0].ToString(),
                "[a-z]", RegexOptions.IgnoreCase);

            string[] parts = CategoryName.Split(' ');

            Icon = isStartsWithEmoji ? parts.First() : isExpense ? "↗️" : "↘️";
            TxDisplayName = !String.IsNullOrEmpty(Transaction.Note)
                ? $"\"{Transaction.Note}\""
                : isStartsWithEmoji
                    ? String.Join(" ", parts.Skip(1))
                    : CategoryName;
        }

        public TransactionWithAccountWithCategory() { this.ComputeDisplayProperties(); }
        public TransactionWithAccountWithCategory(
            TransactionWithAccount transaction, Optional<TCategory> category)
            : base(transaction.Transaction, transaction.Account)
        {
            Category = category.HasValue ? category.Value : null;
            CategoryName = category.HasValue ? category.Value.Name : "❓ Uncategorised";
            this.ComputeDisplayProperties();
        }
    }

    public class TransactionGroupKey : IEquatable<TransactionGroupKey>
    {
        public string CreatedAt { get; set; }
        public string TotalSpent { get; set; }

        public override int GetHashCode() => (CreatedAt, TotalSpent).GetHashCode();

        public override bool Equals(object obj) => this.Equals(obj as TransactionGroupKey);

        public bool Equals(TransactionGroupKey other)
        {
            if (other is null) return false;
            return CreatedAt == other.CreatedAt && TotalSpent == other.TotalSpent;
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

            Func<Int64, string> formatMoney = n => String.Format(
                    CultureInfo.GetCultureInfo("en-US"), "{0:N0}", n);

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
                .Group(tx => tx.FormattedCreatedAt)
                .Transform(group => new ObservableGroupedCollection<string,
                    TransactionWithAccountWithCategory, string, Int64, string>(
                        group,
                        SortExpressionComparer<TransactionWithAccountWithCategory>
                            .Descending(tx => tx.Transaction.CreatedAt),
                        q => q.Items
                            .Where(item => item.Transaction.Type == CategoryType.Expense)
                            .Sum(item => item.Transaction.Amount),
                        totalSpent => formatMoney(totalSpent)))
                .Sort(SortExpressionComparer<ObservableGroupedCollection<string,
                    TransactionWithAccountWithCategory, string, Int64, string>>
                        .Descending(group => group.Key))
                .Bind(out transactions)
                .DisposeMany()
                .Subscribe();

            totalSpent = transactionsObservable
                .Filter(tx => tx.Type == CategoryType.Expense)
                .QueryWhenChanged(q => q.Items.Sum(tx => tx.Amount))
                .ToProperty(this, nameof(TotalSpent));

            totalReceived = transactionsObservable
                .Filter(tx => tx.Type == CategoryType.Income)
                .QueryWhenChanged(q => q.Items.Sum(tx => tx.Amount))
                .ToProperty(this, nameof(TotalReceived));

            formattedTotalReceived = this
                .WhenAnyValue(vm => vm.TotalReceived)
                .DistinctUntilChanged()
                .Select(total => formatMoney(total))
                .ToProperty(this, nameof(FormattedTotalReceived));

            formattedTotalSpent = this
                .WhenAnyValue(vm => vm.TotalSpent)
                .DistinctUntilChanged()
                .Select(total => formatMoney(total))
                .ToProperty(this, nameof(FormattedTotalSpent));

            formattedTotalBalance = this.WhenAnyValue(
                    vm => vm.TotalReceived,
                    vm => vm.TotalSpent,
                    (received, spent) => received - spent)
                .Select(balance => formatMoney(balance))
                .ToProperty(this, nameof(FormattedTotalBalance));

            AddCommand = ReactiveCommand.CreateFromTask(ExecuteAddCommand);
            EditCommand = ReactiveCommand.CreateFromTask<
                TransactionWithAccountWithCategory>(ExecuteEditCommand);

            _cleanUp = new CompositeDisposable(transactionsDisposable);
        }

        private readonly ReadOnlyObservableCollection<
            ObservableGroupedCollection<
                string,
                TransactionWithAccountWithCategory,
                string, Int64, string>> transactions;
        public ReadOnlyObservableCollection<
            ObservableGroupedCollection<
                string,
                TransactionWithAccountWithCategory,
                string, Int64, string>> Transactions => transactions;

        readonly ObservableAsPropertyHelper<string> formattedTotalBalance;
        public string FormattedTotalBalance => formattedTotalBalance.Value;

        readonly ObservableAsPropertyHelper<Int64> totalSpent;
        public Int64 TotalSpent => totalSpent.Value;

        readonly ObservableAsPropertyHelper<string> formattedTotalSpent;
        public string FormattedTotalSpent => formattedTotalSpent.Value;

        readonly ObservableAsPropertyHelper<Int64> totalReceived;
        public Int64 TotalReceived => totalReceived.Value;

        readonly ObservableAsPropertyHelper<string> formattedTotalReceived;
        public string FormattedTotalReceived => formattedTotalReceived.Value;

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
