using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace Phoesion.DevJwt
{
    class AuthenticationHandler : AuthenticationHandler<DevJwtOptions>
    {
        public AuthenticationHandler(IOptionsMonitor<DevJwtOptions> options, ILoggerFactory logger, UrlEncoder encoder, ISystemClock clock)
            : base(options, logger, encoder, clock)
        {
        }

        protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            //get auth header
            if (!Request.Headers.TryGetValue("Authorization", out var authHeaders))
                return AuthenticateResult.NoResult();
            var authheader = authHeaders.FirstOrDefault();
            if (authHeaders.Count == 0 || string.IsNullOrWhiteSpace(authheader))
                return AuthenticateResult.NoResult();

            //get token
            var bearerKV = authheader.Split(' ', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);
            if (bearerKV.Length != 2)
                return AuthenticateResult.Fail("Invalid authorization header");

            //get token
            var bearerToken = bearerKV[1];

            //validate access token
            var handler = new JwtSecurityTokenHandler();
            SecurityToken token;
            var claims = handler.ValidateToken(bearerToken,
                new TokenValidationParameters()
                {
                    ValidateAudience = Options.ValidateAudience,
                    ValidateIssuer = Options.ValidateIssuer,
                    ValidateLifetime = Options.ValidateLifetime,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Options.SigningKey)),
                    ValidIssuer = Options.ClaimsIssuer ?? DevJwtDefaults.Issuer,
                    ValidAudience = Options.Audience,
                },
                out token);

            if (claims == null || token == null)
                return AuthenticateResult.Fail("JWT token not valid");

            //build user
            var ticket = new AuthenticationTicket(claims, Options.Scheme);

            //done!
            return AuthenticateResult.Success(ticket);
        }
    }
}
