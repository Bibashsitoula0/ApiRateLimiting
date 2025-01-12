using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Http;

namespace ApiRateLimiting
{   

    public class RateLimitFilter : IAsyncActionFilter
    {
        private readonly CustomRateLimitService _rateLimitService;

        public RateLimitFilter(CustomRateLimitService rateLimitService)
        {
            _rateLimitService = rateLimitService;
        }

        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            var clientId = context.HttpContext.Connection.RemoteIpAddress.ToString(); 
            var endpoint = context.ActionDescriptor.AttributeRouteInfo?.Template ?? "/";

            if (!_rateLimitService.IsRequestAllowed(clientId, endpoint))
            {
                context.HttpContext.Response.Headers.Add("X-Rate-Limit-Remaining", "0");
                context.HttpContext.Response.Headers.Add("X-Rate-Limit-Reset", DateTime.UtcNow.AddMinutes(1).ToString());

                var rateLimitExceededModel = new RateLimitExceededModel(
                    "Rate limit exceeded",
                    StatusCodes.Status429TooManyRequests,
                    0,
                    DateTime.UtcNow.AddMinutes(1)
                );

                context.Result = new RateLimitExceededActionResult(rateLimitExceededModel);
                return;
            }
            context.HttpContext.Response.Headers.Add("X-Rate-Limit-Remaining", _rateLimitService.GetRemainingRequests(clientId, endpoint).ToString());
            await next(); 
        }
    }
}
