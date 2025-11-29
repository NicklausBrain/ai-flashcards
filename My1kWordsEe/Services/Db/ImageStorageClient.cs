using System.Text;

using Azure.Storage.Blobs;

using CSharpFunctionalExtensions;

using static My1kWordsEe.Models.Conventions;

namespace My1kWordsEe.Services.Db
{
    public class ImageStorageClient
    {
        private readonly AzureStorageClient azureStorageClient;

        public ImageStorageClient(AzureStorageClient azureStorageClient)
        {
            this.azureStorageClient = azureStorageClient;
        }

        public Task<Result<bool>> DeleteImage(string blobName) =>
            this.GetImageContainer().Bind((container) =>
            this.azureStorageClient.DeleteIfExistsAsync(container.GetBlobClient(blobName)));

        public Task<Result<Uri>> SaveImage(Guid sampleId, string prompt, MemoryStream imageStream) =>
            this.GetImageContainer().Bind((container) =>
            {
                Task.Run(() =>
                {
                    return this.azureStorageClient.UploadStreamAsync(
                        container.GetBlobClient($"{sampleId}.{TextFormat}"),
                        new MemoryStream(Encoding.UTF8.GetBytes(prompt)));
                });
                return this.azureStorageClient.UploadStreamAsync(
                    container.GetBlobClient($"{sampleId}.{ImageFormat}"),
                    imageStream);
            }
            );

        private Task<Result<BlobContainerClient>> GetImageContainer() =>
            this.azureStorageClient.GetOrCreateContainer(ImageContainer);
    }
}