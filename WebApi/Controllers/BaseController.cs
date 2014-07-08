using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Reflection;
using System.Web.Compilation;
using System.Web.Http;
using WeatherAggregator.Core;
using WeatherAggregator.Core.Entities;
using WeatherAggregator.Core.Interfaces;
using WeatherAggregator.DataAccess.Mongo;

namespace WeatherAggregator.WebApi.Controllers
{
    public abstract class BaseController : ApiController
    {
        protected readonly CoreFacade Facade = CreateCoreFacade();

        private static CoreFacade CreateCoreFacade()
        {
            var settings = new Settings
            {
                CacheRepositoryFactory = () => DataAccess.InMemory.CacheRepository.Instance,
                Sources = LoadSources(),
                FeedbackRepositoryFactory = CreateFeedbackRepository,
                SubscriptionRepositoryFactory = CreateSubscriptionRepository
            };

            return new CoreFacade(settings);
        }

        private static IFeedbackRepository CreateFeedbackRepository()
        {
            var connectionString = ConfigurationManager.AppSettings["MONGOLAB_URI"];
            var context = new MongoContext(connectionString);
            return new FeedbackRepository(context);
        }

        private static ISubscriptionRepository CreateSubscriptionRepository()
        {
            var connectionString = ConfigurationManager.AppSettings["MONGOLAB_URI"];
            var context = new MongoContext(connectionString);
            return new SubscriptionRepository(context);
        }

        private static List<ISource> LoadSources()
        {
            var iSource = typeof(ISource);
            var sources = BuildManager.GetReferencedAssemblies().Cast<Assembly>()
                .SelectMany(s => s.GetTypes())
                .Where(iSource.IsAssignableFrom)
                .Where(t => !t.IsInterface)
                .Select(t => (ISource)Activator.CreateInstance(t))
                .ToList();
            return sources;
        }
    }
}