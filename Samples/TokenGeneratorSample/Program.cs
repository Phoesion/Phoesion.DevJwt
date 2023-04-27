using Phoesion.DevJwt;

namespace TokenGeneratorSample
{
    internal class Program
    {
        static void Main(string[] args)
        {
            //parameters
            string userId = new Guid().ToString();
            string email = "john.doe@example.com";
            string audience = "myApi";

            //generate
            var token = TokenGenerator.Create(audience, email, userId)
                                      .AddScope("openid", "profile")
                                      .AddRole("admin")
                                      .AddClaim("username", "johndoe")
                                      .ExpiresIn(TimeSpan.FromDays(365))
                                      .Build();

            Console.WriteLine("Token : " + token);
            Console.WriteLine("");

            Console.WriteLine("Press any key to continue...");
            Console.ReadKey();
        }
    }
}