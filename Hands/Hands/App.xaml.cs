using Akavache;
using ReactiveUI;
using Splat;
using Xamarin.Forms;
using Hands.Services;
using Hands.Views;
using Hands.ViewModels;

namespace Hands
{
    public partial class App : Application
    {
        public App()
        {
            Akavache.Registrations.Start("Hands");

            InitializeComponent();

            Locator
                .CurrentMutable
                .RegisterConstant<ISettingsService>(new SettingService());

            MainPage = new AppShell();
        }

        protected override void OnStart() { }

        protected override void OnSleep() { }

        protected override void OnResume() { }
    }
}
