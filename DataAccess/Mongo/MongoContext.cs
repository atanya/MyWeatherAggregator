using System;
using MongoDB.Bson.Serialization;
using MongoDB.Driver;
using WeatherAggregator.Core.Entities;

namespace WeatherAggregator.DataAccess.Mongo
{
    public class MongoContext
    {
        public MongoCollection<Feedback> Feddback { get; private set; }

        public MongoCollection<SubscriptionInfo> Subscription { get; private set; }

        static MongoContext()
        {
            BsonClassMap.RegisterClassMap<Feedback>(cm =>
            {
                cm.AutoMap();
                cm.SetIdMember(cm.GetMemberMap(f => f.DateCreated));
            });
            BsonClassMap.RegisterClassMap<SubscriptionInfo>(cm =>
            {
                cm.AutoMap();
                cm.SetIdMember(cm.GetMemberMap(f => f.CreatedDate));
            });
        }

        public MongoContext(string connectionString)
        {
            var url = new MongoUrl(connectionString);
            var client = new MongoClient(url);
            var server = client.GetServer();
            var database = server.GetDatabase(url.DatabaseName);

            Feddback = database.GetCollection<Feedback>("Feedback");
            Subscription = database.GetCollection<SubscriptionInfo>("Subscription");
        }
    }
}
