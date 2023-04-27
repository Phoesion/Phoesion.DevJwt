using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Hosting.Internal;

namespace SampleWebApi
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddControllers();

            builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                            .AddJwtBearer(o => o.UseDevJwt(builder.Environment)); //<-- enables dev-jwt (only for Development/Testing environments)


            var app = builder.Build();

            app.UseHttpsRedirection();

            app.UseAuthorization();

            app.MapControllers();

            app.Run();
        }
    }
}