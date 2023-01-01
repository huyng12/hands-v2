using Splat;
using ReactiveUI;
using DynamicData;
using System;
using System.Reactive;
using System.Reactive.Linq;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Hands.Services;
using Hands.Models;

namespace Hands.ViewModels
{
    public class SettingsCategoriesViewModel : ReactiveObject
    {
        private readonly ISettingsService service;

        public SettingsCategoriesViewModel()
        {
            service = Locator.Current.GetService<ISettingsService>();

            service.ConnectCategoriesSetting()
                .ObserveOn(RxApp.MainThreadScheduler)
                .Bind(out items)
                .Subscribe();
        }

        private readonly ReadOnlyObservableCollection<TCategory> items;
        public ReadOnlyObservableCollection<TCategory> Items => items;
    }
}
