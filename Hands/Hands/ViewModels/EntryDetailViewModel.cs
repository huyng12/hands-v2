using Xamarin.Forms;
using System;
using System.Reactive;
using ReactiveUI;
using Splat;
using System.Reactive.Disposables;
using System.Threading.Tasks;
using System.Reactive.Linq;
using System.Globalization;
using System.Collections.ObjectModel;
using Hands.Models;
using Hands.Services;
using DynamicData;
using DynamicData.Binding;
using System.Linq;
using Newtonsoft.Json;

namespace Hands.ViewModels
{
    public class EntryDetailViewModel : ReactiveObject, IDisposable
    {
        private readonly ISettingsService settingsService;
        private readonly ITransactionService transactionService;

        private readonly TransactionWithAccountWithCategory transaction;

        public EntryDetailViewModel(TransactionWithAccountWithCategory transaction)
        {
            this.transaction = transaction;

            settingsService = Locator.Current.GetService<ISettingsService>();
            transactionService = Locator.Current.GetService<ITransactionService>();

            #region Initial UI states

            // Note: Technically, variables hold numbers will be initialized with 0
            // Because of it, setting index to 0 (zero) will not trigger UI to update
            // REMEMBER to assign all indexes to -1
            SelectedAccountIndex = -1;
            SelectedCategoryIndex = -1;

            // Update UI states to match with input transaction
            Amount = transaction != null ? transaction.Transaction.Amount : 0;
            SelectedCategoryType = transaction != null
                ? transaction.Transaction.Type : "Expense";

            #endregion

            bool shouldSetSelectedAccountIndex = transaction == null;
            var accountsDisposable = this.settingsService
                .ConnectAccountsSetting()
                .RefCount()
                .ObserveOn(RxApp.MainThreadScheduler)
                .Bind(out accounts)
                .DisposeMany()
                .Subscribe(_ =>
                {
                    if (shouldSetSelectedAccountIndex) { SelectedAccountIndex = 0; return; }
                    shouldSetSelectedAccountIndex = true;
                    if (transaction.Account != null)
                        SelectedAccount = transaction.Account;
                });

            var typeFilter = this
                .WhenAnyValue(vm => vm.SelectedCategoryType)
                .Select(CreateFilterByType);

            bool shouldSetSelectedCategoryIndex = transaction == null;
            var categoriesDisposable = this.settingsService
                .ConnectCategoriesSetting()
                .RefCount()
                .ObserveOn(RxApp.MainThreadScheduler)
                .Filter(typeFilter)
                .DistinctUntilChanged()
                .Bind(out categories)
                .DisposeMany()
                .Subscribe(_ =>
                {
                    if (shouldSetSelectedCategoryIndex) { SelectedCategoryIndex = 0; return; }
                    shouldSetSelectedCategoryIndex = true;
                    if (transaction.Category != null)
                        SelectedCategory = transaction.Category;
                });

            amountStr = this
                .WhenAnyValue(vm => vm.Amount)
                .DistinctUntilChanged()
                .Select(amount => String.Format(CultureInfo.GetCultureInfo("en-US"), "{0:N0}", amount))
                .ToProperty(this, nameof(AmountStr));

            var canExecuteDoneCommand = this
                .WhenAnyValue(
                    vm => vm.Amount, vm => vm.SelectedAccount, vm => vm.SelectedCategory,
                    (amount, account, category) =>
                        amount > 0 && account != null && category != null);

            DoneCommand = ReactiveCommand.CreateFromTask(
                ExecuteDoneCommand, canExecuteDoneCommand);

            _cleanUp = new CompositeDisposable(accountsDisposable, categoriesDisposable);
        }

        private readonly ReadOnlyObservableCollection<TAccount> accounts;
        public ReadOnlyObservableCollection<TAccount> Accounts => accounts;

        private readonly ReadOnlyObservableCollection<TCategory> categories;
        public ReadOnlyObservableCollection<TCategory> Categories => categories;

        readonly ObservableAsPropertyHelper<string> amountStr;
        public string AmountStr => amountStr.Value;

        private string selectedCategoryType;
        public string SelectedCategoryType
        {
            get => selectedCategoryType;
            set => this.RaiseAndSetIfChanged(ref selectedCategoryType, value);
        }

        private int selectedAccountIndex;
        public int SelectedAccountIndex
        {
            get => selectedAccountIndex;
            set => this.RaiseAndSetIfChanged(ref selectedAccountIndex, value);
        }

        private TAccount selectedAccount;
        public TAccount SelectedAccount
        {
            get => selectedAccount;
            set => this.RaiseAndSetIfChanged(ref selectedAccount, value);
        }

        private int selectedCategoryIndex;
        public int SelectedCategoryIndex
        {
            get => selectedCategoryIndex;
            set => this.RaiseAndSetIfChanged(ref selectedCategoryIndex, value);
        }

        private TCategory selectedCategory;
        public TCategory SelectedCategory
        {
            get => selectedCategory;
            set => this.RaiseAndSetIfChanged(ref selectedCategory, value);
        }

        private Int64 amount;
        public Int64 Amount
        {
            get => amount;
            set => this.RaiseAndSetIfChanged(ref amount, value);
        }

        public ReactiveCommand<Unit, Unit> DoneCommand { get; set; }

        private async Task ExecuteDoneCommand()
        {
            if (transaction != null)
            {
                TTransaction tx = new TTransaction(transaction.Transaction);
                tx.Amount = Amount;
                tx.Type = SelectedCategory.Type;
                tx.AccountId = SelectedAccount.Id;
                tx.CategoryId = SelectedCategory.Id;
                transactionService.UpdateTransaction(tx);
            }
            else
            {
                transactionService.AddNewTransaction(Amount, SelectedAccount, SelectedCategory);
            }

            await Shell.Current.Navigation.PopModalAsync();
        }

        public void OnNumericKeyboardButtonClicked(int number)
        {
            if (number == -1) Amount = Amount / 10;
            else if (number == 1000) Amount *= 1000;
            else Amount = Amount * 10 + number;
        }

        private Func<TCategory, bool> CreateFilterByType(string type)
        {
            if (String.IsNullOrEmpty(type)) return category => true;
            return category => category.Type == type;
        }

        private readonly IDisposable _cleanUp;
        public void Dispose() => _cleanUp.Dispose();
    }
}
