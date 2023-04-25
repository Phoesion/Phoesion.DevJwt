using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.DependencyInjection;
using Phoesion.Glow.SDK.Authentication;
using Phoesion.Glow.SDK.Authorization;
using Phoesion.Glow.SDK.Firefly;
using System.Text;
using System.Threading.Tasks;

namespace SampleGlowMicroservice
{
    [ServiceName("SampleService1")]
    public class ServiceMain : Phoesion.Glow.SDK.Firefly.FireflyService
    {
        protected override void ConfigureServices(IServiceCollection services)
        {
            // Add authorization services
            services.AddAuthorization();

            // Add and configure authentication services
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                    .AddJwtBearer(o => o.AddDevJwt(HostingEnvironment)); //<-- enable dev-jwt (by default only for Development/Testing environments)
        }

        protected override void Configure(IGlowApplicationBuilder app)
        {
            // Enable Authentication/Authorization middleware (order is important!)
            app.UseAuthentication();
            app.UseAuthorization();
        }
    }
}