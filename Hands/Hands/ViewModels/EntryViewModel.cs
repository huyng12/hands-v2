using System;
using ReactiveUI;

namespace Hands.ViewModels
{
    public class EntryViewModel : ReactiveObject, IDisposable
    {
        public EntryViewModel()
        {
        }

        private readonly IDisposable disposable;
        public void Dispose() { disposable.Dispose(); }
    }
}
