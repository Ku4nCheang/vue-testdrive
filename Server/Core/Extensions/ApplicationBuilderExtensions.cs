using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using netcore.Models;

namespace netcore.Core.Extensions
{
    public static class ApplicationBuilderExtensions
    {
        public static IApplicationBuilder UseFixture(this IApplicationBuilder builder)
        {   
            var scope = builder.ApplicationServices.CreateScope();
            var seed = scope.ServiceProvider.GetRequiredService(typeof(Seed)) as Seed;
            seed.AddFixture().Wait();
            scope.Dispose();
            return builder;
        }
    }
}