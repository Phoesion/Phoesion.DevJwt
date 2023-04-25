using Phoesion.DevJwt;

namespace TokenGeneratorSample
{
    internal class Program
    {
        static void Main(string[] args)
        {
            //parameters
            string username = "johnDoe";
            string email = "john.doe@example.com";
            string audience = "myApi";

            //generate
            var token = TokenGenerator.Create(audience, email, username)
                                      .ExpiresIn(TimeSpan.FromDays(365))
                                      .Build();

            Console.WriteLine("Token : " + token);
            Console.WriteLine("");

            Console.WriteLine("Press any key to continue...");
            Console.ReadKey();
        }
    }
}