using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Intrinsics.X86;
using System.Text;
using System.Threading.Tasks;

namespace ApiRateLimiting
{
    public class IpRateLimitOptions
    {
        public List<RateLimitRule> GeneralRules { get; set; }
    }

      public class RateLimitRule
    {
        public string Endpoint { get; set; }
        public string Period { get; set; }  // e.g., "1h", "30m"
        public int Limit { get; set; }

        public TimeSpan PeriodTimeSpan => ParsePeriod(Period);

        private TimeSpan ParsePeriod(string period)
        {
            if (string.IsNullOrEmpty(period))
                throw new ArgumentException("Period cannot be empty.");

            int value = int.Parse(period.Substring(0, period.Length - 1));
            char unit = period.Last();

            return unit switch
            {
                'h' => TimeSpan.FromHours(value),
                'm' => TimeSpan.FromMinutes(value),
                'd' => TimeSpan.FromDays(value),
                _ => throw new ArgumentException($"Invalid period unit: {unit}")
            };
        }
    }
}
