using SimpleMediator.Interfaces;

namespace Cadastro.Commands;

public class CadastrarClienteCommand : IRequest<string>
{
    public Guid Id { get; set; }
    public required string Nome { get; set; }
}