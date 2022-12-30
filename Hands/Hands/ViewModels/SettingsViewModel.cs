using ReactiveUI;

namespace Hands.ViewModels
{
    public class SettingsViewModel : ReactiveObject
    {
        private string text;
        public string Text
        {
            get => text;
            set => this.RaiseAndSetIfChanged(ref text, value);
        }

        public SettingsViewModel()
        {
        }
    }
}
