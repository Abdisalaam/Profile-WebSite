using System;
using System.Collections.Concurrent;
namespace LeadersSolutionWebSite.Models
{
    public static class RateLimiter
    {
        private static readonly ConcurrentDictionary<string, DateTime> RequestTimesLS_Profile =
            new ConcurrentDictionary<string, DateTime>();
        private static readonly ConcurrentDictionary<string, DateTime> RequestTimesContactUs =
           new ConcurrentDictionary<string, DateTime>();
        private static readonly ConcurrentDictionary<string, DateTime> RequestTimesNewsLetterSubscrip =
           new ConcurrentDictionary<string, DateTime>();
        public static bool IsLimitedLSProfile(string key, int seconds)
        {
            if (RequestTimesLS_Profile.TryGetValue(key, out DateTime lastRequest))
            {
                if ((DateTime.Now - lastRequest).TotalSeconds < seconds)
                {
                    return true; // Too fast → limit
                }
            }

            // Update last request time
            RequestTimesLS_Profile[key] = DateTime.Now;
            return false;
        }
        public static bool IsLimitedContactUs(string key, int seconds)
        {
            if (RequestTimesContactUs.TryGetValue(key, out DateTime lastRequest))
            {
                if ((DateTime.Now - lastRequest).TotalSeconds < seconds)
                {
                    return true; // Too fast → limit
                }
            }

            // Update last request time
            RequestTimesContactUs[key] = DateTime.Now;
            return false;
        }
        public static bool IsLimitedNewsLetterSubscrip(string key, int seconds)
        {
            if (RequestTimesNewsLetterSubscrip.TryGetValue(key, out DateTime lastRequest))
            {
                if ((DateTime.Now - lastRequest).TotalSeconds < seconds)
                {
                    return true; // Too fast → limit
                }
            }

            // Update last request time
            RequestTimesNewsLetterSubscrip[key] = DateTime.Now;
            return false;
        }

    }
}
