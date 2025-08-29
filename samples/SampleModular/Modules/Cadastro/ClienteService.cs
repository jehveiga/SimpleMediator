using Cadastro.Commands;
using SimpleMediator.Interfaces;

namespace Cadastro;

public class ClienteService : IClienteService
{
    private readonly IMediator _mediator;

    public ClienteService(IMediator mediator)
    {
        _mediator = mediator;
    }

    public async Task<string> CadastrarCliente(CadastrarClienteCommand cliente)
    {
        string resultado = await _mediator.Send(new CadastrarClienteCommand { Id = cliente.Id, Nome = cliente.Nome }, default);
        return resultado;
    }

    public async Task<bool> ExcluirCliente(ExcluirClienteCommand cliente)
    {
        bool resultado = await _mediator.Send(new ExcluirClienteCommand { Id = cliente.Id }, default);
        return resultado;
    }
}