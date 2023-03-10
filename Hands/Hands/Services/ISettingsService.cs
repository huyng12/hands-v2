using System;
using System.Reactive;
using System.Threading.Tasks;
using System.Collections.Generic;
using Hands.Models;
using DynamicData;

namespace Hands.Services
{
    public interface ISettingsService
    {
        // Common
        Task ResetSettingAsync();

        // Notification Setting
        IObservable<string> GetNotificationSettingObservable();
        IObservable<string> ResetNotificationSettingObservable();
        IObservable<string> UpdateNotificationSettingObservable(
            string notificationSetting);

        // Categories Setting
        IObservable<IChangeSet<TCategory, string>> ConnectCategoriesSetting();
        // TODO: Rename to `GetCategoriesSettingFromStorageObservable`
        IObservable<List<TCategory>> GetCategoriesSettingObservable();
        void AddNewCategory(string name, string type);
        void RemoveCategory(TCategory category);
        void UpdateCategory(TCategory category);

        // Accounts Setting
        IObservable<IChangeSet<TAccount, string>> ConnectAccountsSetting();
        // TODO: Rename to `GetAccountsSettingFromStorageObservable`
        IObservable<List<TAccount>> GetAccountsSettingObservable();
        void AddNewAccount(string name);
        void RemoveAccount(TAccount account);
        void UpdateAccount(TAccount account);
    }
}
