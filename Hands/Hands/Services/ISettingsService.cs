using System;
using System.Reactive;
using System.Threading.Tasks;
using Hands.Models;

namespace Hands.Services
{
    public interface ISettingsService
    {
        IObservable<TSettings> ResetSettingsObservable();
        IObservable<TSettings> GetSettingsObservable();
        IObservable<TSettings> UpdateSettingsObservable(TSettings settings);
    }
}
