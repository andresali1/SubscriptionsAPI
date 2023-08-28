using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using WebApiAuthors;
using WebApiAuthors.DTOs;
using WebApiAuthors.Entities;

namespace WebApiAuthors.Middlewares
{
    public static class RequestsLimitMiddlewareExtensions
    {
        public static IApplicationBuilder UseRequestsLimit(this IApplicationBuilder app)
        {
            return app.UseMiddleware<RequestsLimitMiddleware>();
        }
    }
}

public class RequestsLimitMiddleware
{
    private readonly RequestDelegate _next;
    private readonly IConfiguration _configuration;

    public RequestsLimitMiddleware(RequestDelegate next, IConfiguration configuration)
    {
        _next = next;
        _configuration = configuration;
    }

    /// <summary>
    /// Method to forbid any request if this doesn't have the Correct header to know the ApiKey
    /// </summary>
    /// <param name="httpContext"></param>
    /// <param name="context"></param>
    /// <returns></returns>
    public async Task InvokeAsync(HttpContext httpContext, ApplicationDbContext context)
    {
        var requestsLimitConfiguration = new RequestLimitConfiguration();
        _configuration.GetRequiredSection("LimitRequests").Bind(requestsLimitConfiguration);

        var route = httpContext.Request.Path.ToString();
        var isInWhiteList = requestsLimitConfiguration.RouteWhiteList.Any(r => route.Contains(r));

        if (isInWhiteList)
        {
            await _next(httpContext);
            return;
        }

        var keyStringValues = httpContext.Request.Headers["X-Api-Key"];

        if(keyStringValues.Count == 0)
        {
            httpContext.Response.StatusCode = 400;
            await httpContext.Response.WriteAsync("Debe proveer la llave en la cabecera X-Api-Key");
            return;
        }

        if(keyStringValues.Count > 1)
        {
            httpContext.Response.StatusCode = 400;
            await httpContext.Response.WriteAsync("Solo una llave debe de estar presente");
            return;
        }

        var key = keyStringValues[0];

        var keyDB = await context.APIKey
            .Include(k => k.DomainRestrictions)
            .Include(k => k.IPRestrictions)
            .FirstOrDefaultAsync(k => k.Key == key);
        
        if (keyDB == null)
        {
            httpContext.Response.StatusCode = 400;
            await httpContext.Response.WriteAsync("La llave no existe");
            return;
        }

        if (!keyDB.Active)
        {
            httpContext.Response.StatusCode = 400;
            await httpContext.Response.WriteAsync("La llave se encuentra inactiva");
            return;
        }

        if(keyDB.KeyType == KeyType.Free)
        {
            var today = DateTime.Today;
            var tomorrow = today.AddDays(1);
            var requestsMadeToday = await context.Request.CountAsync(
                r => r.KeyId == keyDB.Id
                && r.RequestDate >= today
                && r.RequestDate < tomorrow
            );

            if(requestsMadeToday >= requestsLimitConfiguration.FreeRequestsPerDay)
            {
                httpContext.Response.StatusCode = 429; //Too many requests
                await httpContext.Response.WriteAsync(
                    @"Ha excedido el límita de preticiones por día. Si desea realizar más peticiones
                      actualice su suscripción a una cuenta profesional"
                );
                return;
            }
        }

        var passRestrictions = PassOneRestriction(keyDB, httpContext);

        if (!passRestrictions)
        {
            httpContext.Response.StatusCode = 403; //Prohibited
            return;
        }

        var request = new Request() { KeyId = keyDB.Id, RequestDate = DateTime.UtcNow };
        context.Add(request);
        await context.SaveChangesAsync();

        await _next(httpContext);
    }

    /// <summary>
    /// Method to validate if the request pass at least one Domain or IP restriction
    /// </summary>
    /// <param name="apiKey"></param>
    /// <param name="httpContext"></param>
    /// <returns></returns>
    private bool PassOneRestriction(APIKey apiKey, HttpContext httpContext)
    {
        var areRestrictions = apiKey.DomainRestrictions.Any() || apiKey.IPRestrictions.Any();

        if(!areRestrictions)
        {
            return true;
        }

        var requestPassDomainRestrictions = RequestPassDomainRestrictions(apiKey.DomainRestrictions, httpContext);

        var requestPassIPRestrictions = RequestPassIPRestrictions(apiKey.IPRestrictions, httpContext);

        return requestPassDomainRestrictions || requestPassIPRestrictions;
    }

    /// <summary>
    /// Method to validate if the request surpass the IP restriction
    /// </summary>
    /// <param name="restrictions">Given IP restrictions</param>
    /// <param name="httpContext"></param>
    /// <returns></returns>
    private bool RequestPassIPRestrictions(List<IPRestriction> restrictions, HttpContext httpContext)
    {
        if (restrictions == null || restrictions.Count == 0)
        {
            return false;
        }

        var IP = httpContext.Connection.RemoteIpAddress.ToString();

        if(IP == string.Empty)
        {
            return false;
        }

        var passRestriction = restrictions.Any(r => r.IP == IP);

        return passRestriction;
    }

    /// <summary>
    /// Method to validate if the request surpass the Domain restrictions
    /// </summary>
    /// <param name="restrictions">Given domain restrictions</param>
    /// <param name="httpContext"></param>
    /// <returns></returns>
    private bool RequestPassDomainRestrictions(List<DomainRestriction> restrictions, HttpContext httpContext)
    {
        if(restrictions == null || restrictions.Count == 0)
        {
            return false;
        }

        var referer = httpContext.Request.Headers["Referer"].ToString();

        if(referer == string.Empty)
        {
            return false;
        }

        Uri myUri = new Uri(referer);
        string host = myUri.Host;

        var passRestrictions = restrictions.Any(x => x.Domain == host);
        return passRestrictions;
    }
}
