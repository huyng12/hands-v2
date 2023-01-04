using Akavache;
using ReactiveUI;
using Splat;
using Xamarin.Forms;
using Hands.Services;
using Hands.Views;
using Hands.ViewModels;
using System;

namespace Hands
{
    public partial class App : Application
    {
        public App()
        {
            BlobCache.ForcedDateTimeKind = DateTimeKind.Local;
            Akavache.Registrations.Start("Hands");

            InitializeComponent();

            Locator.CurrentMutable.RegisterConstant<ISettingsService>(new SettingService());
            Locator.CurrentMutable.RegisterConstant<ITransactionService>(new TransactionService());

            MainPage = new AppShell();
        }

        protected override void OnStart() { }

        protected override void OnSleep() { }

        protected override void OnResume() { }
    }
}
