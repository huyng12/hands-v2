using System.Reactive.Disposables;
using ReactiveUI;
using ReactiveUI.XamForms;
using Hands.ViewModels;

namespace Hands.Views
{
    public partial class SettingsPage : ReactiveContentPage<SettingsViewModel>
    {
        public SettingsPage()
        {
            InitializeComponent();

            ViewModel = new SettingsViewModel();

            this.WhenActivated(disposable =>
            {
                this.Bind(ViewModel, x => x.Text, x => x.TheEntry.Text)
                    .DisposeWith(disposable);
                this.OneWayBind(ViewModel, vm => vm.Text, x => x.TheLabel.Text)
                    .DisposeWith(disposable);
            });
        }
    }
}
