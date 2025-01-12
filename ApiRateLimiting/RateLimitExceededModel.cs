using System;

namespace ApiRateLimiting
{
    public class RateLimitExceededModel
    {
        public string Message { get; set; }
        public int StatusCode { get; set; }
        public int RemainingRequests { get; set; }
        public DateTime ResetTime { get; set; }

        public RateLimitExceededModel(string message, int statusCode, int remainingRequests, DateTime resetTime)
        {
            Message = message;
            StatusCode = statusCode;
            RemainingRequests = remainingRequests;
            ResetTime = resetTime;
        }
    }
}
