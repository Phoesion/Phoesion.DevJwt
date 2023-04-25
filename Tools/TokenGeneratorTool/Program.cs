using System.CommandLine;

namespace Phoesion.DevJwt.CLI;

internal class Program
{
    static Task<int> Main(string[] args)
    {
        var audienceArgument = new Argument<string>(name: "audience", description: "The 'aud' (audience) claim for the token.");

        var subOption = new Option<string>(name: "--sub", description: "Add claim for 'sub' (subject) type.");
        var emailOption = new Option<string>(name: "--email", description: "Add claim for 'sub' (subject) type.");
        var scopeOption = new Option<string[]>(name: "--scope", description: "Add scope item.") { AllowMultipleArgumentsPerToken = true };
        var claimsOption = new Option<string[]>(name: "--claim", description: "Add claims to the JWT. (in the format \"name=value\")") { AllowMultipleArgumentsPerToken = true };
        var durationOption = new Option<int>(name: "--duration", description: "Set duration (in days) for the token. ( expiration date = utc.now + duration )");
        var audienceOption = new Option<string[]>(name: "--audience", description: "Add more audience values to the 'aud' claim") { AllowMultipleArgumentsPerToken = true };
        var signKeyOption = new Option<string>(name: "--signkey", description: "Use custom key to sign the jwt.");

        var rootCommand = new RootCommand("dotnet tool for generating jwt tokens for the Phoesion.DevJwt library");

        var createCommand = new Command("create", "Create a new jwt token.")
        {
            audienceArgument,
            subOption,
            emailOption,
            scopeOption,
            claimsOption,
            durationOption,
            audienceOption,
            signKeyOption
        };
        rootCommand.AddCommand(createCommand);

        createCommand.SetHandler(static async (aud, sub, email, scopes, claims, duration, audiences, signkey) =>
        {
            //create token builder
            var token = TokenGenerator.Create(aud);

            //setup token
            if (!string.IsNullOrEmpty(sub)) token.AddClaimSubject(sub);
            if (!string.IsNullOrEmpty(email)) token.AddClaimEmail(sub);

            //add scopes
            if (scopes != null && scopes.Length > 0)
                foreach (var scope in scopes)
                    if (!string.IsNullOrWhiteSpace(scope))
                        token.AddScope(scope);

            //add extra claims
            if (claims != null && claims.Length > 0)
                foreach (var claim in claims)
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
            if (audiences != null && audiences.Length > 0)
                foreach (var audience in audiences)
                    if (!string.IsNullOrWhiteSpace(audience))
                        token.AddAudience(audience);

            //setup expiration
            if (duration > 0)
                token.ExpiresIn(TimeSpan.FromDays(duration));
            else
                token.ExpiresIn(TimeSpan.FromDays(365));

            //use custom key
            if (!string.IsNullOrWhiteSpace(signkey))
                token.WithSigningKey(signkey.Trim());

            //build and write token
            Console.WriteLine(token.Build());

        }, audienceArgument, subOption, emailOption, scopeOption, claimsOption, durationOption, audienceOption, signKeyOption);

        return rootCommand.InvokeAsync(args);
    }
}
