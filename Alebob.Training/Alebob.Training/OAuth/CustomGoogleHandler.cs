using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Encodings.Web;
using System.Threading.Tasks;

namespace Alebob.Training.OAuth
{
    public class CustomGoogleHandler : GoogleHandler
    {
        private bool isProduction;
        public CustomGoogleHandler(IWebHostEnvironment env, IOptionsMonitor<GoogleOptions> options, ILoggerFactory logger, UrlEncoder encoder, ISystemClock clock) : base(options, logger, encoder, clock)
        {
            isProduction = !env.IsDevelopment();
        }

        // we are using kestel on HTTP with nginx as proxy server on HTTPS
        // unfortunately, BuildRedirectUri puts http as Scheme, because
        // request was proxied by nginx
        protected override string BuildChallengeUrl(AuthenticationProperties properties, string redirectUri)
        {
            if (isProduction)
            {
                UriBuilder ub = new UriBuilder(redirectUri);
                ub.Scheme = "https";
                ub.Port = -1;
                redirectUri = ub.ToString();
            }
            return base.BuildChallengeUrl(properties, redirectUri);
        }
    }
}
