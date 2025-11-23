using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Users.Application.Interfaces.Security;
using Users.Infrastructure.Security;

namespace Microsoft.Extensions.DependencyInjection;

public static class DependencyInjection
{
    public static void AddInfrastructureServices(this IHostApplicationBuilder builder)
    {
        builder.Services.AddSingleton<IPasswordHasher, BCryptPasswordHasher>();
    }
}
