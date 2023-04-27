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

        /// <summary>
        /// Enable the DevJwt validator. By default it only enables for 'Development' and 'Testing' environments
        /// </summary>
        /// <param name="options">The JwtBearerOptions to modify</param>
        /// <param name="env">the hosting environment to be used for checking if DevJwt will be enabled</param>
        /// <param name="key">Specify a custom key for checking key signature</param>
        /// <param name="allowed_environments">Environment names to allow the DevJwt to be enabled</param>
        /// <returns>True if the DevJwt validator has been added. False if no changes have been made</returns>
        public static bool UseDevJwt(this Microsoft.AspNetCore.Authentication.JwtBearer.JwtBearerOptions options, IHostEnvironment env, string signkey = null, string[] allowed_environments = null)
        {
            //check if valid environment
            if (allowed_environments != null)
            {
                if (!allowed_environments.Contains(env.EnvironmentName))
                    return false;
            }
            else if (!(env.IsDevelopment() || env.EnvironmentName == "Testing"))
                return false;

            //do not allow production environment without at-least a custom key!
            if (env.IsProduction() && string.IsNullOrWhiteSpace(signkey))
                throw new Exception("UseDevJwt() cannot be used in Production environment without a custom signing key!");

            //add custom validator
            options.SecurityTokenValidators.Insert(0, new TokenValidator(signkey));

            //add valid issuers
            var issuers = options.TokenValidationParameters.ValidIssuers?.ToHashSet() ?? new();
            if (!string.IsNullOrWhiteSpace(options.TokenValidationParameters.ValidIssuer))
                issuers.Add(options.TokenValidationParameters.ValidIssuer);
            issuers.Add("phoesion.devjwt");
            options.TokenValidationParameters.ValidIssuer = null;
            options.TokenValidationParameters.ValidIssuers = issuers;

            //inform that validator has been applied!
            return true;
        }
    }
}

