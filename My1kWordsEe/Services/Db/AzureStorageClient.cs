using Azure;
using Azure.Storage.Blobs;

using CSharpFunctionalExtensions;

namespace My1kWordsEe.Services.Db
{
    /// <summary>
    /// Facade for Azure blob storage API
    /// </summary>
    public partial class AzureStorageClient
    {
        public const string ApiSecretKey = "Secrets:AzureBlobConnectionString";

        private readonly IConfiguration config;
        private readonly ILogger logger;
        private readonly BlobServiceClient BlobServiceClient;

        public AzureStorageClient(
            IConfiguration config,
            ILogger<AzureStorageClient> logger)
        {
            this.config = config;
            this.logger = logger;
            this.BlobServiceClient = new BlobServiceClient(this.config[ApiSecretKey]);
        }

        public Uri AzureBlobEndpoint => this.BlobServiceClient.Uri;

        private async Task<Result<BlobContainerClient>> GetOrCreateContainer(string containerId)
        {
            try
            {
                var container = new BlobContainerClient(
                    this.config[ApiSecretKey],
                    containerId);
                await container.CreateIfNotExistsAsync();
                return container;
            }
            catch (RequestFailedException exception)
            {
                this.logger.LogError(exception, "Failure to get or create container {ContainerId}", containerId);
                return Result.Failure<BlobContainerClient>("Azure storage access error");
            }
        }

        private async Task<Result<Uri>> UploadStreamAsync(BlobClient blob, Stream stream)
        {
            try
            {
                await blob.UploadAsync(stream, overwrite: true);
                return blob.Uri;
            }
            catch (RequestFailedException exception)
            {
                this.logger.LogError(exception, "Failure to upload data to blob {name}", blob.Name);
                return Result.Failure<Uri>("Azure storage upload error");
            }
        }

        private async Task<Result<bool>> DeleteIfExistsAsync(BlobClient blob)
        {
            try
            {
                var response = await blob.DeleteIfExistsAsync();
                return response.Value;
            }
            catch (RequestFailedException exception)
            {
                this.logger.LogError(exception, "Failure to delete blob {name}", blob.Name);
                return Result.Failure<bool>("Failure to delete blob {name}");
            }
        }

        private static string JsonBlobName(string stringId) => stringId.ToLower() + ".json";
    }
}