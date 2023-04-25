using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;

namespace Phoesion.DevJwt
{
    public class DevJwtOptions : AuthenticationSchemeOptions
    {
        public string Scheme => DevJwtDefaults.AuthenticationScheme;
        public string AuthenticationType = DevJwtDefaults.AuthenticationScheme;

        public string Audience { get; set; }

        public string SigningKey { get; set; } = DevJwtDefaults.DefaultSigningKey;

        public bool ValidateAudience { get; set; } = true;
        public bool ValidateIssuer { get; set; } = true;
        public bool ValidateLifetime { get; set; } = true;
    }
}
