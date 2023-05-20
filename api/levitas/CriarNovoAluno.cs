using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace levitas
{
    public class Aluno
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("partitionKey")]
        public string AlunoPartitionKeyValue { get; set; }= "Aluno";
        public string Nome { get; set; }
        public DateTime DataDeNascimento { get; set; }
        public int Idade => DateTime.Now.Year - DataDeNascimento.Year;
        public string Telefone { get; set; }
        public string NomeDoResponsavel { get; set; }
        public bool TemSkate { get; set; }
        public string UrlFoto { get; set; }
        public string UrlTermoDeResponsabilidadeAssinado { get; set; }

    }
    public static class CriarNovoAluno
    {
        [FunctionName("CriarNovoAluno")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = "alunos")] HttpRequest req,
             [CosmosDB(
                databaseName: "Levitas",
                collectionName: "Alunos",
                ConnectionStringSetting = "CosmosDBConnection")] IAsyncCollector<Aluno> alunos,
            ILogger log)
        {


            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            dynamic data = JsonConvert.DeserializeObject(requestBody);
            var aluno = new Aluno
            {
                Id = Guid.NewGuid().ToString(),
                Nome = data?.nome,
                DataDeNascimento = data?.dataDeNascimento,
                NomeDoResponsavel = data?.nomeDoResponsavel,
                TemSkate = data?.temSkate
            };
            await alunos.AddAsync(aluno);

            return new CreatedResult($"/api/alunos/{aluno.Id}", aluno);
        }
    }
}
