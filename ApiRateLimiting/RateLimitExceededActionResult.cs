using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using System;
using System.Threading.Tasks;

namespace ApiRateLimiting
{
    public class RateLimitExceededActionResult : ActionResult
    {
        private readonly RateLimitExceededModel _model;

        public RateLimitExceededActionResult(RateLimitExceededModel model)
        {
            _model = model;
        }

        public override Task ExecuteResultAsync(ActionContext context)
        {
            context.HttpContext.Response.StatusCode = _model.StatusCode; 
            return context.HttpContext.Response.WriteAsync(System.Text.Json.JsonSerializer.Serialize(_model)); 
        }
    }
}
