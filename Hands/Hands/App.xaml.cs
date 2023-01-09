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
            Syncfusion.Licensing.SyncfusionLicenseProvider.RegisterLicense("Mgo+DSMBaFt/QHRqVVhjVFpGaVldX2NLfUNwT2ZQdV5xZCQ7a15RRnVfQ11kSH9WcUZjX3hbdg==;Mgo+DSMBPh8sVXJ0S0J+XE9HflRAQmVWfFN0RnNfdVt5flVHcC0sT3RfQF5jSH5Ud0RmXnxedn1VQA==;ORg4AjUWIQA/Gnt2VVhkQlFadVdJX3xPYVF2R2BJeVR1fV9GYUwgOX1dQl9gSX9TcURiXHhdcHNVQ2Y=;ODk0NzYxQDMyMzAyZTM0MmUzMEpVTi9IQVJKYmg1ZDJseTVpQ0hrOUFrMEFINUR4N2IxN0MwcXptdFBST1k9;ODk0NzYyQDMyMzAyZTM0MmUzMGJLMFZqRTk2eDFmRG9rYjVmekEzaWNrRCt4NHhBQkZ1dUwwWCs4R0MrOTQ9;NRAiBiAaIQQuGjN/V0Z+WE9EaFxKVmFWeEx0RWFab1h6cVxMYF1BNQtUQF1hSn5SdkJiWHpZc3RSQWda;ODk0NzY0QDMyMzAyZTM0MmUzMGcrRWZ6d0ZuekZiSmVKcnUreUtUYVRMcEdzR2RYUlcrbkozaCtzTkgxNzA9;ODk0NzY1QDMyMzAyZTM0MmUzMGcwak8xZ2Z4aEdVTytKNzg1bTFkMkN2c04zYTFKMTlHVUhMc2E1Und2MTQ9;Mgo+DSMBMAY9C3t2VVhkQlFadVdJX3xPYVF2R2BJeVR1fV9GYUwgOX1dQl9gSX9TcURiXHhdcHxQRmY=;ODk0NzY3QDMyMzAyZTM0MmUzMFRjd3RtVTI5ZmRKZ3VTL29XS1NXdkFSUDdVTmYzRGZ3U09tbUdjRkVZcWM9;ODk0NzY4QDMyMzAyZTM0MmUzMFBuSWZYTG1RTytDMWNJaXQ4TmZiZnRFZlkwam1ISDI4OEJWSkZkbTdOeWc9;ODk0NzY5QDMyMzAyZTM0MmUzMGcrRWZ6d0ZuekZiSmVKcnUreUtUYVRMcEdzR2RYUlcrbkozaCtzTkgxNzA9");

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
