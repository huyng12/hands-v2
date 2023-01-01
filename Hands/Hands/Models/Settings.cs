using System;
using System.Collections.Generic;

namespace Hands.Models
{
    public enum NotificationSetting { None, EveryMorning, EveryEvening }

    public class TSettings
    {
        public NotificationSetting Notification { get; set; }
        public List<TAccount> Accounts { get; set; }
        public List<TCategory> Categories { get; set; }
    }
}
