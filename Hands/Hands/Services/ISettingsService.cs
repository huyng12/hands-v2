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
        // Notification Setting
        IObservable<NotificationSetting> GetNotificationSettingObservable();
        IObservable<NotificationSetting> ResetNotificationSettingObservable();
        IObservable<NotificationSetting> UpdateNotificationSettingObservable(
            NotificationSetting notificationSetting);

        // Categories Setting
        IObservable<IChangeSet<TCategory, string>> ConnectCategoriesSetting();
        IObservable<List<TCategory>> GetCategoriesSettingObservable();
        //void AddNewCategory(string name, CategoryType type);
        //void RemoveCategory(TCategory category);
        //void UpdateCategory(TCategory category);

        // Accounts Setting
        IObservable<IChangeSet<TAccount, string>> ConnectAccountsSetting();
        IObservable<List<TAccount>> GetAccountsSettingObservable();
        void AddNewAccount(string name);
        void RemoveAccount(TAccount account);
        void UpdateAccount(TAccount account);
    }
}
