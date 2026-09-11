using Application.Interfaces.Services;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

public class AzureBlobStorageService : IFileService
{
    private readonly BlobContainerClient _containerClient;
    private readonly FileExtensionContentTypeProvider _contentTypeProvider;
    private readonly ILogger<AzureBlobStorageService> _logger;

    public AzureBlobStorageService(
        IConfiguration configuration,
        ILogger<AzureBlobStorageService> logger)
    {
        _logger = logger;
        _contentTypeProvider = new FileExtensionContentTypeProvider();

        var connectionString = configuration["AzureBlobStorage"];
        if (string.IsNullOrWhiteSpace(connectionString))
        {
            throw new InvalidOperationException("Azure Blob Storage connection string is missing.");
        }


        var containerName = configuration["StorageContainerName"];

        if (string.IsNullOrWhiteSpace(containerName))
        {
            throw new InvalidOperationException("Azure Blob Storage container name is missing.");
        }

        var blobServiceClient = new BlobServiceClient(connectionString);
        _containerClient = blobServiceClient.GetBlobContainerClient(containerName);
    }

    public async Task<string> UploadFileAsync(IFormFile? file, string folderName, string? existingFileUrl = null)
    {
        if (file == null || file.Length == 0)
            return string.Empty;

        string blobPath;

        if (!string.IsNullOrWhiteSpace(existingFileUrl) && Uri.TryCreate(existingFileUrl, UriKind.Absolute, out var existingUri))
        {
            blobPath = GetBlobNameFromUri(existingUri);
        }
        else
        {
            var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
            var uniqueName = $"{Guid.NewGuid()}{extension}";
            var cleanFolderName = folderName?.Trim('/') ?? string.Empty;

            blobPath = string.IsNullOrEmpty(cleanFolderName)
                ? uniqueName
                : $"{cleanFolderName}/{uniqueName}";
        }

        var blobClient = _containerClient.GetBlobClient(blobPath);

        using var stream = file.OpenReadStream();

        await blobClient.UploadAsync(stream, new BlobUploadOptions
        {
            HttpHeaders = new BlobHttpHeaders
            {
                ContentType = GetContentType(file.FileName)
            }
        });

        return blobClient.Uri.ToString();
    }

    public async Task DeleteFileAsync(string fileUrl)
    {
        if (string.IsNullOrWhiteSpace(fileUrl))
            return;

        if (!Uri.TryCreate(fileUrl, UriKind.Absolute, out var uri))
            return;

        try
        {
            var blobPath = GetBlobNameFromUri(uri);
            var blobClient = _containerClient.GetBlobClient(blobPath);
            await blobClient.DeleteIfExistsAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to delete blob from URL: {FileUrl}", fileUrl);
        }
    }

    private string GetBlobNameFromUri(Uri uri)
    {
        var uriBuilder = new BlobUriBuilder(uri);
        return uriBuilder.BlobName;
    }

    private string GetContentType(string fileName)
    {
        if (!_contentTypeProvider.TryGetContentType(fileName, out var contentType))
        {
            contentType = "application/octet-stream";
        }
        return contentType;
    }
}