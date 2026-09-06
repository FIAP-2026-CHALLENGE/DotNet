using DotNet.Api.Excecoes;
using DotNet.Api.Models;
using DotNet.Api.Repositorios;

namespace DotNet.Api.Servicos;

public class ResponsavelServico : IResponsavelServico
{
    private readonly IResponsavelRepositorio _responsavelRepositorio;

    public ResponsavelServico(IResponsavelRepositorio responsavelRepositorio)
    {
        _responsavelRepositorio = responsavelRepositorio;
    }

    public async Task<IEnumerable<Responsavel>> ListarAsync()
    {
        return await _responsavelRepositorio.ObterTodosAsync();
    }

    public async Task<Responsavel> ObterPorIdAsync(int id)
    {
        var responsavel = await _responsavelRepositorio.ObterPorIdAsync(id);

        return responsavel ?? throw new RecursoNaoEncontradoException($"Responsavel com Id {id} não foi encontrado.");
    }

    public async Task<Responsavel> ObterPorCpfAsync(string cpf)
    {
        var responsavel = await _responsavelRepositorio.ObterPorCpfAsync(cpf);

        return responsavel ?? throw new RecursoNaoEncontradoException($"Responsavel com CPF {cpf} não foi encontrado.");
    }

    public async Task<Responsavel> CriarAsync(string name, string email, string phone, string cpf)
    {
        if (await _responsavelRepositorio.CpfExisteAsync(cpf))
        {
            throw new RegraDeNegocioException("CPF already registered.");
        }

        var responsavel = Responsavel.Criar(name, email, phone, cpf);

        await _responsavelRepositorio.AdicionarAsync(responsavel);

        return responsavel;
    }

    public async Task AtualizarAsync(int id, string name, string email, string phone, string cpf, bool isActive)
    {
        var responsavel = await _responsavelRepositorio.ObterPorIdAsync(id)
            ?? throw new RecursoNaoEncontradoException($"Responsavel com Id {id} não foi encontrado.");

        if (await _responsavelRepositorio.CpfExisteAsync(cpf, id))
        {
            throw new RegraDeNegocioException("CPF already registered by another responsavel.");
        }

        responsavel.Atualizar(name, email, phone, cpf, isActive);

        await _responsavelRepositorio.SalvarAsync();
    }

    public async Task RemoverAsync(int id)
    {
        var responsavel = await _responsavelRepositorio.ObterPorIdAsync(id)
            ?? throw new RecursoNaoEncontradoException($"Responsavel com Id {id} não foi encontrado.");

        await _responsavelRepositorio.RemoverAsync(responsavel);
    }
}