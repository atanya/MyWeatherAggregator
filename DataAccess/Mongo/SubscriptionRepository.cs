using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MongoDB.Driver.Linq;
using WeatherAggregator.Core.Entities;
using WeatherAggregator.Core.Interfaces;

namespace WeatherAggregator.DataAccess.Mongo
{
    public class SubscriptionRepository : ISubscriptionRepository
    {
        private readonly MongoContext _mongo;

        public SubscriptionRepository(MongoContext context)
        {
            _mongo = context;
        }

        public bool IsExist(string email)
        {
            return _mongo.Subscription.AsQueryable().Any(s => s.Email == email && s.IsConfirmed);
        }

        public void Add(SubscriptionInfo subscription)
        {
            subscription.CreatedDate = DateTime.Now;
            subscription.IsConfirmed = false;
            subscription.Key = Guid.NewGuid().ToString();

            _mongo.Subscription.Insert(subscription);
        }

        public string Confirm(string key)
        {
            SubscriptionInfo notConfirmedSubscription =
                _mongo.Subscription.AsQueryable().FirstOrDefault(s => s.Key == key && s.IsConfirmed == false);
            if (notConfirmedSubscription == null)
                throw new Exception(string.Format("can' find subscription with key {0}", key));

            SubscriptionInfo existConfirmedSubscription =
                _mongo.Subscription.AsQueryable()
                    .FirstOrDefault(s => s.Email == notConfirmedSubscription.Email && s.IsConfirmed);

            if (existConfirmedSubscription != null)
            {
                //_mongo.Subscription.Remove(existConfirmedSubscription);
            }

            notConfirmedSubscription.IsConfirmed = true;
            notConfirmedSubscription.UpdateDate = DateTime.Now;
            //_mongo.Subscription.Update(notConfirmedSubscription);

            return notConfirmedSubscription.Email;
        }

        public string Unsobscribe(string key)
        {
            SubscriptionInfo subscription = _mongo.Subscription.AsQueryable().FirstOrDefault(s => s.Key == key);
            if (subscription == null)
                throw new Exception(string.Format("can' find subscription with key {0}", key));

            //_mongo.Subscription.Remove(subscription);
            return subscription.Email;
        }

        public List<SubscriptionInfo> GetConfirmed()
        {
            return _mongo.Subscription.AsQueryable().Where(s => s.IsConfirmed && (s.LastNotifyDate - DateTime.Now) > new TimeSpan(0, 6, 0)).ToList();
        }

        public void UpdateNotifyDate(SubscriptionInfo subscription)
        {
            SubscriptionInfo updatingSubscription =
                _mongo.Subscription.AsQueryable()
                    .FirstOrDefault(s => s.CreatedDate == subscription.CreatedDate && s.IsConfirmed);
            if (updatingSubscription == null)
                throw new Exception(string.Format("can' find subscription for {0} for updating date on notification",
                    subscription.Email));

            updatingSubscription.LastNotifyDate = DateTime.Now;
            //_mongo.Subscription.Update(updatingSubscription);

        }

        public void DeleteExpiredSubscriptions()
        {
            List<SubscriptionInfo> expiredSubscriptions = _mongo.Subscription.AsQueryable().Where(s => !s.IsConfirmed).ToList();

            foreach (SubscriptionInfo expiredSubscription in expiredSubscriptions)
            {
                if ((DateTime.Now - expiredSubscription.CreatedDate) > new TimeSpan(30, 0, 0))
                {
                    //_mongo.Subscription.Remove(subscription);
                }
            }
        }
    }
}
