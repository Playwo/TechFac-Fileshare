using Microsoft.AspNetCore.Authentication;

namespace Fileshare.Authentication
{
    public class KeyAuthOptions : AuthenticationSchemeOptions
    {
        public string ApiKey { get; set; }
    }
}
