using System.Threading;
using System.Threading.Tasks;

namespace SimpleMediator.Interfaces
{
    public interface IMediator
    {
        Task<TResponse> Send<TResponse>(IRequest<TResponse> request, CancellationToken cancellationToken);

        Task Publish<TNotification>(TNotification notification, CancellationToken cancellationToken)
            where TNotification : INotification;
    }
}