using System;
using System.Collections.Generic;

namespace Hands.Models
{
    public enum NotificationSetting { None, EveryMorning, EveryEvening }

    public class TSettings
    {
        public NotificationSetting Notification { get; set; }
        public List<Account> Accounts { get; set; }
        public List<Category> Categories { get; set; }
    }
}
