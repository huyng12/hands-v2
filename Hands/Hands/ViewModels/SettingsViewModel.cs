using ReactiveUI;
using Xamarin.Forms;
using System.Reactive;
using System.Reactive.Linq;
using System.Threading.Tasks;
using Hands.Services;
using Hands.Models;
using System;
using Newtonsoft.Json;

namespace Hands.ViewModels
{
    public class SettingsViewModel : ReactiveObject
    {
        private readonly ISettingsService service;

        ObservableAsPropertyHelper<TSettings> settings;
        public TSettings Settings => settings.Value;

        public ReactiveCommand<Unit, Unit> ResetSettingsCommand { get; set; }

        public SettingsViewModel()
        {
            service = DependencyService.Get<ISettingsService>();
            SetupObservables();
        }

        private void SetupObservables()
        {
            settings = service.GetSettingsObservable()
                .ToProperty(this, vm => vm.Settings);

            ResetSettingsCommand = ReactiveCommand
                .Create(ExecuteResetSettingsCommand);
        }

        private void ExecuteResetSettingsCommand()
        {
            service.ResetSettingsObservable()
                .ToProperty(this, vm => vm.Settings, out settings);
        }
    }
}
