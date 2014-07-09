using System;
using System.Linq;
using System.Reflection;
using Newtonsoft.Json;
using PostSharp.Aspects;
using PostSharp.Extensibility;
using WeatherAggregator.Core.Interfaces;

namespace WeatherAggregator.Core.Aspects
{
    [Serializable]
    [MulticastAttributeUsage(MulticastTargets.Method, AllowMultiple = false)]
    public abstract class MethodCacheAttribute : BaseMethodInterceptor
    {
        internal static Func<ICacheRepository> CacheRepositoryFactory;

        private string _cacheKey;

        protected abstract int Timeout { get; }

        protected MethodCacheAttribute(Type target = null)
            : base(target) { }

        public override void OnEntry(MethodExecutionArgs args)
        {
            var cache = CacheRepositoryFactory();
            _cacheKey = BuildCacheKey(args.Method, args.Arguments);
            var cacheEntry = cache[_cacheKey] as CacheEntry;
            if (cacheEntry == null || (DateTime.Now - cacheEntry.DateCreated).TotalSeconds > Timeout)
            {
                Logger.Info("Key missed in cache: {0}", _cacheKey);
            }
            else
            {
                args.FlowBehavior = FlowBehavior.Return;

                Logger.Info("Key found in cache: {0}", _cacheKey);
                args.ReturnValue = cacheEntry.Value;
            }
        }

        public override void OnSuccess(MethodExecutionArgs args)
        {
            var cache = CacheRepositoryFactory();

            cache[_cacheKey] = new CacheEntry
            {
                Value = args.ReturnValue,
                DateCreated = DateTime.Now
            };
        }

        private string BuildCacheKey(MethodBase method, Arguments arguments)
        {
            return JsonConvert.SerializeObject(new
            {
                Class = method.DeclaringType.FullName,
                Method = method.Name,
                Arguments = arguments
            });
        }

        class CacheEntry
        {
            public object Value;
            public DateTime DateCreated;
        }
    }
}
