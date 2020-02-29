using System.Linq;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Fileshare.Extensions;
using Fileshare.Models;

namespace Fileshare.Authentication
{
    public class BearerAuthHandler : AuthenticationHandler<BearerAuthOptions>
    {
        public UploaderContext DbContext;

        public BearerAuthHandler(IOptionsMonitor<BearerAuthOptions> options, ILoggerFactory logger, UrlEncoder encoder, ISystemClock clock,
                                 UploaderContext dbContext)
            : base(options, logger, encoder, clock)
        {
            DbContext = dbContext;
        }

        protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            if (!Request.Headers.TryGetValue("Authorization", out var header) ||
                !AuthenticationHeaderValue.TryParse(header, out var authHeader) ||
                authHeader.Scheme != Scheme.Name)
            {
                return AuthenticateResult.NoResult();
            }

            string token = authHeader.Parameter;

            var user = await DbContext.Users.Where(x => x.Token == token)
                                            .FirstOrDefaultAsync();

            if (user == null)
            {
                return AuthenticateResult.Fail("Invalid token");
            }

            var claims = new[]
            {
                new Claim("UserId", user.Id.ToString()),
                new Claim("Bearer", token),
            };

            var identity = new ClaimsIdentity(claims, Scheme.Name);
            var principal = new ClaimsPrincipal(identity);
            var ticket = new AuthenticationTicket(principal, Scheme.Name);

            return AuthenticateResult.Success(ticket);
        }
    }
}
