using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Users.Application.Interfaces.Identity;
using Users.Application.Interfaces.Messaging;
using Users.Application.Interfaces.Repositories;
using Users.Application.Interfaces.Services;
using Users.Infrastructure.Data;
using Users.Infrastructure.Data.Repositories;
using Users.Infrastructure.Identity;
using Users.Infrastructure.Messaging;
using Users.Infrastructure.Models;
using Users.Infrastructure.Services;
using Users.Infrastructure.Validators;

namespace Microsoft.Extensions.DependencyInjection;

public static class DependencyInjection
{
    public static void AddInfrastructureServices(this IHostApplicationBuilder builder)
    {
        
        builder.Services.AddSingleton<IPasswordHasher, BCryptPasswordHasher>();
        builder.Services.AddSingleton<IJwtTokenGenerator, JwtTokenGenerator>();

        builder.Services.AddDbContext<UserContext>((sp, options) =>
        {
            var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
            if (string.IsNullOrEmpty(connectionString))
            {
                throw new InvalidOperationException("Database connection string is not configured.");
            }
            options.UseSqlServer(connectionString);
        });

        builder.Services.AddScoped<IUserRepository, UserRepository>();
        builder.Services.AddScoped<IEmailService, EmailService>();
        builder.Services.AddSingleton<IMessageBus, RabbitMQService>();

        builder.Services.Configure<SmtpSettings>(builder.Configuration.GetSection("SmtpSettings"));
        builder.Services.Configure<ApplicationSettings>(builder.Configuration.GetSection("ApplicationSettings"));

        builder.Services.AddScoped<IValidator<SmtpSettings>, SmtpSettingsValidator>();
        builder.Services.AddScoped<IValidator<ApplicationSettings>, ApplicationSettingsValidator>();
    }
}