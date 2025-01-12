using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using System;
using System.Linq;
using System.Collections.Generic;

namespace ApiRateLimiting
{
    public class CustomRateLimitService
    {
        private readonly IMemoryCache _cache;
        private readonly List<RateLimitRule> _rateLimitRules;

        public CustomRateLimitService(IMemoryCache memoryCache, IOptions<IpRateLimitOptions> options)
        {
            _cache = memoryCache;
            _rateLimitRules = options.Value.GeneralRules;
        }

        public bool IsRequestAllowed(string clientId, string endpoint)
        {
            // Find the rate limit rule for this endpoint
            var rule = _rateLimitRules.FirstOrDefault(r => r.Endpoint == "*" || r.Endpoint == endpoint);
            if (rule == null) return true; // No rate limit rule, so allow the request

            var cacheKey = $"RateLimit-{clientId}-{endpoint}";
            var requestInfo = _cache.Get<RequestInfoModel>(cacheKey) ?? new RequestInfoModel();

            // Remove requests older than the defined time period
            requestInfo.Requests.RemoveAll(r => r.Timestamp < DateTime.UtcNow - rule.PeriodTimeSpan);

            // If the user has exceeded the limit, deny access
            if (requestInfo.Requests.Count >= rule.Limit)
            {
                return false;
            }

            // Add the current request
            requestInfo.Requests.Add(new RequestDetails
            {
                Timestamp = DateTime.UtcNow
            });

            // Update the cache with the new request info
            _cache.Set(cacheKey, requestInfo, rule.PeriodTimeSpan);
            return true;
        }

        public int GetRemainingRequests(string clientId, string endpoint)
        {
            var rule = _rateLimitRules.FirstOrDefault(r => r.Endpoint == "*" || r.Endpoint == endpoint);
            if (rule == null) return int.MaxValue; // No rate limit for this endpoint

            var cacheKey = $"RateLimit-{clientId}-{endpoint}";
            var requestInfo = _cache.Get<RequestInfoModel>(cacheKey) ?? new RequestInfoModel();

            return rule.Limit - requestInfo.Requests.Count;
        }
    }

    public class RequestInfoModel
    {
        public List<RequestDetails> Requests { get; set; } = new List<RequestDetails>();
    }

    public class RequestDetails
    {
        public DateTime Timestamp { get; set; }
    }
}
