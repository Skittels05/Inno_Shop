using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Products.Application.CQRS.Commands;
using Products.Application.DTOs;
using Products.Application.Events;
using Products.Application.Interfaces.Messaging;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Products.Infrastructure.Messaging
{
    public class UserEventsBackgroundService : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly IMessageBus _messageBus;
        private readonly ILogger<UserEventsBackgroundService> _logger;

        public UserEventsBackgroundService(
            IServiceProvider serviceProvider,
            IMessageBus messageBus,
            ILogger<UserEventsBackgroundService> logger)
        {
            _serviceProvider = serviceProvider;
            _messageBus = messageBus;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("User Events Background Service started");

            SetupConsumers();

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    await Task.Delay(TimeSpan.FromMinutes(1), stoppingToken);
                    _logger.LogDebug("User Events Background Service is running...");
                }
                catch (TaskCanceledException)
                {

                    break;
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error in User Events Background Service main loop");
                    await Task.Delay(TimeSpan.FromSeconds(30), stoppingToken);
                }
            }

            _logger.LogInformation("User Events Background Service stopped");
        }

        private void SetupConsumers()
        {
            try
            {

                _messageBus.Consume<UserDeactivatedEvent>("user_deactivated", async (eventMessage) =>
                {
                    _logger.LogInformation("Received UserDeactivatedEvent for user: {UserId}", eventMessage.UserId);

                    using var scope = _serviceProvider.CreateScope();
                    var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();

                    try
                    {
                        await mediator.Send(new SoftDeleteProductsByUserCommand(eventMessage.UserId));
                        _logger.LogInformation("Successfully soft deleted products for user: {UserId}", eventMessage.UserId);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Error soft deleting products for user: {UserId}", eventMessage.UserId);

                    }
                });

                _messageBus.Consume<UserActivatedEvent>("user_activated", async (eventMessage) =>
                {
                    _logger.LogInformation("Received UserActivatedEvent for user: {UserId}", eventMessage.UserId);

                    using var scope = _serviceProvider.CreateScope();
                    var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();

                    try
                    {
                        await mediator.Send(new RestoreProductsByUserCommand(eventMessage.UserId));
                        _logger.LogInformation("Successfully restored products for user: {UserId}", eventMessage.UserId);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Error restoring products for user: {UserId}", eventMessage.UserId);
                    }
                });

                _logger.LogInformation("Listening to RabbitMQ queues: user_deactivated, user_activated");
            }
            catch (Exception ex)
            {
                _logger.LogCritical(ex, "Failed to setup RabbitMQ consumers");
                throw;
            }
        }

        public override async Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Stopping User Events Background Service...");
            await base.StopAsync(cancellationToken);
        }
    }
}