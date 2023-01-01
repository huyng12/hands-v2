using System;
using System.Collections.Generic;

namespace Hands.Models
{
    public static class NotificationSetting
    {
        public static string None = "None";
        public static string EveryMorning = "Every Morning";
        public static string EveryEvening = "Every Evening";
    }

    public class TSettings
    {
        public string Notification { get; set; }
        public List<TAccount> Accounts { get; set; }
        public List<TCategory> Categories { get; set; }
    }
}
