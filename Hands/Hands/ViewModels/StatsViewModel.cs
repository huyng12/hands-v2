using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using ReactiveUI;
using Splat;
using Hands.Models;
using Hands.Services;
using System.Reactive.Linq;
using DynamicData;
using System.Reactive.Disposables;
using System.Collections.ObjectModel;
using DynamicData.Alias;
using DynamicData.Binding;
using System.ComponentModel;
using System.Reactive;
using Newtonsoft.Json;
using DynamicData.Kernel;
using System.Text.RegularExpressions;
using System.Diagnostics;

namespace Hands.ViewModels
{
    public class TransactionGrouping<TGroupKey, TObject, TKey>
        : ObservableCollectionExtended<TObject>, IDisposable
    {
        public TGroupKey Key { get; }

        public TransactionGrouping(IGroup<TObject, TKey, TGroupKey> group)
        {
            this.Key = group.Key;
            _cleanUp = group.Cache.Connect().Bind(this).Subscribe();
        }

        private readonly IDisposable _cleanUp;
        public void Dispose() { _cleanUp.Dispose(); }
    }

    public class TransactionColumn : ReactiveObject, IDisposable
    {
        private Int64 amount;
        public Int64 Amount
        {
            get => amount;
            set => this.RaiseAndSetIfChanged(ref amount, value);
        }

        private DateTime date;
        public DateTime Date
        {
            get => date;
            set => this.RaiseAndSetIfChanged(ref date, value);
        }

        public TransactionColumn(IGroup<TTransaction, string, DateTime> group)
        {
            this.Date = group.Key;
            _cleanUp = group.Cache
                .Connect()
                .QueryWhenChanged(q => q.Items.Sum(tx => tx.Amount))
                .Subscribe(total => Amount = total);
        }

        private readonly IDisposable _cleanUp;
        public void Dispose() { _cleanUp?.Dispose(); }
    }

    public static class IntervalTypes
    {
        public static string Days = "Days";
    }

    public static class TimeframeViews
    {
        public static string Week = "Week";
        public static string Month = "Month";
    }

    public class TTimeframe
    {
        public double Interval { get; set; }
        public string IntervalType { get; set; }

        public string View { get; set; }

        public DateTime From { get; set; }
        public DateTime FromSf { get; set; }
        public DateTime To { get; set; }
    }

    public class TransactionWithCategory
    {
        public TTransaction Transaction { get; set; }
        public TCategory Category { get; set; }
        public string CategoryName { get; set; }

        public TransactionWithCategory(
            TTransaction transaction, Optional<TCategory> category)
        {
            Transaction = transaction;
            Category = category.HasValue ? category.Value : null;
            CategoryName = category.HasValue ? category.Value.Name : "❓ Uncategorised";
        }
    }

    public class TransactionWithCategoryGrouping : ReactiveObject, IDisposable
    {
        private string categoryName;
        public string CategoryName
        {
            get => categoryName;
            set => this.RaiseAndSetIfChanged(ref categoryName, value);
        }

        private string icon;
        public string Icon
        {
            get => icon;
            set => this.RaiseAndSetIfChanged(ref icon, value);
        }

        private Int64 count;
        public Int64 Count
        {
            get => count;
            set => this.RaiseAndSetIfChanged(ref count, value);
        }

        private Int64 total;
        public Int64 Total
        {
            get => total;
            set => this.RaiseAndSetIfChanged(ref total, value);
        }

        private string formattedTotal;
        public string FormattedTotal
        {
            get => formattedTotal;
            set => this.RaiseAndSetIfChanged(ref formattedTotal, value);
        }

