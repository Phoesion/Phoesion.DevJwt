using Microsoft.IdentityModel.Tokens;
using System.CommandLine;
using System.CommandLine.Binding;
using System.CommandLine.Invocation;
using System.CommandLine.NamingConventionBinder;
using System.CommandLine.Parsing;

namespace Phoesion.DevJwt.CLI;

internal class Program
{
    static Task<int> Main(string[] args)
    {
#if DEBUG
        args ??= new[] { "create", "myApi", "--email", "u@test.com", "--sub", "42", "--role", "a", "--role", "b", "--claim", "some=thing", "other=nothing", "--audience", "aud2", "--scope", "openid", "profile" };
#endif

        var audienceArgument = new CliArgument<string>(name: "aud") { Description = "The 'aud' (audience) claim for the token." };

        var subOption = new CliOption<string>(name: "--sub") { Description = "Add claim for 'sub' (subject) type." };
        var emailOption = new CliOption<string>(name: "--email") { Description = "Add claim for 'sub' (subject) type." };
        var scopeOption = new CliOption<string[]>(name: "--scope") { Description = "Add 'scope' claim.", AllowMultipleArgumentsPerToken = true };
        var roleOption = new CliOption<string[]>(name: "--role") { Description = "Add 'role' claim.", AllowMultipleArgumentsPerToken = true };
        var claimsOption = new CliOption<string[]>(name: "--claim") { Description = "Add claims to the JWT. (in the format \"name=value\")", AllowMultipleArgumentsPerToken = true };
        var durationOption = new CliOption<int>(name: "--duration") { Description = "Set duration (in days) for the token. ( expiration date = utc.now + duration )" };
        var audienceOption = new CliOption<string[]>(name: "--audience") { Description = "Add more audience values to the 'aud' claim", AllowMultipleArgumentsPerToken = true };
        var signKeyOption = new CliOption<string>(name: "--signkey") { Description = "Use custom key to sign the jwt." };

        var rootCommand = new CliRootCommand("dotnet tool for generating jwt tokens for the Phoesion.DevJwt library");

        var createCommand = new CliCommand("create", "Create a new jwt token.")
        {
            audienceArgument,

            subOption,
            emailOption,
            scopeOption,
            roleOption,
            claimsOption,
            durationOption,
            audienceOption,
            signKeyOption
        };
        rootCommand.Subcommands.Add(createCommand);

        createCommand.Action = CommandHandler.Create(
            static async (TokenGenOptions o) =>
            {
                //create token builder
                var token = TokenGenerator.Create(o.Audience);

                //setup token
                if (!string.IsNullOrEmpty(o.Subject)) token.AddClaimSubject(o.Subject);
                if (!string.IsNullOrEmpty(o.Email)) token.AddClaimEmail(o.Email);

                //add scopes
                if (o.Scopes != null && o.Scopes.Length > 0)
                    foreach (var scope in o.Scopes)
                        if (!string.IsNullOrWhiteSpace(scope))
                            token.AddScope(scope);

                //add roles
                if (o.Roles != null && o.Roles.Length > 0)
                    foreach (var role in o.Roles)
                        if (!string.IsNullOrWhiteSpace(role))
                            token.AddRole(role);

                //add extra claims
                if (o.Claims != null && o.Claims.Length > 0)
                    foreach (var claim in o.Claims)
                        if (!string.IsNullOrWhiteSpace(claim) || !claim.Contains('='))
                        {
                            //get parts
                            var split = claim.IndexOf('=');
                            var key = claim.Substring(0, split);
                            var value = claim.Substring(split + 1);
                            if (string.IsNullOrWhiteSpace(key) || string.IsNullOrWhiteSpace(value))
                                throw new Exception($"Invalid claim '{claim}'");
                            //add claim
                            token.AddClaim(key, value);
                        }

                //add extra audiences
                if (o.ExtraAudience != null && o.ExtraAudience.Length > 0)
                    foreach (var audience in o.ExtraAudience)
                        if (!string.IsNullOrWhiteSpace(audience))
                            token.AddAudience(audience);

                //setup expiration
                if (o.Duration > 0)
                    token.ExpiresIn(TimeSpan.FromDays(o.Duration));
                else
                    token.ExpiresIn(TimeSpan.FromDays(365));

                //use custom key
                if (!string.IsNullOrWhiteSpace(o.SigningKey))
                    token.WithSigningKey(o.SigningKey.Trim());

                //build and write token
                Console.WriteLine(token.Build());

            });

        //invoke
        return new CliConfiguration(rootCommand).InvokeAsync(args);
    }

    public class TokenGenOptions
    {
        public string Audience { get; set; }

        public string Subject { get; set; }
        public string Email { get; set; }
        public string[] Scopes { get; set; }
        public string[] Roles { get; set; }
        public string[] Claims { get; set; }
        public int Duration { get; set; }
        public string[] ExtraAudience { get; set; }
        public string SigningKey { get; set; }

        public TokenGenOptions(string aud, string sub, string email, string[] scope, string[] role, string[] claim, int duration, string[] audience, string signkey)
        {
            this.Audience = aud;
            this.Subject = sub;
            this.Email = email;
            this.Scopes = scope;
            this.Roles = role;
            this.Claims = claim;
            this.Duration = duration;
            this.ExtraAudience = audience;
            this.SigningKey = signkey;
        }
    }
}