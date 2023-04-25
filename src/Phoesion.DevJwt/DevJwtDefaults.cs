using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.IdentityModel.Tokens;

namespace Phoesion.DevJwt
{
    public class DevJwtDefaults
    {
        public const string AuthenticationScheme = "Bearer";

        public const string Issuer = "phoesion.devjwt";

        public const string DefaultSigningKey = "some_default_key_for_devs";

        public readonly static SecurityKey DefaultSigningSecurityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(DefaultSigningKey));

    }
}
