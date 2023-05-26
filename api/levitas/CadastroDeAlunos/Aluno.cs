using System;
using System.Runtime.Serialization;
using levitas.PoContract;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace levitas.CadastroDeAlunos;
[Serializable]
public class Aluno
{
    [JsonProperty("id")]
    public string Id { get; set; }
    public string Nome { get; set; }
    [JsonConverter(typeof(PoDateTimeConverter))]
    [JsonProperty("dataDeNascimento")]
    public DateTime DataDeNascimento { get; set; }
    public int Idade => DateTime.Now.Year - DataDeNascimento.Year;
    public string Telefone { get; set; }
    public string NomeDoResponsavel { get; set; }
    public bool TemSkate { get; set; }
    public string UrlFoto { get; set; }
    public string UrlTermoDeResponsabilidadeAssinado { get; set; }

    public static Aluno Create(dynamic data)
    => new Aluno
    {
        Id = Guid.NewGuid().ToString(),
        Nome = data?.nome,
        DataDeNascimento = data?.dataDeNascimento,
        NomeDoResponsavel = data?.nomeDoResponsavel,
        Telefone = data?.telefone,
        TemSkate = data?.temSkate?.Value ?? false
    };

}
