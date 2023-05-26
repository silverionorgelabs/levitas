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
using Microsoft.WindowsAzure.Storage;

namespace levitas.CadastroDeAlunos;
public static class Alunos
{
    private const string databaseName = "Levitas";
    private const string containerCosmosName = "Alunos";
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
                collectionName: containerCosmosName,
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
            TemSkate = data?.temSkate?.Value ?? false
        };
        await alunos.AddAsync(aluno);

        return new CreatedResult($"/api/alunos/{aluno.Id}", aluno);
    }

    [FunctionName("ObterAluno")]
    public static IActionResult ObterAluno(
        [HttpTrigger(AuthorizationLevel.Function, "get", Route = "v1/alunos/{id}")] HttpRequest req,
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
        [HttpTrigger(AuthorizationLevel.Function, "get", Route = "v1/alunos")] HttpRequest req,
        [CosmosDB(
            databaseName: databaseName,
            collectionName: containerCosmosName,
            ConnectionStringSetting = cosmosdbConnection)] DocumentClient client,
        ILogger log)
    {
        log.LogInformation("ObterAlunos solicitado");

        var poLookup = new PoLookup
        {
            page = req.Query.ContainsKey("page") ? int.Parse(req.Query["page"]) : 1,
            pageSize = req.Query.ContainsKey("pageSize") ? int.Parse(req.Query["pageSize"]) : 10,
            order = req.Query.ContainsKey("order") ? req.Query["order"].ToString() : string.Empty,
            search = req.Query.ContainsKey("search") ? req.Query["search"].ToString() : string.Empty,
            nome = req.Query.ContainsKey("nome") ? req.Query["nome"].ToString() : string.Empty,
            nomeDoResponsavel = req.Query.ContainsKey("nomeDoResponsavel") ? req.Query["nomeDoResponsavel"].ToString() : string.Empty
        };


        var query = client.CreateDocumentQuery<Aluno>(
            UriFactory.CreateDocumentCollectionUri(databaseName, containerCosmosName),
            new FeedOptions { EnableCrossPartitionQuery = true })
            .AsQueryable();

        if (!string.IsNullOrEmpty(poLookup.search))
        {
            log.LogInformation("ObterAlunos search: {0}", poLookup.search);
            query = query.Where(x => x.Nome.ToLower().Contains(poLookup.search.ToLower()));
        }

        
        if (!string.IsNullOrEmpty(poLookup.nome))
        {
            log.LogInformation("ObterAlunos name: {0}", poLookup.nome);
            query = query.Where(x => x.Nome.ToLower().Contains(poLookup.nome.ToLower()));
        }

        
        if (!string.IsNullOrEmpty(poLookup.nomeDoResponsavel))
        {
            log.LogInformation("ObterAlunos nome responsavel: {0}", poLookup.nomeDoResponsavel);
            query = query.Where(x => x.NomeDoResponsavel.ToLower().Contains(poLookup.nomeDoResponsavel.ToLower()));
        }

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
        var storageAccount = CloudStorageAccount.Parse(connectionString);
        var blobClient = storageAccount.CreateCloudBlobClient();

        var containerNameAutorizacaoDosPais = "autorizacao-dos-pais";
        var containerAutorizacaoDosPais = blobClient.GetContainerReference(containerNameAutorizacaoDosPais);
        var blobAutorizacaoDosPais = containerAutorizacaoDosPais.GetBlockBlobReference(Path.GetFileName(aluno.UrlTermoDeResponsabilidadeAssinado));
        await blobAutorizacaoDosPais.DeleteIfExistsAsync();

        var containerNameFotoPerfilAluno = "foto-perfil-aluno";
        var containerFotoPerfilAluno = blobClient.GetContainerReference(containerNameFotoPerfilAluno);
        var blobFotoPerfilAluno = containerFotoPerfilAluno.GetBlockBlobReference(Path.GetFileName(aluno.UrlFoto));
        await blobFotoPerfilAluno.DeleteIfExistsAsync();

        return new NoContentResult();
    }

    [FunctionName("AtualizarAluno")]
    public static async Task<IActionResult> AtualizarAluno(
       [HttpTrigger(AuthorizationLevel.Function, "put", Route = "v1/alunos/{id}")] HttpRequest req,
         [CosmosDB(
            databaseName: databaseName,
            collectionName: containerCosmosName,
            ConnectionStringSetting = cosmosdbConnection)] DocumentClient client,
       ILogger log,

       string id)
    {
        log.LogInformation("AtualizarAluno: {0}", id);

        Uri documentUri = UriFactory.CreateDocumentUri(databaseName, containerCosmosName, id);
        var alunoString = await req.ReadAsStringAsync();
        var aluno = JsonConvert.DeserializeObject<Aluno>(alunoString);

        await client.ReplaceDocumentAsync(documentUri, aluno);

        return new NoContentResult();
    }

    [FunctionName("UploadImage")]
    public static async Task<IActionResult> UploadFotoDoAluno(
        [HttpTrigger(AuthorizationLevel.Function, "post", Route = "v1/alunos/{id}")] HttpRequest req,
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
        var containerBlobName = "foto-perfil-aluno";

        var storageAccount = CloudStorageAccount.Parse(connectionString);
        var blobClient = storageAccount.CreateCloudBlobClient();
        var container = blobClient.GetContainerReference(containerBlobName);

        await container.CreateIfNotExistsAsync();

        var blobName = id + Path.GetExtension(imageFile.FileName);
        var blob = container.GetBlockBlobReference(blobName);

        using (var stream = imageFile.OpenReadStream())
            await blob.UploadFromStreamAsync(stream);

        var response = new PoUploadResponse()
        {
            FileName = blob.Uri.ToString(),
            uniqueId = id,
            uploadedDate = DateTime.Now,
            PoUploadStatus = PoUploadStatus.Done
        };

        Uri documentUri = UriFactory.CreateDocumentUri(databaseName, containerCosmosName, id);

        log.LogInformation("Uri do aluno: {0}", documentUri);

        var documentoAluno = await client.ReadDocumentAsync<Aluno>(documentUri, new RequestOptions { PartitionKey = new PartitionKey(id) });

        documentoAluno.Document.UrlFoto = response.FileName;
        await client.ReplaceDocumentAsync(documentUri, documentoAluno.Document);

        return new OkObjectResult(response);
    }
    [FunctionName("UploadTermoDeResponsabilidade")]
    public static async Task<IActionResult> UploadTermoDeResponsabilidade(
           [HttpTrigger(AuthorizationLevel.Function, "post", Route = "v1/alunos/{id}/termo-de-responsabilidade")] HttpRequest req,
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
        var containerBlobName = "autorizacao-dos-pais";

        var storageAccount = CloudStorageAccount.Parse(connectionString);
        var blobClient = storageAccount.CreateCloudBlobClient();
        var container = blobClient.GetContainerReference(containerBlobName);

        await container.CreateIfNotExistsAsync();

        var blobName = id + Path.GetExtension(imageFile.FileName);
        var blob = container.GetBlockBlobReference(blobName);

        using (var stream = imageFile.OpenReadStream())
            await blob.UploadFromStreamAsync(stream);

        var response = new PoUploadResponse()
        {
            FileName = blob.Uri.ToString(),
            uniqueId = id,
            uploadedDate = DateTime.Now,
            PoUploadStatus = PoUploadStatus.Done
        };

        Uri documentUri = UriFactory.CreateDocumentUri(databaseName, containerCosmosName, id);

        log.LogInformation("Uri do aluno: {0}", documentUri);

        var documentoAluno = await client.ReadDocumentAsync<Aluno>(documentUri, new RequestOptions { PartitionKey = new PartitionKey(id) });

        documentoAluno.Document.UrlTermoDeResponsabilidadeAssinado = response.FileName;
        await client.ReplaceDocumentAsync(documentUri, documentoAluno.Document);

        return new OkObjectResult(response);
    }



}
