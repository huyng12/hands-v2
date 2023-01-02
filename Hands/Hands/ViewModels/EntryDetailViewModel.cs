using Xamarin.Forms;
using System;
using System.Reactive;
using ReactiveUI;
using Splat;
using System.Threading.Tasks;
using System.Reactive.Linq;
using System.Globalization;
using System.Collections.ObjectModel;
using Hands.Models;
using Hands.Services;
using DynamicData;
using DynamicData.Binding;

namespace Hands.ViewModels
{
    public class EntryDetailViewModel : ReactiveObject, IDisposable
    {
        private readonly ISettingsService settingsService;

        public EntryDetailViewModel()
        {
            settingsService = Locator.Current.GetService<ISettingsService>();

            Amount = 0;

            accountsDisposable = this.settingsService
                .ConnectAccountsSetting()
                .RefCount()
                .ObserveOn(RxApp.MainThreadScheduler)
                .Sort(SortExpressionComparer<TAccount>.Ascending(v => v.Name))
                .Bind(out accounts)
                .DisposeMany()
                .Subscribe();

            categoriesDisposable = this.settingsService
                .ConnectCategoriesSetting()
                .RefCount()
                .ObserveOn(RxApp.MainThreadScheduler)
                .Sort(SortExpressionComparer<TCategory>.Ascending(v => v.Name))
                .Bind(out categories)
                .DisposeMany()
                .Subscribe();

            amountStr = this
                .WhenAnyValue(vm => vm.Amount)
                .DistinctUntilChanged()
                .Select(amount => String.Format(CultureInfo.GetCultureInfo("en-US"), "{0:N0}", amount))
                .ToProperty(this, nameof(AmountStr));

            var canExecuteDoneCommand = this
                .WhenAnyValue(vm => vm.Amount, (amount) => amount > 0);

            DoneCommand = ReactiveCommand.CreateFromTask(
                ExecuteDoneCommand, canExecuteDoneCommand);
        }

        private readonly ReadOnlyObservableCollection<TAccount> accounts;
        public ReadOnlyObservableCollection<TAccount> Accounts => accounts;

        private readonly ReadOnlyObservableCollection<TCategory> categories;
        public ReadOnlyObservableCollection<TCategory> Categories => categories;

        readonly ObservableAsPropertyHelper<string> amountStr;
        public string AmountStr => amountStr.Value;

        private Int64 amount;
        public Int64 Amount
        {
            get => amount;
            set => this.RaiseAndSetIfChanged(ref amount, value);
        }

        public ReactiveCommand<Unit, Unit> DoneCommand { get; set; }

        private async Task ExecuteDoneCommand()
            => await Shell.Current.Navigation.PopModalAsync();

        public void OnNumericKeyboardButtonClicked(int number)
        {
            if (number == -1) Amount = Amount / 10;
            else if (number == 1000) Amount *= 1000;
            else Amount = Amount * 10 + number;
        }

        private readonly IDisposable accountsDisposable;
        private readonly IDisposable categoriesDisposable;

        public void Dispose()
        {
            accountsDisposable.Dispose();
            categoriesDisposable.Dispose();
        }
    }
}
