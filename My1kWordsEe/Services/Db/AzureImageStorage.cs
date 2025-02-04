using System.Text;

using Azure.Storage.Blobs;

using CSharpFunctionalExtensions;

namespace My1kWordsEe.Services.Db
{
    public partial class AzureStorageClient
    {
        public Task<Result<bool>> DeleteImage(string blobName) =>
            this.GetImageContainer().Bind((container) =>
            this.DeleteIfExistsAsync(container.GetBlobClient(blobName)));

        public Task<Result<Uri>> SaveImage(string prompt, MemoryStream imageStream) =>
            this.GetImageContainer().Bind((container) =>
            {
                var blobId = Guid.NewGuid();
                Task.Run(() =>
                {
                    return this.UploadStreamAsync(
                        container.GetBlobClient($"{blobId}.txt"),
                        new MemoryStream(Encoding.UTF8.GetBytes(prompt)));
                });
                return this.UploadStreamAsync(
                    container.GetBlobClient($"{blobId}.jpg"),
                    imageStream);
            }
            );

        private Task<Result<BlobContainerClient>> GetImageContainer() =>
            this.GetOrCreateContainer("image");
    }
}