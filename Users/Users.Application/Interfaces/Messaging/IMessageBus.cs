using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Users.Application.Interfaces.Messaging
{
    public interface IMessageBus
    {
        void Publish<T>(T message, string queueName);
        void Consume<T>(string queueName, Action<T> action);
        void Dispose();
    }
}
