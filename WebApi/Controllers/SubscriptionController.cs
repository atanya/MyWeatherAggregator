using System;
using System.Collections.Generic;
using System.Web.Http;
using WeatherAggregator.Core.Entities;

namespace WeatherAggregator.WebApi.Controllers
{
    public class SubscriptionController : BaseController
    {
        [HttpPost]
        public void Subscribe(SubscriptionInfo subscriber)
        {
            Facade.Subscribe(subscriber);
        }

        [HttpGet]
        public void ConfirmSubscription(string key)
        {
            Facade.ConfirmSubscription(key);
        }


        [HttpGet]
        public void Unsubscribe(string key)
        {
            Facade.Unsubscribe( key);
        }

        [HttpGet]
        public void NotifySubscribers()
        {
            Facade.NotifySubscribers();
        }

    }
}
