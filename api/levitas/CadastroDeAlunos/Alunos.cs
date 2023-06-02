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
using levitas.CadastroDeAlunos.Infra;

namespace levitas.CadastroDeAlunos;
public static partial class Alunos
{
    private const string databaseName = "Levitas";
    private const string containerCosmosName = "Alunos";
    private const string cosmosdbConnection = "CosmosDBConnection";
    private const string containerNameAutorizacaoDosPais = "autorizacao-dos-pais";
    private const string containerNameFotoPerfilAluno = "foto-perfil-aluno";

    [FunctionName("CriarNovoAluno")]
    public static async Task<IActionResult> CriarNovoAluno(
        [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "v1/alunos")] HttpRequest req,
         [CosmosDB(
                databaseName: databaseName,
                collectionName: containerCosmosName,
                ConnectionStringSetting = cosmosdbConnection)] IAsyncCollector<Aluno> alunos,
        ILogger log)
    {
        log.LogInformation("CriarNovoAluno solicitado");

        string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
        dynamic data = JsonConvert.DeserializeObject(requestBody);
        Aluno aluno = Aluno.Create(data);
        if (string.IsNullOrEmpty(aluno.Nome) || aluno.Nome.Length < 3)
            return new BadRequestObjectResult(new PoMessage()
            {
                Code = "NomeInvalido",
                Message = "Nome do aluno é obrigatório e deve ter no mínimo 3 caracteres"
            });

        if (string.IsNullOrEmpty(aluno.NomeDoResponsavel) || aluno.NomeDoResponsavel.Length < 3)
            return new BadRequestObjectResult(new PoMessage()
            {
                Code = "NomeDoResponsavelInvalido",
                Message = "Nome do responsável é obrigatório e deve ter no mínimo 3 caracteres"
            });

        await alunos.AddAsync(aluno);

        return new CreatedResult($"/api/alunos/{aluno.Id}", aluno);
    }

    [FunctionName("ObterAluno")]
    public static IActionResult ObterAluno(
        [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "v1/alunos/{id}")] HttpRequest req,
        [CosmosDB(
                databaseName: databaseName,
                collectionName: containerCosmosName,
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

    [FunctionName("ObterAlunos")]
    public static IActionResult ObterAlunosAsync(
        [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "v1/alunos")] HttpRequest req,
        [CosmosDB(
            databaseName: databaseName,
            collectionName: containerCosmosName,
            ConnectionStringSetting = cosmosdbConnection)] DocumentClient client,
        ILogger log)
    {
        log.LogInformation("ObterAlunos solicitado");

        var poLookup = AlunoLookup.Create(req);
        var query = client.CreateDocumentQuery<Aluno>(
            UriFactory.CreateDocumentCollectionUri(databaseName, containerCosmosName),
            new FeedOptions { EnableCrossPartitionQuery = true })
            .AsQueryable();
        query = poLookup.BuildQuery(query);

        var response = new PoSuccessResponseCollections<Aluno>() { Items = query.ToList() };

        return new OkObjectResult(response);
    }


    [FunctionName("DeletarAluno")]
    public static async Task<IActionResult> DeletarAluno(
           [HttpTrigger(AuthorizationLevel.Anonymous, "delete", Route = "v1/alunos/{id}")] HttpRequest req,
           [CosmosDB(
            databaseName: databaseName,
            collectionName: containerCosmosName,
            ConnectionStringSetting = cosmosdbConnection)] DocumentClient client,
              [CosmosDB(
                databaseName: databaseName,
                collectionName: containerCosmosName,
                ConnectionStringSetting = cosmosdbConnection,
                Id = "{id}",
                PartitionKey ="{id}")]  Aluno aluno,
           ILogger log,
           string id)
    {
        log.LogInformation("Deletar Aluno solicitado: {0}", id);

        Uri documentUri = UriFactory.CreateDocumentUri(databaseName, containerCosmosName, id);
        if (documentUri is null)
            return new NotFoundResult();

        await client.DeleteDocumentAsync(documentUri, new RequestOptions { PartitionKey = new PartitionKey(id) });

        var connectionString = Environment.GetEnvironmentVariable("AzureWebJobsStorage");
        var storage = new Storage(connectionString);

        if (!string.IsNullOrEmpty(aluno.UrlTermoDeResponsabilidadeAssinado))
            await storage.DeletarArquivoDoStorage(
                blobName: Path.GetFileName(aluno.UrlTermoDeResponsabilidadeAssinado),
                containerName: containerNameAutorizacaoDosPais);

        if (!string.IsNullOrEmpty(aluno.UrlFoto))
            await storage.DeletarArquivoDoStorage(
               blobName: Path.GetFileName(aluno.UrlFoto),
               containerName: containerNameAutorizacaoDosPais);

        return new NoContentResult();
    }



    [FunctionName("AtualizarAluno")]
    public static async Task<IActionResult> AtualizarAluno(
       [HttpTrigger(AuthorizationLevel.Anonymous, "put", Route = "v1/alunos/{id}")] HttpRequest req,
         [CosmosDB(
            databaseName: databaseName,
            collectionName: containerCosmosName,
            ConnectionStringSetting = cosmosdbConnection)] DocumentClient client,
       ILogger log,

       string id)
    {
        log.LogInformation("AtualizarAluno: {0}", id);

        var alunoString = await req.ReadAsStringAsync();
        var aluno = JsonConvert.DeserializeObject<Aluno>(alunoString);
        Uri documentUri = UriFactory.CreateDocumentUri(databaseName, containerCosmosName, id);

        await client.ReplaceDocumentAsync(documentUri, aluno);

        return new NoContentResult();
    }

    [FunctionName("Upload-Foto-Aluno")]
    public static async Task<IActionResult> UploadFotoDoAluno(
        [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "v1/alunos/{id}")] HttpRequest req,
        [CosmosDB(
            databaseName: databaseName,
            collectionName: containerCosmosName,
            ConnectionStringSetting = cosmosdbConnection)] DocumentClient client,
        string id,
        ILogger log)
    {
        log.LogInformation("UploadImage solicitado: {0}", id);

        var imageFile = req.Form.Files["files"];
        var connectionString = Environment.GetEnvironmentVariable("AzureWebJobsStorage");
        var storage = new Storage(connectionString);

        var uri = await storage.Upload(containerNameFotoPerfilAluno, imageFile.FileName, imageFile);

        var response = PoUploadResponse.Success(uri, id);

        Uri documentUri = UriFactory.CreateDocumentUri(databaseName, containerCosmosName, id);

        log.LogInformation("Uri do aluno: {0}", documentUri);

        var documentoAluno = await client.ReadDocumentAsync<Aluno>(documentUri, new RequestOptions { PartitionKey = new PartitionKey(id) });

        documentoAluno.Document.UrlFoto = response.FileName;
        await client.ReplaceDocumentAsync(documentUri, documentoAluno.Document);

        return new OkObjectResult(response);
    }
    [FunctionName("UploadTermoDeResponsabilidade")]
    public static async Task<IActionResult> UploadTermoDeResponsabilidade(
           [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "v1/alunos/{id}/termo-de-responsabilidade")] HttpRequest req,
           [CosmosDB(
            databaseName: databaseName,
            collectionName: containerCosmosName,
            ConnectionStringSetting = cosmosdbConnection)] DocumentClient client,
           string id,
           ILogger log)
    {
        log.LogInformation("Upload termo de responsabilidade solicitado: {0}", id);

        var imageFile = req.Form.Files["files"];
        var connectionString = Environment.GetEnvironmentVariable("AzureWebJobsStorage");
        var storage = new Storage(connectionString);

        var uri = await storage.Upload(containerNameFotoPerfilAluno, imageFile.FileName, imageFile);

        var response = PoUploadResponse.Success(uri, id);

        Uri documentUri = UriFactory.CreateDocumentUri(databaseName, containerCosmosName, id);

        log.LogInformation("Uri do aluno: {0}", documentUri);
        var documentoAluno = await client.ReadDocumentAsync<Aluno>(documentUri, new RequestOptions { PartitionKey = new PartitionKey(id) });

        documentoAluno.Document.UrlTermoDeResponsabilidadeAssinado = response.FileName;
        await client.ReplaceDocumentAsync(documentUri, documentoAluno.Document);

        return new OkObjectResult(response);
    }

}