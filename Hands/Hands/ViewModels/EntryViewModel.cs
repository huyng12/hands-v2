using System;
using System.Reactive;
using System.Threading.Tasks;
using ReactiveUI;
using Xamarin.Forms;
using Hands.Views;

namespace Hands.ViewModels
{
    public class EntryViewModel : ReactiveObject, IDisposable
    {
        public EntryViewModel()
        {
            AddCommand = ReactiveCommand.CreateFromTask(ExecuteAddCommand);
        }

        public ReactiveCommand<Unit, Unit> AddCommand { get; set; }

        private async Task ExecuteAddCommand()
            => await Shell.Current.Navigation.PushAsync(
                new EntryDetailPage());
        //=> await Shell.Current.Navigation.PushModalAsync(
        //    new NavigationPage(new EntryDetailPage()));

        private readonly IDisposable disposable;
        public void Dispose() { disposable.Dispose(); }
    }
}
