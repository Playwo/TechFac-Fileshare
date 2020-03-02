using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Fileshare.Authentication
{
    public class KeyAuthHandler : AuthenticationHandler<KeyAuthOptions>
    {
        public KeyAuthHandler(IOptionsMonitor<KeyAuthOptions> options, ILoggerFactory logger, UrlEncoder encoder, ISystemClock clock)
            : base(options, logger, encoder, clock)
        {
        }

        protected override Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            if (!Request.Headers.TryGetValue("Authorization", out var header) ||
                !AuthenticationHeaderValue.TryParse(header, out var authHeader) ||
                authHeader.Scheme != Scheme.Name)
            {
                return Task.FromResult(AuthenticateResult.NoResult());
            }

            string apiKey = authHeader.Parameter;

            if (apiKey != Options.ApiKey)
            {
                return Task.FromResult(AuthenticateResult.Fail("Invalid api key"));
            }

            var identity = new ClaimsIdentity(Scheme.Name);
            var principal = new ClaimsPrincipal(identity);
            var ticket = new AuthenticationTicket(principal, Scheme.Name);

            return Task.FromResult(AuthenticateResult.Success(ticket));
        }
    }
}
