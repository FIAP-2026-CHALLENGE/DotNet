using DotNet.Domain.Excecoes;
using DotNet.Domain.Entidades;
using DotNet.Domain.Repositorios;
using DotNet.Application.Dtos;

namespace DotNet.Application.Servicos;

public class ResponsavelServico : IResponsavelServico
{
    private readonly IResponsavelRepositorio _responsavelRepositorio;

    public ResponsavelServico(IResponsavelRepositorio responsavelRepositorio)
    {
        _responsavelRepositorio = responsavelRepositorio;
    }

    public async Task<IEnumerable<ResponsavelResponse>> ListarAsync()
    {
        return (await _responsavelRepositorio.ObterTodosAsync()).Select(ResponsavelResponse.De).ToList();
    }

    public async Task<ResponsavelResponse> ObterPorIdAsync(int id)
    {
        var responsavel = await _responsavelRepositorio.ObterPorIdAsync(id);

        return ResponsavelResponse.De(responsavel ?? throw new RecursoNaoEncontradoException($"Responsavel com Id {id} não foi encontrado."));
    }

    public async Task<ResponsavelResponse> ObterPorCpfAsync(string cpf)
    {
        var responsavel = await _responsavelRepositorio.ObterPorCpfAsync(cpf);

        return ResponsavelResponse.De(responsavel ?? throw new RecursoNaoEncontradoException($"Responsavel com CPF {cpf} não foi encontrado."));
    }

    public async Task<ResponsavelResponse> CriarAsync(CriarResponsavelRequest request)
    {
        var (name, email, phone, cpf) = request;

        if (await _responsavelRepositorio.CpfExisteAsync(cpf))
        {
            throw new RegraDeNegocioException("CPF already registered.");
        }

        var responsavel = Responsavel.Criar(name, email, phone, cpf);

        await _responsavelRepositorio.AdicionarAsync(responsavel);

        return ResponsavelResponse.De(responsavel);
    }

    public async Task AtualizarAsync(int id, AtualizarResponsavelRequest request)
    {
        var (name, email, phone, cpf, isActive) = request;

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