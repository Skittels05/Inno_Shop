using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Users.Application.Interfaces.Identity;
using Users.Infrastructure.Identity;

namespace Microsoft.Extensions.DependencyInjection;

public static class DependencyInjection
{
    public static void AddInfrastructureServices(this IHostApplicationBuilder builder)
    {
        builder.Services.AddSingleton<IPasswordHasher, BCryptPasswordHasher>();
        builder.Services.AddSingleton<IJwtTokenGenerator, JwtTokenGenerator>();
    }
}
