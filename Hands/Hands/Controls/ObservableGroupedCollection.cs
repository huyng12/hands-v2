﻿using System;
using DynamicData;
using DynamicData.Binding;
using ReactiveUI;
using System.Collections.Generic;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Linq;

namespace Hands.Controls
{
    public class ObservableGroupedCollection<TGroupKey, TObject, TKey,
        TAggregateValue, TFormattedAggregateValue>
        : ObservableCollectionExtended<TObject>, IDisposable
    {
        public TGroupKey Key { get; }
        public TAggregateValue AggregateValue { get; set; }
        public TFormattedAggregateValue FormattedAggregateValue { get; set; }

        public ObservableGroupedCollection(
            IGroup<TObject, TKey, TGroupKey> group,
            SortExpressionComparer<TObject> comparer,
            Func<IQuery<TObject, TKey>, TAggregateValue> aggregateQuery,
            Func<TAggregateValue, TFormattedAggregateValue> aggregateFormatter)
        {
            this.Key = group.Key;

            var source = group.Cache.Connect();

            var aggregatedLoader = source
                .QueryWhenChanged(aggregateQuery)
                .Subscribe(value =>
                {
                    AggregateValue = value;
                    FormattedAggregateValue = aggregateFormatter(value);
                });

            var dataLoader = source
                .Sort(comparer)
                .ObserveOn(RxApp.MainThreadScheduler)
                .Bind(this)
                .Subscribe();

            _cleanUp = new CompositeDisposable(dataLoader, aggregatedLoader);
        }

        private readonly IDisposable _cleanUp;
        public void Dispose() => _cleanUp.Dispose();
    }
}
