using SimpleMediator.Interfaces;

namespace SharedKernel.Events;
public record ClienteExcluidoEvent(Guid ClienteId) : INotification;