using System;
using System.Collections.Generic;
using System.Linq;
using WeatherAggregator.Core.Aspects;
using WeatherAggregator.Core.Entities;
using WeatherAggregator.Core.Interfaces;
using WeatherAggregator.Core.Logic;

namespace WeatherAggregator.Core
{
    public class CoreFacade
    {
        private readonly List<ISource> _sources;
        private readonly Func<IFeedbackRepository> _feedbackRepositoryFactory;
        private readonly Func<ISubscriptionRepository> _subscriptionRepositoryFactory;

        public CoreFacade(Settings settings)
        {
            _sources = settings.Sources;
            MethodCacheAttribute.CacheRepositoryFactory = settings.CacheRepositoryFactory;
            _feedbackRepositoryFactory = settings.FeedbackRepositoryFactory;
            _subscriptionRepositoryFactory = settings.SubscriptionRepositoryFactory;
        }

        public List<Weather> GetWeather(List<Guid> sources, DateRange dateRange, Location location)
        {
            var provider = new WeatherProvider(_sources.Where(s => sources.Contains(s.Id)).ToList());
            return provider.GetWeather(dateRange, location);
        }

        public IEnumerable<ISource> GetSources()
        {
            return _sources;
        }

        public void AddFeedback(Feedback feedback)
        {
            var provider = new FeedbackProvider(_feedbackRepositoryFactory());
            provider.Add(feedback);
        }

        public void Subscribe(SubscriptionInfo subscription)
        {
            var provider = new SubscriptionProvider(_subscriptionRepositoryFactory());
            bool isUpdatingSubscription = provider.Add(subscription);
            EmailSender.SendEmail(subscription.Email, isUpdatingSubscription ? "Subscription Updated" : "Subscription Adde");
        }

        public void ConfirmSubscription(string key)
        {
            var provider = new SubscriptionProvider(_subscriptionRepositoryFactory());
            string email = provider.Confirm(key);
            EmailSender.SendEmail(email, "You subscription confirmed");
        }

        public void Unsubscribe(string key)
        {
            var provider = new SubscriptionProvider(_subscriptionRepositoryFactory());
            string email  = provider.Unsubscribe(key);
            EmailSender.SendEmail(email, "Good luck!");
        }

        public void NotifySubscribers()
        {
            var provider = new SubscriptionProvider(_subscriptionRepositoryFactory());
            
            provider.DeleteExpiredSubscriptions();
            
            List<SubscriptionInfo> subscriptions = provider.GetConfirmed();

            DateRange dateRange = new DateRange {From = DateTime.Now, To = DateTime.Now.AddDays(3)};

            var weatherProvider = new WeatherProvider(_sources);
            foreach (SubscriptionInfo subscription in subscriptions)
            {
                List<DateTime> rainDates = new List<DateTime>();
                Location coords = new Location {Latitude = subscription.Latitude, Longitude = subscription.Longitude};
                List<Weather> weatherForecast = weatherProvider.GetWeather(dateRange, coords);
                foreach (Weather weather in weatherForecast)
                {
                    if (weather.Precipitation == Precipitation.Rain)
                    {
                        rainDates.Add(weather.Date);
                    }
                }
                if (rainDates.Count > 0)
                {
                    string rainDatesMessage = string.Join(", ", rainDates);
                    EmailSender.SendEmail(subscription.Email, rainDatesMessage);
                    provider.UpdateNotifyDate(subscription);
                }
            }
        }
    }
}
