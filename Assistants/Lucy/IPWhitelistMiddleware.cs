using Microsoft.Extensions.Options;
using System.Net;

namespace Lucy
{
    public class IPWhitelistMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IPWhitelistOptions _iPWhitelistOptions;
        private readonly ILogger<IPWhitelistMiddleware> _logger;
        public IPWhitelistMiddleware(RequestDelegate next, ILogger<IPWhitelistMiddleware> logger, IOptions<IPWhitelistOptions> applicationOptionsAccessor)
        {
            _iPWhitelistOptions = applicationOptionsAccessor.Value;
            _next = next;
            _logger = logger;
        }
        public async Task Invoke(HttpContext context)
        {
            var ipAddress = context.Connection.RemoteIpAddress;
            List<string> whiteListIPList =
            _iPWhitelistOptions.Whitelist;
            var isIPWhitelisted = whiteListIPList
            .Where(ip => IPAddress.Parse(ip)
            .Equals(ipAddress))
            .Any();
            if (!isIPWhitelisted)
            {
                _logger.LogWarning(
                "Request from Remote IP address: {RemoteIp} is forbidden.", ipAddress);
                context.Response.StatusCode =
                (int)HttpStatusCode.Forbidden;
                return;
            }
            await _next.Invoke(context);
        }
    }
    public class IPWhitelistOptions
    {
        public List<string> Whitelist { get; set; }
    }
    public static class IPWhitelistMiddlewareExtensions
    {
        public static IApplicationBuilder UseIPWhitelist(this
        IApplicationBuilder builder)
        {
            return builder.UseMiddleware<IPWhitelistMiddleware>();
        }
    }
}
