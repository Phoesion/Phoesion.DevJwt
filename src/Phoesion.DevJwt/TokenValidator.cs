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
    class TokenValidator : ISecurityTokenValidator
    {
        readonly JwtSecurityTokenHandler jwtTokenHandler;
        readonly SecurityKey key;

        public bool CanValidateToken => true;

        public int MaximumTokenSizeInBytes { get => jwtTokenHandler.MaximumTokenSizeInBytes; set => jwtTokenHandler.MaximumTokenSizeInBytes = value; }


        public TokenValidator(string key)
        {
            this.jwtTokenHandler = new JwtSecurityTokenHandler();
            this.key = DevJwtDefaults.DefaultSigningSecurityKey ?? new SymmetricSecurityKey(TokenGenerator.GetSigningKeyBufferFromString(key));
        }

        public bool CanReadToken(string securityToken) => jwtTokenHandler.CanReadToken(securityToken);

        public ClaimsPrincipal ValidateToken(string securityToken, TokenValidationParameters validationParameters, out SecurityToken validatedToken)
        {
            //validate access token
            var claims = jwtTokenHandler.ValidateToken(securityToken,
                                new TokenValidationParameters()
                                {
                                    ValidateAudience = validationParameters.ValidateAudience,
                                    ValidateIssuer = validationParameters.ValidateIssuer,
                                    ValidateLifetime = validationParameters.ValidateLifetime,
                                    IssuerSigningKey = validationParameters.IssuerSigningKey ?? key,
                                    ValidIssuer = validationParameters.ValidIssuer ?? DevJwtDefaults.Issuer,
                                    ValidAudience = validationParameters.ValidAudience,
                                    ValidAudiences = validationParameters.ValidAudiences,
                                },
                                out validatedToken);

            if (claims == null || validatedToken == null)
                throw new Exception("JWT token not valid");

            return claims;
        }
    }
}
