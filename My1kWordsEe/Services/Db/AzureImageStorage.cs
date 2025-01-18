using Azure.Storage.Blobs;

using CSharpFunctionalExtensions;

namespace My1kWordsEe.Services.Db
{
    public partial class AzureStorageClient
    {
        public Task<Result<bool>> DeleteImage(string blobName) =>
            this.GetImageContainer().Bind((container) =>
            this.DeleteIfExistsAsync(container.GetBlobClient(blobName)));

        public Task<Result<Uri>> SaveImage(Stream imageStream) =>
            this.GetImageContainer().Bind((container) =>
            this.UploadStreamAsync(
                container.GetBlobClient(JpgBlobName()),
                imageStream));

        private Task<Result<BlobContainerClient>> GetImageContainer() =>
            this.GetOrCreateContainer("image");

        private static string JpgBlobName() => Guid.NewGuid() + ".jpg";
    }
}