using Splat;
using ReactiveUI;
using DynamicData;
using System;
using System.Reactive;
using System.Reactive.Linq;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Hands.Services;
using Hands.Models;

namespace Hands.ViewModels
{
    public class SettingsAccountsViewModel : ReactiveObject, IDisposable
    {
        private readonly ISettingsService service;

        public SettingsAccountsViewModel()
        {
            service = Locator.Current.GetService<ISettingsService>();

            disposable = service.ConnectAccountsSetting()
                .RefCount()
                .ObserveOn(RxApp.MainThreadScheduler)
                .Bind(out items)
                .DisposeMany()
                .Subscribe();

            AddCommand = ReactiveCommand.CreateFromTask(ExecuteAddCommand);
            EditCommand = ReactiveCommand.CreateFromTask<TAccount>(ExecuteEditCommand);
            DeleteCommand = ReactiveCommand.CreateFromTask<TAccount>(ExecuteRemoveCommand);
        }

        private readonly ReadOnlyObservableCollection<TAccount> items;
        public ReadOnlyObservableCollection<TAccount> Items => items;

        public ReactiveCommand<Unit, Unit> AddCommand { get; set; }

        public ReactiveCommand<TAccount, Unit> EditCommand { get; set; }

        public ReactiveCommand<TAccount, Unit> DeleteCommand { get; set; }

        private async Task ExecuteAddCommand()
        {
            var name = await App.Current.MainPage.DisplayPromptAsync(
                "New Account", "Please input a name to create a new account",
                "Create", "Cancel", "Name");
            if (String.IsNullOrEmpty(name)) return;
            service.AddNewAccount(name);
        }

        private async Task ExecuteRemoveCommand(TAccount account)
        {
            bool answer = await App.Current.MainPage.DisplayAlert(
                $"Delete \"{account.Name}\"?",
                "Transactions under this account will remain.",
                "Delete", "Cancel");
            if (!answer) return;
            service.RemoveAccount(account);
        }

        private async Task ExecuteEditCommand(TAccount account)
        {
            var name = await App.Current.MainPage.DisplayPromptAsync(
                $"Editing \"{account.Name}\"",
                "Please input a new name for this account",
                "Update", "Cancel", "", -1, null, account.Name);
            if (String.IsNullOrEmpty(name)) return;
            service.UpdateAccount(new TAccount { Id = account.Id, Name = name });
        }

        private readonly IDisposable disposable;
        public void Dispose() { disposable.Dispose(); }
    }
}
