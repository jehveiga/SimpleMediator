using Cadastro.Commands;

namespace Cadastro;

public interface IClienteService
{
    Task<string> CadastrarCliente(CadastrarClienteCommand cliente);

    Task<bool> ExcluirCliente(ExcluirClienteCommand cliente);
}