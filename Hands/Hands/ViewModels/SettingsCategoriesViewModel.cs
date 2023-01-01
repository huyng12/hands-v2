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


using Xamarin.Forms;
using Xamarin.Essentials;
using System.Web;


namespace Hands.ViewModels
{
    public class SettingsAccountsViewModel : ReactiveObject
    {
        private readonly ISettingsService service;

        public SettingsAccountsViewModel()
        {
            service = Locator.Current.GetService<ISettingsService>();

            service.ConnectAccountsSetting()
                .ObserveOn(RxApp.MainThreadScheduler)
                .Bind(out items)
                .Subscribe();
        }

        private readonly ReadOnlyObservableCollection<TAccount> items;
        public ReadOnlyObservableCollection<TAccount> Items => items;
    }
}
