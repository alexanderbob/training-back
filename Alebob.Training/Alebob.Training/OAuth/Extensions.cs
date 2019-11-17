using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Alebob.Training.OAuth
{
    public static class OauthExtensions
    {
        public static AuthenticationBuilder AddCustomGoogle(this AuthenticationBuilder builder)
            => builder.AddCustomGoogle(GoogleDefaults.AuthenticationScheme, _ => { });

        public static AuthenticationBuilder AddCustomGoogle(this AuthenticationBuilder builder, Action<GoogleOptions> configureOptions)
            => builder.AddCustomGoogle(GoogleDefaults.AuthenticationScheme, configureOptions);

        public static AuthenticationBuilder AddCustomGoogle(this AuthenticationBuilder builder, string authenticationScheme, Action<GoogleOptions> configureOptions)
            => builder.AddCustomGoogle(authenticationScheme, GoogleDefaults.DisplayName, configureOptions);

        public static AuthenticationBuilder AddCustomGoogle(this AuthenticationBuilder builder, string authenticationScheme, string displayName, Action<GoogleOptions> configureOptions)
            => builder.AddOAuth<GoogleOptions, CustomGoogleHandler>(authenticationScheme, displayName, configureOptions);
    }
}
