using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Microsoft.Azure.Documents.Client;
using levitas.PoContract;
using System.Linq;
using Microsoft.Azure.Documents;

namespace levitas.CadastroDeAlunos;
public static class Alunos
{
    private const string databaseName = "Levitas";
    private const string containerName = "Alunos";
    private const string cosmosdbConnection = "CosmosDBConnection";
    //TODO: Implementar mascara de dados nos metadados
    [FunctionName("MetaData")]
    public static IActionResult MetaData(
        [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "v1/alunos/metadata")] HttpRequest req,
        ILogger log)
    {
        log.LogInformation("MetaData solicitado");
        var metaData = new AlunoMetaData();
        return new OkObjectResult(metaData);
    }
    [FunctionName("LoadMetaData")]
    public static IActionResult LoadMetaData(
        [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = "v1/alunos/load-metadata")] HttpRequest req,
        ILogger log)
    {
        log.LogInformation("LoadMetaData solicitado");
        var metaData = new AlunoLoadMetaData();
        return new OkObjectResult(metaData);
    }
    [FunctionName("CriarNovoAluno")]
    public static async Task<IActionResult> CriarNovoAluno(
        [HttpTrigger(AuthorizationLevel.Function, "post", Route = "v1/alunos")] HttpRequest req,
         [CosmosDB(
                databaseName: databaseName,
                collectionName: containerName,
                ConnectionStringSetting = cosmosdbConnection)] IAsyncCollector<Aluno> alunos,
        ILogger log)
    {

        log.LogInformation("CriarNovoAluno solicitado");

        string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
        dynamic data = JsonConvert.DeserializeObject(requestBody);

        var aluno = new Aluno
        {
            Id = Guid.NewGuid().ToString(),
            Nome = data?.nome,
            DataDeNascimento = data?.dataDeNascimento,
            NomeDoResponsavel = data?.nomeDoResponsavel,
            Telefone = data?.telefone,
            TemSkate = data?.temSkate
        };
        await alunos.AddAsync(aluno);

        return new CreatedResult($"/api/alunos/{aluno.Id}", aluno);
    }

    [FunctionName("ObterAluno")]
    public static IActionResult ObterAluno(
        [HttpTrigger(AuthorizationLevel.Function, "get", Route = "v1/alunos/{id}")] HttpRequest req,
        [CosmosDB(
                databaseName: databaseName,
                collectionName: containerName,
                ConnectionStringSetting = cosmosdbConnection,
                Id = "{id}",
                PartitionKey ="{id}")]  Aluno aluno,
        ILogger log,
        string id)
    {
        log.LogInformation("ObterAluno solicitado : {0}", id);
        if (aluno is null)
            return new NotFoundResult();


        return new OkObjectResult(aluno);
    }

    //TODO: Implementar paginação e método search, implentar filtros, refatorar
    [FunctionName("ObterAlunos")]
    public static IActionResult ObterAlunosAsync(
        [HttpTrigger(AuthorizationLevel.Function, "get", Route = "v1/alunos")] HttpRequest req,
        [CosmosDB(
            databaseName: databaseName,
            collectionName: containerName,
            ConnectionStringSetting = cosmosdbConnection)] DocumentClient client,
        ILogger log)
    {
        log.LogInformation("ObterAlunos solicitado");

        var poLookup = new PoLookup
        {
            page = req.Query.ContainsKey("page") ? int.Parse(req.Query["page"]) : 1,
            pageSize = req.Query.ContainsKey("pageSize") ? int.Parse(req.Query["pageSize"]) : 10,
            order = req.Query.ContainsKey("order") ? req.Query["order"].ToString() : string.Empty
        };


        var query = client.CreateDocumentQuery<Aluno>(
            UriFactory.CreateDocumentCollectionUri(databaseName, containerName),
            new FeedOptions { EnableCrossPartitionQuery = true })
            .AsQueryable();

        if (!string.IsNullOrEmpty(poLookup.order))
        {
            if (poLookup.order.StartsWith("-"))
            {

                query = query.OrderByDescending(x => x.Nome);
            }
            else
            {
                query = query.OrderBy(x => x.Nome);
            }
            query = query.OrderBy(x => x.Nome);
        }

        var skip = (poLookup.page - 1) * poLookup.pageSize;
        var take = poLookup.pageSize;
        query = query.Skip(skip).Take(take);

        var response = new PoSuccessResponseCollections<Aluno>() { Items = query.ToList() };
        return new OkObjectResult(response);
    }


    [FunctionName("DeletarAluno")]
    public static async Task<IActionResult> DeletarAluno(
           [HttpTrigger(AuthorizationLevel.Function, "delete", Route = "v1/alunos/{id}")] HttpRequest req,
           [CosmosDB(
            databaseName: databaseName,
            collectionName: containerName,
            ConnectionStringSetting = cosmosdbConnection)] DocumentClient client,
           ILogger log,
           string id)
    {
        log.LogInformation("Deletar Aluno solicitado: {0}", id);

        Uri documentUri = UriFactory.CreateDocumentUri(databaseName, containerName, id);
        if (documentUri is null)
            return new NotFoundResult();

        await client.DeleteDocumentAsync(documentUri, new RequestOptions { PartitionKey = new PartitionKey(id) });

        return new NoContentResult();
    }

    [FunctionName("AtualizarAluno")]
    public static async Task<IActionResult> AtualizarAluno(
       [HttpTrigger(AuthorizationLevel.Function, "put", Route = "v1/alunos/{id}")] HttpRequest req,
         [CosmosDB(
            databaseName: databaseName,
            collectionName: containerName,
            ConnectionStringSetting = cosmosdbConnection)] DocumentClient client,
       ILogger log,

       string id)
    {
        log.LogInformation("AtualizarAluno: {0}", id);

        Uri documentUri = UriFactory.CreateDocumentUri(databaseName, containerName, id);
        var alunoString = await req.ReadAsStringAsync();
        var aluno = JsonConvert.DeserializeObject<Aluno>(alunoString);

        await client.ReplaceDocumentAsync(documentUri, aluno);

        return new NoContentResult();
    }

    [FunctionName("UploadFoto")]
    public static async Task<IActionResult> UploadFotoDoAluno(
      [HttpTrigger(AuthorizationLevel.Function, "put", Route = "v1/alunos/{id}")] HttpRequest req,
        [CosmosDB(
            databaseName: databaseName,
            collectionName: containerName,
            ConnectionStringSetting = cosmosdbConnection)] DocumentClient client,
       [Blob("foto-perfil-aluno/{id}", FileAccess.Read, Connection = "AzureWebJobsStorage")] Stream myBlob,
      ILogger log,
      string id)
    {
        log.LogInformation("AtualizarAluno: {0}", id);

        var file = req.Form.Files[0];
        await file.CopyToAsync(myBlob);

        Uri documentUri = UriFactory.CreateDocumentUri(databaseName, containerName, id);
        var alunoString = await req.ReadAsStringAsync();
        var aluno = JsonConvert.DeserializeObject<Aluno>(alunoString);

        aluno.UrlFoto = id;

        await client.ReplaceDocumentAsync(documentUri, aluno);

        return new NoContentResult();
    }

}
