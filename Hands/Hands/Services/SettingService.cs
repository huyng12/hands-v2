using System;
using System.Threading.Tasks;
using System.Reactive;
using System.Reactive.Linq;
using System.Collections.Generic;
using Akavache;
using Hands.Models;
using Newtonsoft.Json;
using ReactiveUI;

namespace Hands.Services
{
    public class SettingService : ISettingsService
    {
        private readonly string storeKey = "settings";

        public ObservableAsPropertyHelper<TSettings> SettingsObservable;

        public IObservable<TSettings> ResetSettingsObservable()
        {
            return this.UpdateSettingsObservable(ServiceConsts.defaultSettings);
        }

        public IObservable<TSettings> GetSettingsObservable()
        {
            return BlobCache.LocalMachine.GetOrCreateObject<TSettings>(
                storeKey,
                () => ServiceConsts.defaultSettings);
        }

        public IObservable<TSettings> UpdateSettingsObservable(TSettings settings)
        {
            return BlobCache.LocalMachine
                .InsertObject<TSettings>(storeKey, settings)
                .Select(_ => settings);
        }
    }
}
