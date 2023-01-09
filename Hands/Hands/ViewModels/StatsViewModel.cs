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

    public class StatsViewModel : ReactiveObject, IDisposable
    {
        private readonly ITransactionService transactionService;

        public StatsViewModel()
        {
            transactionService = Locator.Current.GetService<ITransactionService>();

            Timeframe = getCurrentWeekTimeframe();

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
                    //Console.WriteLine($"{timeframe.From.ToString()} - {timeframe.To.ToString()}");
                    TimeframeIndicator = $"{from} - {to}";
                });

            var transactionsInTimeframe = transactionService
                .Connect()
                .RefCount()
                .ObserveOn(RxApp.MainThreadScheduler)
                .Filter(filterByTimeframe);

            var transactionsDisposable = transactionsInTimeframe
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

            GoBackCommand = ReactiveCommand.Create(executeGoBackCommand);
            GoNextCommand = ReactiveCommand.Create(executeGoNextCommand);

            _cleanUp = new CompositeDisposable(transactionsDisposable,
                incomesDisposable, timeframeIndicatorDisposable);
        }

        private readonly ReadOnlyObservableCollection<
            TransactionColumn> transactions;
        public ReadOnlyObservableCollection<
            TransactionColumn> Transactions => transactions;

        private readonly ReadOnlyObservableCollection<
            TransactionColumn> incomes;
        public ReadOnlyObservableCollection<
            TransactionColumn> Incomes => incomes;

        private TTimeframe timeframe;
        public TTimeframe Timeframe
        {
            get => timeframe;
            set => this.RaiseAndSetIfChanged(ref timeframe, value);
        }

        //readonly ObservableAsPropertyHelper<string> timeframeIndicator;
        //public string TimeframeIndicator => timeframeIndicator.Value;

        private string timeframeIndicator;
        public string TimeframeIndicator
        {
            get => timeframeIndicator;
            set => this.RaiseAndSetIfChanged(ref timeframeIndicator, value);
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

        //private IEnumerable<DateTime> getCurrentWeekDays()
        //{
        //    DateTime startOfWeek = DateTime.Today.AddDays(
        //      (int)CultureInfo.CurrentCulture.DateTimeFormat.FirstDayOfWeek -
        //      (int)DateTime.Today.DayOfWeek);
        //    return Enumerable
        //        .Range(0, 7)
        //        .Select(i => startOfWeek.AddDays(i));
        //}

        private readonly IDisposable _cleanUp;
        public void Dispose() { _cleanUp.Dispose(); }
    }
}
