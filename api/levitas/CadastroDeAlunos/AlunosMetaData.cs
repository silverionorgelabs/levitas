using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;

namespace levitas.CadastroDeAlunos;
public class AlunosMetaData
{
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
}
