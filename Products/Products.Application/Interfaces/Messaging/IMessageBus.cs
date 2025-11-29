using System;
using System.Threading.Tasks;

namespace Products.Application.Interfaces.Messaging
{
    public interface IMessageBus
    {
        void Publish<T>(T message, string queueName);
        void Consume<T>(string queueName, Action<T> action);
        void Dispose();
    }
}