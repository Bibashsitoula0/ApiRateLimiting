using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Threading.Tasks;

namespace ApiRateLimiting
{
    public class RateLimitingMiddleware
    {
        private readonly RequestDelegate _next;
        private static readonly ConcurrentDictionary<string, RequestInfo> _requestCounts = new();
        private readonly int _requestLimit = 100;
        private readonly TimeSpan _timeSpan = TimeSpan.FromMinutes(1);

        public RateLimitingMiddleware(RequestDelegate next, int requestLimit = 100, TimeSpan? timeSpan = null)
        {
            _next = next;
            _requestLimit = requestLimit;
            _timeSpan = timeSpan ?? TimeSpan.FromMinutes(1);
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var userIp = context.Connection.RemoteIpAddress.ToString();
            var userIdentifier = context.User.Identity.Name;

            var effectiveIdentifier = string.IsNullOrEmpty(userIdentifier) ? userIp : userIdentifier;

            if (IsRequestLimitExceeded(effectiveIdentifier))
            {
                context.Response.StatusCode = StatusCodes.Status429TooManyRequests;
                await context.Response.WriteAsync("Too many requests. Please try again later.");
                return;
            }
            await _next(context);
        }

        private bool IsRequestLimitExceeded(string effectiveIdentifier)
        {
            var now = DateTime.UtcNow;

            // Cleanup old requests older than the specified time span
            var expiredRequests = _requestCounts.Where(r => r.Value.Timestamp < now - _timeSpan).ToList();
            foreach (var expiredRequest in expiredRequests)
            {
                _requestCounts.TryRemove(expiredRequest.Key, out _);
            }

            // Count the number of requests within the time span
            var userRequests = _requestCounts.Values.Where(r => r.Identifier == effectiveIdentifier && r.Timestamp >= now.AddMinutes(-1)).Count();

            // If the user has exceeded the defined request limit, deny access
            return userRequests >= _requestLimit;
        }

        public static void TrackRequest(string userIdentifier)
        {
            var now = DateTime.UtcNow;

            if (_requestCounts.ContainsKey(userIdentifier))
            {
                _requestCounts[userIdentifier] = new RequestInfo
                {
                    Identifier = userIdentifier,
                    Timestamp = now
                };
            }
            else
            {
                _requestCounts.TryAdd(userIdentifier, new RequestInfo
                {
                    Identifier = userIdentifier,
                    Timestamp = now
                });
            }
        }
    }

    public class RequestInfo
    {
        public string Identifier { get; set; }
        public DateTime Timestamp { get; set; }
    }
}
