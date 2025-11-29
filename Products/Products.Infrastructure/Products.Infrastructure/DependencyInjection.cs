using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Products.Application.Interfaces.Messaging;
using Products.Domain.Interfaces.Repositories;
using Products.Infrastructure.Data;
using Products.Infrastructure.Data.Repositories;
using Products.Infrastructure.Messaging;

namespace Microsoft.Extensions.DependencyInjection;

public static class DependencyInjection
{
    public static void AddInfrastructureServices(this IHostApplicationBuilder builder)
    {
        builder.Services.AddDbContext<ProductContext>((sp, options) =>
        {
            options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
        });
        builder.Services.AddScoped<IProductRepository, ProductRepository>();
        builder.Services.AddSingleton<IMessageBus, RabbitMQService>();
        builder.Services.AddHostedService<UserEventsBackgroundService>();
    }
}
