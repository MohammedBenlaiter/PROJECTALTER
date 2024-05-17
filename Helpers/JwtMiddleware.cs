using PROJECTALTERAPI.Models;
using Microsoft.Extensions.Options;

namespace PROJECTALTERAPI.Helpers
{

    public class JwtMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly HostApplicationBuilderSettings _appSettings;

        public JwtMiddleware(RequestDelegate next, IOptions<HostApplicationBuilderSettings> appSettings)
        {
            _next = next;
            _appSettings = appSettings.Value;
        }
    }
}