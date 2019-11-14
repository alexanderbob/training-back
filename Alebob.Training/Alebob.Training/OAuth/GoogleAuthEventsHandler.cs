using Alebob.Training.DataLayer;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.OAuth;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Alebob.Training.OAuth
{
    public class GoogleAuthEventsHandler : OAuthEvents
    {
        private string _frontIndexUrl;
        private IUserProvider _userProvider;
        public GoogleAuthEventsHandler(IConfiguration configuration, IUserProvider userProvider)
        {
            _frontIndexUrl = configuration.GetValue<string>("Application:frontAddress");
            _userProvider = userProvider;
        }

        public override Task RedirectToAuthorizationEndpoint(RedirectContext<OAuthOptions> context)
        {
            context.Response.Headers["location"] = context.RedirectUri;
            context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
            return Task.CompletedTask;
        }

        public override async Task CreatingTicket(OAuthCreatingTicketContext context)
        {
            context.Properties.RedirectUri = _frontIndexUrl;
            var user = await _userProvider.UpsertUser(
                context.Identity.FindFirst(ClaimTypes.Email).Value,
                context.Identity.FindFirst(ClaimTypes.Name).Value
            ).ConfigureAwait(false);
            context.Identity.AddClaim(new Claim(Claims.UserId, user.Id));
        }
    }
}
