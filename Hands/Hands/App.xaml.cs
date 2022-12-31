using Akavache;
using Xamarin.Forms;
using Hands.Services;

namespace Hands
{
    public partial class App : Application
    {
        public App()
        {
            Akavache.Registrations.Start("Hands");

            DependencyService.Register<ISettingsService, SettingService>();

            InitializeComponent();

            MainPage = new AppShell();
        }

        protected override void OnStart() { }

        protected override void OnSleep() { }

        protected override void OnResume() { }
    }
}