        public TransactionWithCategoryGrouping(IGroup<TransactionWithCategory
            , string, string> group)
        {
            var fullCategoryName = group.Key;

            bool isStartsWithEmoji = !Regex.IsMatch(
                fullCategoryName[0].ToString(),
                "[a-z]", RegexOptions.IgnoreCase);

            string[] parts = fullCategoryName.Split(' ');

            Icon = isStartsWithEmoji ? parts.First() : "";
            CategoryName = isStartsWithEmoji
                    ? String.Join(" ", parts.Skip(1))
                    : fullCategoryName;

            Func<Int64, string> formatMoney = n => String.Format(
                    CultureInfo.GetCultureInfo("en-US"), "{0:N0}", n);

            var totalDisposable = group.Cache
                .Connect()
                .QueryWhenChanged(q => q.Items.Sum(tx => tx.Transaction.Amount))
                .Subscribe(total =>
                {
                    Total = total;
                    FormattedTotal = formatMoney(total);
                });

            var amountDisposable = group.Cache
                .Connect()
                .QueryWhenChanged(q => q.Items.Count())
                .Subscribe(count => Count = count);

            _cleanUp = new CompositeDisposable(totalDisposable, amountDisposable);
        }

        private readonly IDisposable _cleanUp;
        public void Dispose() { _cleanUp?.Dispose(); }
    }

    public class StatsViewModel : ReactiveObject, IDisposable
    {
        private readonly ISettingsService settingsService;
        private readonly ITransactionService transactionService;

        public StatsViewModel()
        {
            settingsService = Locator.Current.GetService<ISettingsService>();
            transactionService = Locator.Current.GetService<ITransactionService>();

            Timeframe = getCurrentWeekTimeframe();

            Dcm = "Dcm";

            var filterByTimeframe = this
                .WhenAnyValue(vm => vm.Timeframe)
                .Select(createFilterByTimeframe);

            var timeframeIndicatorDisposable = this
                .WhenAnyValue(vm => vm.Timeframe)
                .Where(timeframe => timeframe != null
                    && timeframe.From != null
                    && timeframe.To != null)
                .Subscribe(timeframe =>
                {
                    var from = timeframe.From.ToString("dd/MM/yyyy");
                    var to = timeframe.To.ToString("dd/MM/yyyy");
                    TimeframeIndicator = $"{from} - {to}";
                });

            var categoriesObservable = settingsService
                .ConnectCategoriesSetting()
                .RefCount()
                .ObserveOn(RxApp.MainThreadScheduler);

            var transactionsInTimeframe = transactionService
                .Connect()
                .RefCount()
                .ObserveOn(RxApp.MainThreadScheduler)
                .Filter(filterByTimeframe);

            var transactionsDisposable = categoriesObservable
                .RightJoin(
                    transactionsInTimeframe, tx => tx.CategoryId,
                    (category, transaction) => new TransactionWithCategory(transaction, category))
                .ChangeKey(tx => tx.Transaction.Id)
                .Group(tx => tx.CategoryName)
                .Transform(group => new TransactionWithCategoryGrouping(group))
                .ChangeKey(group => group.CategoryName)
                .AutoRefresh()
                .Sort(SortExpressionComparer<TransactionWithCategoryGrouping>
                    .Descending(group => group.Total))
                .Bind(out groupedTransactions)
                .DisposeMany()
                .Subscribe();

            var transactionColumnsDisposable = transactionsInTimeframe
                .Filter(tx => tx.Type == CategoryType.Expense)
                .Group(tx => tx.CreatedAt.Date)
                .Transform(group => new TransactionColumn(group))
                .Bind(out transactions)
                .DisposeMany()
                .Subscribe();

            var incomesDisposable = transactionsInTimeframe
                .Filter(tx => tx.Type == CategoryType.Income)
                .Group(tx => tx.CreatedAt.Date)
                .Transform(group => new TransactionColumn(group))
                .Bind(out incomes)
                .DisposeMany()
                .Subscribe();

            Func<Int64, string> formatMoney = n => String.Format(
                    CultureInfo.GetCultureInfo("en-US"), "{0:N0}", n);

            var formattedTotalSpentDisposable = transactionsInTimeframe
                .Filter(tx => tx.Type == CategoryType.Expense)
                .QueryWhenChanged(q => q.Items.Sum(tx => tx.Amount))
                .Select(total => formatMoney(total))
                .Subscribe(formatted => FormattedTotalSpent = formatted);

            var formattedTotalReceivedDisposable = transactionsInTimeframe
                .Filter(tx => tx.Type == CategoryType.Income)
                .QueryWhenChanged(q => q.Items.Sum(tx => tx.Amount))
                .Select(total => formatMoney(total))
                .Subscribe(formatted => FormattedTotalReceived = formatted);

            GoBackCommand = ReactiveCommand.Create(executeGoBackCommand);
            GoNextCommand = ReactiveCommand.Create(executeGoNextCommand);

            _cleanUp = new CompositeDisposable(transactionColumnsDisposable,
                incomesDisposable, timeframeIndicatorDisposable,
                transactionsDisposable, formattedTotalSpentDisposable,
                formattedTotalReceivedDisposable);
        }

