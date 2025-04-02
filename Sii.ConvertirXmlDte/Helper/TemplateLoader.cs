using Azure.Storage.Blobs;

namespace Sii.ConvertirXmlDte.Helper;

public class TemplateLoader
{
    private readonly IConfiguration _config;
    private readonly BlobServiceClient _blobClient;

    public TemplateLoader(IConfiguration config, BlobServiceClient blobClient)
    {
        _config = config;
        _blobClient = blobClient;
    }

    public async Task<string> GetXsltTemplateAsync()
    {
        string containerName = _config.GetValue<string>("StorageConnection:containerName")!;
        string blobName = _config.GetValue<string>("StorageConnection:BlobName")!;

        BlobContainerClient containerClient = _blobClient.GetBlobContainerClient(containerName);
        BlobClient blobClient = containerClient.GetBlobClient(blobName);

        using MemoryStream ms = new();
        await blobClient.DownloadToAsync(ms);
        ms.Position = 0;

        using StreamReader reader = new(ms);
        return await reader.ReadToEndAsync();
    }
}
