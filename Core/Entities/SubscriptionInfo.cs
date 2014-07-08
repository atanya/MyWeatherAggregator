using System;

namespace WeatherAggregator.Core.Entities
{
    public class SubscriptionInfo
    {
        public string Email { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public bool IsConfirmed { get; set; }
        public string AddressText { get; set; }
        public string Key { get; set; } //when IsConfirmed = false, key used for confirm subscription, when IsConfirmed = true, key regenerated and used for confirm unsobscription.
        public DateTime CreatedDate { get; set; }
        public DateTime? UpdateDate { get; set; }
        public DateTime? LastNotifyDate { get; set; }
    }
}