        private readonly ReadOnlyObservableCollection<
            TransactionColumn> transactions;
        public ReadOnlyObservableCollection<
            TransactionColumn> Transactions => transactions;

        private readonly ReadOnlyObservableCollection<
            TransactionColumn> incomes;
        public ReadOnlyObservableCollection<
            TransactionColumn> Incomes => incomes;

        private readonly ReadOnlyObservableCollection<
            TransactionWithCategoryGrouping> groupedTransactions;
        public ReadOnlyObservableCollection<
            TransactionWithCategoryGrouping> GroupedTransactions => groupedTransactions;

        private TTimeframe timeframe;
        public TTimeframe Timeframe
        {
            get => timeframe;
            set => this.RaiseAndSetIfChanged(ref timeframe, value);
        }

        private string formattedTotalSpent;
        public string FormattedTotalSpent
        {
            get => formattedTotalSpent;
            set => this.RaiseAndSetIfChanged(ref formattedTotalSpent, value);
        }

        private string formattedTotalReceived;
        public string FormattedTotalReceived
        {
            get => formattedTotalReceived;
            set => this.RaiseAndSetIfChanged(ref formattedTotalReceived, value);
        }

        private string timeframeIndicator;
        public string TimeframeIndicator
        {
            get => timeframeIndicator;
            set => this.RaiseAndSetIfChanged(ref timeframeIndicator, value);
        }

        private string dcm;
        public string Dcm
        {
            get => dcm;
            set => this.RaiseAndSetIfChanged(ref dcm, value);
        }

        public ReactiveCommand<Unit, Unit> GoBackCommand { get; set; }
        public ReactiveCommand<Unit, Unit> GoNextCommand { get; set; }

        private void executeGoBackCommand()
        {
            Timeframe = new TTimeframe
            {
                From = Timeframe.From.AddDays(-7),
                FromSf = Timeframe.FromSf.AddDays(-7),
                To = Timeframe.To.AddDays(-7),
                View = Timeframe.View,
                Interval = Timeframe.Interval,
                IntervalType = Timeframe.IntervalType
            };
        }

        private void executeGoNextCommand()
        {
            Timeframe = new TTimeframe
            {
                From = Timeframe.From.AddDays(7),
                FromSf = Timeframe.FromSf.AddDays(7),
                To = Timeframe.To.AddDays(7),
                View = Timeframe.View,
                Interval = Timeframe.Interval,
                IntervalType = Timeframe.IntervalType
            };
        }

        private Func<TTransaction, bool> createFilterByTimeframe(
            TTimeframe timeframe)
        {
            return transaction =>
                transaction.CreatedAt >= timeframe.From &&
                transaction.CreatedAt <= timeframe.To;
        }

        private TTimeframe getCurrentWeekTimeframe()
        {
            DateTime startOfWeek = DateTime.Now.Date.AddDays(
              (int)CultureInfo.CurrentCulture.DateTimeFormat.FirstDayOfWeek -
              (int)DateTime.Today.DayOfWeek);
            DateTime endOfWeek = startOfWeek.AddDays(6).AddHours(23).AddMinutes(59).AddSeconds(59);
            return new TTimeframe
            {
                From = startOfWeek,
                FromSf = startOfWeek.AddHours(-12),
                To = endOfWeek,
                View = TimeframeViews.Week,
                Interval = 1,
                IntervalType = IntervalTypes.Days
            };
        }

        private readonly IDisposable _cleanUp;
        public void Dispose() { _cleanUp.Dispose(); }
    }
}
