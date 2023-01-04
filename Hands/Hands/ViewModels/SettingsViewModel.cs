using ReactiveUI;
using Xamarin.Forms;
using System;
using System.Reactive;
using System.Reactive.Linq;
using System.Reactive.Concurrency;
using System.Threading.Tasks;
using Hands.Services;
using Hands.Models;
using Hands.Views;
using Splat;

namespace Hands.ViewModels
{
    public class SettingsViewModel : ReactiveObject
    {
        readonly INavigation navigationService;

        private readonly ISettingsService service;

        private readonly ITransactionService transactionService;

        public SettingsViewModel(INavigation navigation)
        {
            navigationService = navigation;

            service = Locator.Current.GetService<ISettingsService>();

            transactionService = Locator.Current.GetService<ITransactionService>();

            notificationSetting = service.GetNotificationSettingObservable()
                .ToProperty(this, nameof(NotificationSetting));

            FetchNotificationSettingCommand = ReactiveCommand.Create(ExecuteFetchNotificationSettingCommand);
            ResetNotificationSettingCommand = ReactiveCommand.CreateFromTask(ExecuteResetNotificationSettingCommand);
            GoToCategoriesSettingCommand = ReactiveCommand.Create(ExecuteGoToCategoriesSettingCommand);
            GoToAccountsSettingCommand = ReactiveCommand.Create(ExecuteGoToAccountsSettingCommand);
        }

        public ReactiveCommand<Unit, Unit> FetchNotificationSettingCommand { get; set; }

        public ReactiveCommand<Unit, Unit> ResetNotificationSettingCommand { get; set; }

        public ReactiveCommand<Unit, Unit> GoToCategoriesSettingCommand { get; set; }

        public ReactiveCommand<Unit, Unit> GoToAccountsSettingCommand { get; set; }

        ObservableAsPropertyHelper<string> notificationSetting;
        public string NotificationSetting
            => notificationSetting.Value;

        private void ExecuteFetchNotificationSettingCommand()
        {
            notificationSetting = service.GetNotificationSettingObservable()
                .ToProperty(this, nameof(NotificationSetting));
        }

        private async Task ExecuteResetNotificationSettingCommand()
        {
            await service.ResetSettingAsync();
            //transactionService.Reset();
            notificationSetting = service.GetNotificationSettingObservable()
                .ToProperty(this, nameof(NotificationSetting));
        }

        private async void ExecuteGoToCategoriesSettingCommand()
            => await Shell.Current.Navigation.PushAsync(new SettingsCategoriesPage());

        private async void ExecuteGoToAccountsSettingCommand()
            => await Shell.Current.Navigation.PushAsync(new SettingsAccountsPage());
    }
}
