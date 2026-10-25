using DotNet.Domain.Excecoes;

namespace DotNet.Domain.Entidades;

public class RelatoTutor
{
    public const int TamanhoMaximoTexto = 2000;

    public string Id { get; set; } = string.Empty;

    public int AnimalId { get; set; }

    public string Texto { get; set; } = string.Empty;

    public List<string> Sintomas { get; set; } = [];

    public DateTime DataOcorrencia { get; set; }

    public DateTime CriadoEm { get; set; } = DateTime.UtcNow;

    public static RelatoTutor Criar(int animalId, string texto, IEnumerable<string>? sintomas, DateTime dataOcorrencia)
    {
        Validar(animalId, texto, dataOcorrencia);

        return new RelatoTutor
        {
            AnimalId = animalId,
            Texto = texto.Trim(),
            Sintomas = NormalizarSintomas(sintomas),
            DataOcorrencia = dataOcorrencia,
            CriadoEm = DateTime.UtcNow
        };
    }

    public void Atualizar(string texto, IEnumerable<string>? sintomas, DateTime dataOcorrencia)
    {
        Validar(AnimalId, texto, dataOcorrencia);

        Texto = texto.Trim();
        Sintomas = NormalizarSintomas(sintomas);
        DataOcorrencia = dataOcorrencia;
    }

    private static void Validar(int animalId, string texto, DateTime dataOcorrencia)
    {
        if (animalId <= 0)
        {
            throw new RegraDeNegocioException("AnimalId é obrigatório.");
        }

        if (string.IsNullOrWhiteSpace(texto))
        {
            throw new RegraDeNegocioException("O texto do relato é obrigatório.");
        }

        if (texto.Trim().Length > TamanhoMaximoTexto)
        {
            throw new RegraDeNegocioException($"O texto do relato deve ter no máximo {TamanhoMaximoTexto} caracteres.");
        }

        if (dataOcorrencia.Date > DateTime.UtcNow.Date)
        {
            throw new RegraDeNegocioException("A data de ocorrência não pode estar no futuro.");
        }
    }

    private static List<string> NormalizarSintomas(IEnumerable<string>? sintomas) =>
        sintomas?
            .Where(s => !string.IsNullOrWhiteSpace(s))
            .Select(s => s.Trim().ToLowerInvariant())
            .Distinct()
            .ToList() ?? [];
}