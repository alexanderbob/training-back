using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authentication.OAuth;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Threading.Tasks;

namespace Alebob.Training.OAuth
{
    /// <summary>
    /// We are using kestel on HTTP with nginx as proxy server on HTTPS.
    /// Unfortunately, BuildRedirectUri puts HTTP as a Scheme, because request was proxied by nginx.
    /// With this class, we are fixing this issue for non-Development builds
    /// </summary>
    public class CustomGoogleHandler : GoogleHandler
    {
        private bool isProduction;
        public CustomGoogleHandler(IWebHostEnvironment env, IOptionsMonitor<GoogleOptions> options, ILoggerFactory logger, UrlEncoder encoder, ISystemClock clock) : base(options, logger, encoder, clock)
        {
            isProduction = !env.IsDevelopment();
        }

        protected override string BuildChallengeUrl(AuthenticationProperties properties, string redirectUri)
        {
            redirectUri = RebuildRedirectUri(redirectUri);
            Logger.LogWarning($"Sending ChallengeUrl with redirectUri {redirectUri}");
            return base.BuildChallengeUrl(properties, redirectUri);
        }

        protected override async Task<OAuthTokenResponse> ExchangeCodeAsync(OAuthCodeExchangeContext context)
        {
            string redirectUri = RebuildRedirectUri(context.RedirectUri);
            var tokenRequestParameters = new Dictionary<string, string>()
            {
                { "client_id", Options.ClientId },
                { "redirect_uri", redirectUri },
                { "client_secret", Options.ClientSecret },
                { "code", context.Code },
                { "grant_type", "authorization_code" },
            };

            // PKCE https://tools.ietf.org/html/rfc7636#section-4.5, see BuildChallengeUrl
            if (context.Properties.Items.TryGetValue(OAuthConstants.CodeVerifierKey, out var codeVerifier))
            {
                tokenRequestParameters.Add(OAuthConstants.CodeVerifierKey, codeVerifier);
                context.Properties.Items.Remove(OAuthConstants.CodeVerifierKey);
            }
            var requestContent = new FormUrlEncodedContent(tokenRequestParameters);
            Logger.LogInformation($"Sending requestContent to get token: {requestContent}");
            var requestMessage = new HttpRequestMessage(HttpMethod.Post, Options.TokenEndpoint);
            requestMessage.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            requestMessage.Content = requestContent;
            var response = await Backchannel.SendAsync(requestMessage, Context.RequestAborted);
            if (response.IsSuccessStatusCode)
            {
                var payload = JsonDocument.Parse(await response.Content.ReadAsStringAsync());
                return OAuthTokenResponse.Success(payload);
            }
            else
            {
                var error = "OAuth token endpoint failure: " + await response.Content.ReadAsStringAsync();
                return OAuthTokenResponse.Failed(new Exception(error));
            }
        }

        private string RebuildRedirectUri(string redirectUri)
        {
            if (isProduction)
            {
                UriBuilder ub = new UriBuilder(redirectUri);
                ub.Scheme = "https";
                ub.Port = -1;
                redirectUri = ub.ToString();
            }
            return redirectUri;
        }
    }
}
