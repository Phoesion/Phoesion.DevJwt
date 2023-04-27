using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using static System.Formats.Asn1.AsnWriter;

namespace Phoesion.DevJwt
{
    /// <summary>
    /// Utility for generating tokens programmatically
    /// </summary>
    public static class TokenGenerator
    {
        /// <summary>
        /// Create a new token with a specified audience claim
        /// </summary>
        public static TokenGeneratorBuilder Create(string audience)
            => Create(audience, null, null);

        /// <summary>
        /// Create a new token with a specified audience and email claims
        /// </summary>
        public static TokenGeneratorBuilder Create(string audience, string email)
            => Create(audience, email, null);

        /// <summary>
        /// Create a new token with a specified audience, email and userid claims
        /// </summary>
        public static TokenGeneratorBuilder Create(string audience, string email, string userid)
        {
            var builder = new TokenGeneratorBuilder();
            if (email != null) builder.AddClaimEmail(email);
            if (userid != null) builder.AddClaimSubject(userid);
            if (audience != null) builder.WithAudience(audience);
            return builder;
        }

        internal static byte[] GetSigningKeyBufferFromString(string key)
        {
            if (key.Length >= 16)
                return Encoding.UTF8.GetBytes(key);
            else
                using (var sha = new SHA256Managed())
                    return sha.ComputeHash(Encoding.UTF8.GetBytes(key));
        }
    }

    public class TokenGeneratorBuilder
    {
        public readonly List<Claim> Claims = new List<Claim>();

        TimeSpan? _expire_time;
        string _audience;
        string _signingkey;

        public TokenGeneratorBuilder()
        {
            //generate token id
            Claims.Add(new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()));
        }

        public TokenGeneratorBuilder AddClaim(string type, string value)
        {
            Claims.Add(new Claim(type, value));
            return this;
        }

        public TokenGeneratorBuilder WithAudience(string audience)
        {
            this._audience = audience;
            return this;
        }

        public TokenGeneratorBuilder AddScope(string scope)
            => this.AddClaim("scope", scope);

        public TokenGeneratorBuilder AddScope(params string[] scopes)
        {
            foreach (var scope in scopes)
                this.AddScope(scope);
            return this;
        }

        public TokenGeneratorBuilder AddAudience(string audience)
            => this.AddClaim(JwtRegisteredClaimNames.Aud, audience);

        public TokenGeneratorBuilder AddRole(string role)
            => this.AddClaim("role", role);

        public TokenGeneratorBuilder AddClaimEmail(string value, string type = JwtRegisteredClaimNames.Email)
            => this.AddClaim(type, value);

        public TokenGeneratorBuilder AddClaimSubject(string value, string type = JwtRegisteredClaimNames.Sub)
            => this.AddClaim(type, value);


        public TokenGeneratorBuilder ExpiresIn(TimeSpan? time)
        {
            _expire_time = time;
            return this;
        }

        public TokenGeneratorBuilder WithSigningKey(string key)
        {
            this._signingkey = key;
            return this;
        }

        public string Build()
        {
            //setup SigningCredentials
            var key = new SymmetricSecurityKey(TokenGenerator.GetSigningKeyBufferFromString(_signingkey ?? DevJwtDefaults.DefaultSigningKey));
            var _credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            //create security token
            var token = new JwtSecurityToken(issuer: DevJwtDefaults.Issuer,
                                             audience: _audience,
                                             claims: Claims,
                                             expires: DateTime.UtcNow + (_expire_time ?? TimeSpan.FromDays(365)),
                                             signingCredentials: _credentials
                                            );

            //sign and serialize token to string
            var strToken = new JwtSecurityTokenHandler().WriteToken(token);

            //return token string
            return strToken;
        }
    }
}