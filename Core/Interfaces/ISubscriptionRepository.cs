using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WeatherAggregator.Core.Entities;

namespace WeatherAggregator.Core.Interfaces
{
    public interface ISubscriptionRepository
    {
        bool IsExist(string email);
        void Add(SubscriptionInfo subscription);
        string Confirm(string key);
        string Unsobscribe(string key);
        List<SubscriptionInfo> GetConfirmed();
        void UpdateNotifyDate(SubscriptionInfo subscription);
        void DeleteExpiredSubscriptions();
    }
}
