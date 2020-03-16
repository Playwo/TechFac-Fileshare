using System;
using System.Linq;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Fileshare.Extensions;
using Fileshare.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Fileshare.Authentication
{
    public class BasicAuthHandler : AuthenticationHandler<BasicAuthOptions>
    {
        private readonly FileshareContext DbContext;

        public BasicAuthHandler(IOptionsMonitor<BasicAuthOptions> options, ILoggerFactory logger, UrlEncoder encoder, ISystemClock clock,
            FileshareContext dbContext)
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

            byte[] authBytes = Convert.FromBase64String(authHeader.Parameter);
            string userAndPass = Encoding.UTF8.GetString(authBytes);
            string[] parts = userAndPass.Split(':');
            if (parts.Length != 2)
            {
                return AuthenticateResult.Fail("Invalid Basic authentication header");
            }
            string username = parts[0];
            string password = parts[1];

            var user = await DbContext.Users.Where(x => x.Username == username && x.PasswordHash == password.GetHashString())
                                            .FirstOrDefaultAsync();

            if (user == null)
            {
                return AuthenticateResult.Fail("Invalid username or password");
            }

            var claims = new[]
            {
                new Claim(ClaimTypes.Name, username),
                new Claim("UserId", user.Id.ToString()),
            };

            var identity = new ClaimsIdentity(claims, Scheme.Name);
            var principal = new ClaimsPrincipal(identity);
            var ticket = new AuthenticationTicket(principal, Scheme.Name);

            return AuthenticateResult.Success(ticket);
        }
    }
}
