using System;
using System.Diagnostics.Eventing.Reader;
using System.Linq;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Phoesion.DevJwt;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class DevJwtBuilderExtensions
    {
        public static AuthenticationBuilder AddDevJwt(this AuthenticationBuilder authenticationBuilder)
            => authenticationBuilder.AddScheme<DevJwtOptions, AuthenticationHandler>(DevJwtDefaults.AuthenticationScheme, _ => { });

        public static AuthenticationBuilder AddDevJwt(this AuthenticationBuilder authenticationBuilder, Action<DevJwtOptions> options)
            => authenticationBuilder.AddScheme<DevJwtOptions, AuthenticationHandler>(DevJwtDefaults.AuthenticationScheme, options);

        public static AuthenticationBuilder AddDevJwt(this AuthenticationBuilder authenticationBuilder, string authSheme, Action<DevJwtOptions> options)
            => authenticationBuilder.AddScheme<DevJwtOptions, AuthenticationHandler>(authSheme, options);


        public static void UseDevJwt(this Microsoft.AspNetCore.Authentication.JwtBearer.JwtBearerOptions options, IHostEnvironment env, string key = null, string[] allowed_environments = null)
        {
            //check if valid environment
            if (allowed_environments != null && !allowed_environments.Contains(env.EnvironmentName))
                return;
            else if (!(env.IsDevelopment() || env.EnvironmentName == "Testing"))
                return;

            //add custom validator
            options.SecurityTokenValidators.Insert(0, new TokenValidator(key));

            //add valid issuers
            var issuers = options.TokenValidationParameters.ValidIssuers?.ToHashSet() ?? new();
            if (!string.IsNullOrWhiteSpace(options.TokenValidationParameters.ValidIssuer))
                issuers.Add(options.TokenValidationParameters.ValidIssuer);
            issuers.Add("phoesion.devjwt");
            options.TokenValidationParameters.ValidIssuer = null;
            options.TokenValidationParameters.ValidIssuers = issuers;
        }
    }
}

