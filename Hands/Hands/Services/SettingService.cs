using System;
using System.Threading.Tasks;
using System.Reactive;
using System.Reactive.Linq;
using System.Collections.Generic;
using Akavache;
using Hands.Models;
using DynamicData;
using ReactiveUI;

namespace Hands.Services
{
    public class SettingService : ISettingsService
    {
        private readonly IBlobCache store = BlobCache.LocalMachine;

        private readonly SourceCache<TCategory, string> categoriesSetting =
            new SourceCache<TCategory, string>(x => x.Id);

        private readonly SourceCache<TAccount, string> accountsSetting =
            new SourceCache<TAccount, string>(x => x.Id);

        public SettingService()
        {
            this.GetCategoriesSettingObservable()
                .Do(items => categoriesSetting.AddOrUpdate(items))
                .Subscribe();

            this.GetAccountsSettingObservable()
                .Do(items => accountsSetting.AddOrUpdate(items))
                .Subscribe();

            this.accountsSetting
                .Connect()
                .ObserveOn(RxApp.TaskpoolScheduler)
                .Throttle(TimeSpan.FromMilliseconds(500))
                .Select(_ => this.accountsSetting.Items)
                .Do(async (accounts) => await store
                    .InsertObject<IEnumerable<TAccount>>(accountsStoreKey, accounts))
                .Subscribe();

            this.categoriesSetting
                .Connect()
                .ObserveOn(RxApp.TaskpoolScheduler)
                .Throttle(TimeSpan.FromMilliseconds(500))
                .Select(_ => this.categoriesSetting.Items)
                .Do(async (categories) => await store
                    .InsertObject<IEnumerable<TCategory>>(categoriesStoreKey, categories))
                .Subscribe();
        }

        public async Task ResetSettingAsync()
        {
            await store.InsertObject<string>(notificationStoreKey,
                ServiceConsts.defaultSettings.Notification);

            accountsSetting.Edit((setting) =>
            {
                setting.Clear();
                setting.AddOrUpdate(ServiceConsts.defaultSettings.Accounts);
            });

            categoriesSetting.Edit(setting =>
            {
                setting.Clear();
                setting.AddOrUpdate(ServiceConsts.defaultSettings.Categories);
            });
        }

        #region Notification Setting
        public IObservable<string> GetNotificationSettingObservable()
        {
            return store.GetOrCreateObject<string>(
                notificationStoreKey,
                () => ServiceConsts.defaultSettings.Notification);
        }

        public IObservable<string> ResetNotificationSettingObservable()
        {
            return this.UpdateNotificationSettingObservable(
                ServiceConsts.defaultSettings.Notification);
        }

        public IObservable<string> UpdateNotificationSettingObservable(string notificationSetting)
        {
            return store
                .InsertObject<string>(notificationStoreKey, notificationSetting)
                .Select(_ => notificationSetting);
        }
        #endregion

        #region Categories Setting
        public IObservable<IChangeSet<TCategory, string>> ConnectCategoriesSetting()
        {
            return categoriesSetting.Connect();
        }

        public IObservable<List<TCategory>> GetCategoriesSettingObservable()
        {
            return store.GetOrCreateObject<List<TCategory>>(
                categoriesStoreKey,
                () => ServiceConsts.defaultSettings.Categories);
        }
        #endregion

        #region Accounts Setting
        public IObservable<IChangeSet<TAccount, string>> ConnectAccountsSetting()
        {
            return accountsSetting.Connect();
        }

        public IObservable<List<TAccount>> GetAccountsSettingObservable()
        {
            return store.GetOrCreateObject<List<TAccount>>(
                accountsStoreKey,
                () => ServiceConsts.defaultSettings.Accounts);
        }

        public void AddNewAccount(string name)
            => accountsSetting
            .AddOrUpdate(new TAccount { Id = Guid.NewGuid().ToString(), Name = name });

        public void RemoveAccount(TAccount account)
            => accountsSetting.Remove(account);

        public void UpdateAccount(TAccount account)
            => accountsSetting.AddOrUpdate(account);
        #endregion

        private readonly string notificationStoreKey = "settings::notification";
        private readonly string accountsStoreKey = "settings::accounts";
        private readonly string categoriesStoreKey = "settings::categories";
    }
}
