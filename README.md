# API Rate Limiting Package

This is a simple API rate limiting middleware and service that allows you to limit the number of requests made by users or IP addresses within a specified time period.

# Features
1. IP-based or User-based rate limiting: Supports rate limiting based on user identity or IP address.
2. Customizable rules: Define custom rules for rate limiting, such as requests per minute, hour, or day.
3. Memory-based storage: Uses an in-memory cache to track requests per user/IP.
4. Configurable: Configure the rate limits and time periods via JSON configuration or programmatically.
5. Extensible: Easily extend the rate limiting logic or add custom features to fit your needs.

# Installation
To install the package, use the `dotnet add package ApiRateLimiting --version 1.1.0`.
Alternatively, you can download it from the [NuGet Gallery](https://www.nuget.org/packages/ApiRateLimiting).

 ### Key Steps:

1. Create a blank project using the ASP.NET Web Api.
2. Choose .NET 8.0 for the project.
3. Install the ApiRateLimiting from NuGet packages.
`Install-Package ApiRateLimiting -Version 1.1.0`
4. Add code in the program.cs file:
   - Add namespace `using ApiRateLimiting;`.   
   - Add
     ```
     builder.Services.Configure<IpRateLimitOptions>(builder.Configuration.GetSection("IpRateLimiting"));   
     builder.Services.AddApiRateLimiting();      
     ```   
   - Add `app.UseMiddleware<RateLimitingMiddleware>();`.
   
5. Update the application settings with the following configurations:
  ```json
  "IpRateLimiting": {
    "GeneralRules": [
      {
        "Endpoint": "WeatherForecast/weatherforecast",  //Set multiple endpoint 
        "Period": "2m",                                 //Set period day for d ,hour for h,min for m 
        "Limit": 1                                      // Set limit
      },
      {
        "Endpoint": "WeatherForecast/weatherforecast/v1",
        "Period": "2m",
        "Limit": 2
      },
      {
        "Endpoint": "WeatherForecast/weatherforecast/v2",
        "Period": "2m",     
        "Limit": 3
      }
    ]

  }
```   
6. Run the project to complete the setup.

## How It Works
1. The rate limiting service tracks requests based on the user or IP address and stores the data in memory.
2. The CustomRateLimitService checks if the number of requests exceeds the defined limit within the specified period (e.g., 100 requests per hour).
3. If the limit is exceeded, the RateLimitFilter blocks the request and returns a 429 Too Many Requests response with rate limiting details.

