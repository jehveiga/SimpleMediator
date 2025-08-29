using SimpleMediator.Interfaces;

namespace Cadastro.Commands;

public class ExcluirClienteCommand : IRequest<bool>
{
    public Guid Id { get; set; }
}