using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Products.Application.Interfaces.Messaging;
using Products.Infrastructure.Data;

namespace Products.IntegrationTests;

public class CustomWebApplicationFactory<TProgram> : WebApplicationFactory<TProgram> where TProgram : class
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {

            var dbDescriptor = services.SingleOrDefault(
                d => d.ServiceType == typeof(DbContextOptions<ProductContext>));

            if (dbDescriptor != null)
            {
                services.Remove(dbDescriptor);
            }

            var rabbitMQDescriptor = services.SingleOrDefault(
                d => d.ServiceType == typeof(IMessageBus));

            if (rabbitMQDescriptor != null)
            {
                services.Remove(rabbitMQDescriptor);
            }

            var hostedServices = services.Where(s => s.ServiceType == typeof(IHostedService)).ToList();
            foreach (var service in hostedServices)
            {
                services.Remove(service);
            }

            services.AddDbContext<ProductContext>(options =>
            {
                options.UseInMemoryDatabase("TestDb_" + Guid.NewGuid().ToString());
            });

            var sp = services.BuildServiceProvider();

            using (var scope = sp.CreateScope())
            {
                var scopedServices = scope.ServiceProvider;
                var db = scopedServices.GetRequiredService<ProductContext>();
                db.Database.EnsureCreated();
            }
        });
    }
}

