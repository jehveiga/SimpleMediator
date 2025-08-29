using SimpleMediator.Interfaces;

namespace SharedKernel.Events;
public record ClienteCadastradoEvent(Guid ClienteId) : INotification;