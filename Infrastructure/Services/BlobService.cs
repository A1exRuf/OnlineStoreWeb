using Application.Abstractions;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Infrastructure.Exceptions;
using Microsoft.Extensions.Configuration;

namespace Infrastructure.Services;

public class BlobService(
    BlobServiceClient blobServiceClient,
    IConfiguration configuration) : IBlobService
{
    private string _сontainerName = configuration["BlobStorage:ContainerName"]!;
    private string _host = configuration["BlobStorage:Host"]!;

    public async Task<Guid> UploadAsync(
        Stream stream, 
        string contentType, 
        CancellationToken cancellationToken = default)
    {
        BlobContainerClient containerClient = blobServiceClient.GetBlobContainerClient(_сontainerName);

        await containerClient.CreateIfNotExistsAsync();
        await containerClient.SetAccessPolicyAsync(PublicAccessType.Blob);

        Guid imageId = Guid.NewGuid();
        BlobClient blobClient = containerClient.GetBlobClient(imageId.ToString());

        await blobClient.UploadAsync(
            stream,
            new BlobHttpHeaders { ContentType = contentType },
            cancellationToken: cancellationToken);

        return imageId;
    }

    public async Task<Uri> GetUriAsync(Guid id, CancellationToken cancellationToken = default)
    {
        BlobContainerClient containerClient = blobServiceClient.GetBlobContainerClient(_сontainerName);

        BlobClient blobClient = containerClient.GetBlobClient(id.ToString());

        if (!await blobClient.ExistsAsync(cancellationToken))
            throw new ImageNotFoundException(id);

        var uriBuilder = new UriBuilder(blobClient.Uri)
        {
            Host = _host,
        };

        return uriBuilder.Uri;
    }

    public async Task DeleteAsync(
        Guid id, 
        CancellationToken cancellationToken = default)
    {
        BlobContainerClient containerClient = blobServiceClient.GetBlobContainerClient(_сontainerName);

        BlobClient blobClient = containerClient.GetBlobClient(id.ToString());

        await blobClient.DeleteAsync(cancellationToken: cancellationToken);
    }
}
