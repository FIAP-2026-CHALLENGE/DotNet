using DotNet.Application.Dtos;
using DotNet.Domain.Consultas;
using DotNet.Domain.Entidades;
using DotNet.Domain.Excecoes;
using DotNet.Domain.Repositorios;

namespace DotNet.Application.Servicos;

public class RelatoTutorServico : IRelatoTutorServico
{
    private readonly IRelatoTutorRepositorio _relatoRepositorio;
    private readonly IAnimalRepositorio _animalRepositorio;

    public RelatoTutorServico(IRelatoTutorRepositorio relatoRepositorio, IAnimalRepositorio animalRepositorio)
    {
        _relatoRepositorio = relatoRepositorio;
        _animalRepositorio = animalRepositorio;
    }

    public async Task<ResultadoPaginado<RelatoTutorResponse>> ListarAsync(FiltroRelatoTutor filtro, ParametrosPaginacao paginacao)
    {
        if (filtro.AnimalId is int animalId && !await _animalRepositorio.ExisteAsync(animalId))
        {
            throw new RecursoNaoEncontradoException($"Animal com Id {animalId} não foi encontrado.");
        }

        var resultado = await _relatoRepositorio.ListarPaginadoAsync(filtro, paginacao);

        return resultado.Mapear(RelatoTutorResponse.De);
    }

    public async Task<RelatoTutorResponse> ObterPorIdAsync(string id)
    {
        return RelatoTutorResponse.De(await ObterOuFalharAsync(id));
    }

    public async Task<RelatoTutorResponse> CriarAsync(CriarRelatoTutorRequest request)
    {
        if (!await _animalRepositorio.ExisteAsync(request.AnimalId))
        {
            throw new RegraDeNegocioException($"Animal com Id {request.AnimalId} não existe.");
        }

        var relato = RelatoTutor.Criar(request.AnimalId, request.Texto, request.Sintomas, request.DataOcorrencia);

        await _relatoRepositorio.AdicionarAsync(relato);

        return RelatoTutorResponse.De(relato);
    }

    public async Task AtualizarAsync(string id, AtualizarRelatoTutorRequest request)
    {
        var relato = await ObterOuFalharAsync(id);

        relato.Atualizar(request.Texto, request.Sintomas, request.DataOcorrencia);

        await _relatoRepositorio.AtualizarAsync(relato);
    }

    public async Task RemoverAsync(string id)
    {
        var relato = await ObterOuFalharAsync(id);

        await _relatoRepositorio.RemoverAsync(relato);
    }

    private async Task<RelatoTutor> ObterOuFalharAsync(string id)
    {
        return await _relatoRepositorio.ObterPorIdAsync(id)
            ?? throw new RecursoNaoEncontradoException($"Relato com Id {id} não foi encontrado.");
    }
}
