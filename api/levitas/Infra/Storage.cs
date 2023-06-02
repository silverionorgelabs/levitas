using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;

namespace levitas.CadastroDeAlunos.Infra;
public class Storage
{
    public string ConnectionString { get; set; }
    public CloudStorageAccount CloudStorageAccount { get; set; }
    public CloudBlobClient CloudBlobClient { get; set; }
    public Storage(string connectionString)
    {
        ConnectionString = connectionString;
        CloudStorageAccount = CloudStorageAccount.Parse(connectionString);
        CloudBlobClient = CloudStorageAccount.CreateCloudBlobClient();
    }

    public async Task DeletarArquivoDoStorage(string blobName, string containerName)
    {
        var container = CloudBlobClient.GetContainerReference(containerName);
        var blob = container.GetBlockBlobReference(blobName);
        await blob.DeleteIfExistsAsync();
    }

    public async Task<string> Upload(string containerName, string name, IFormFile file)
    {

        var container = CloudBlobClient.GetContainerReference(containerName);

        await container.CreateIfNotExistsAsync();

        var blobName = name + Path.GetExtension(file.FileName);
        var blob = container.GetBlockBlobReference(blobName);

        using (var stream = file.OpenReadStream())
            await blob.UploadFromStreamAsync(stream);

        return blob.Uri.ToString();
    }
}