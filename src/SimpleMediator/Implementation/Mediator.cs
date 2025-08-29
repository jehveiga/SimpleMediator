using Microsoft.Extensions.DependencyInjection;
using SimpleMediator.Interfaces;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace SimpleMediator.Implementation
{
    public class Mediator : IMediator
    {
        private readonly IServiceProvider _provider;

        public Mediator(IServiceProvider provider)
        {
            _provider = provider;
        }

#pragma warning disable S1006 // Method overrides should not change parameter defaults

        public async Task<TResponse> Send<TResponse>(IRequest<TResponse> request, CancellationToken cancellationToken = default)
#pragma warning restore S1006 // Method overrides should not change parameter defaults
        {
            Type handlerType = typeof(IRequestHandler<,>).MakeGenericType(request.GetType(), typeof(TResponse));
            object handler = _provider.GetService(handlerType);

            if (handler == null)
                throw new InvalidOperationException($"No handler registered for {request.GetType().Name}");

            return await (Task<TResponse>)handlerType
                .GetMethod("Handle")
                .Invoke(handler, new object[] { request, cancellationToken });
        }

#pragma warning disable S1006 // Method overrides should not change parameter defaults

        public async Task Publish<TNotification>(TNotification notification, CancellationToken cancellationToken = default)
#pragma warning restore S1006 // Method overrides should not change parameter defaults
            where TNotification : INotification
        {
            Type handlerType = typeof(INotificationHandler<>).MakeGenericType(notification.GetType());
            IEnumerable<object?> handlers = _provider.GetServices(handlerType);

            foreach (object? handler in handlers)
            {
                await (Task)handlerType
                    .GetMethod("Handle")
                    .Invoke(handler, new object[] { notification, cancellationToken });
            }
        }
    }
}