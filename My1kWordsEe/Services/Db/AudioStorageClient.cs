using Azure.Storage.Blobs;

using CSharpFunctionalExtensions;

using static My1kWordsEe.Models.Conventions;

namespace My1kWordsEe.Services.Db
{
    public class AudioStorageClient
    {
        private readonly AzureStorageClient azureStorageClient;

        public AudioStorageClient(AzureStorageClient azureStorageClient)
        {
            this.azureStorageClient = azureStorageClient;
        }

        public Task<Result<bool>> DeleteAudio(string blobName) =>
            this.GetAudioContainer().Bind((container) =>
            this.azureStorageClient.DeleteIfExistsAsync(container.GetBlobClient(blobName)));

        public Task<Result<Uri>> SaveAudio(Stream audioStream, string blobName) =>
            this.GetAudioContainer().Bind((container) =>
            this.azureStorageClient.UploadStreamAsync(
                container.GetBlobClient(blobName),
                audioStream));

        private Task<Result<BlobContainerClient>> GetAudioContainer() =>
            this.azureStorageClient.GetOrCreateContainer(AudioContainer);
    }
}