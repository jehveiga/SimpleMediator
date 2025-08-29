using SharedKernel.Events;
using SimpleMediator.Interfaces;

namespace Crm.Handlers;

public class NotificadorHandler :
    INotificationHandler<ClienteCadastradoEvent>,
    INotificationHandler<ClienteExcluidoEvent>
{
    public Task Handle(ClienteCadastradoEvent notification, CancellationToken cancellationToken)
    {
        Console.WriteLine($"[CRM] Enviando e-mail de boas-vindas ao cliente {notification.ClienteId}");
        return Task.CompletedTask;
    }

    public Task Handle(ClienteExcluidoEvent notification, CancellationToken cancellationToken)
    {
        Console.WriteLine($"[CRM] Enviando e-mail de despedidas ao cliente {notification.ClienteId}");
        return Task.CompletedTask;
    }
}