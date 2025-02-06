using Azure.Storage.Blobs;

using CSharpFunctionalExtensions;

using static My1kWordsEe.Models.Conventions;

namespace My1kWordsEe.Services.Db
{
    public partial class AzureStorageClient
    {
        public Task<Result<bool>> DeleteAudio(string blobName) =>
            this.GetAudioContainer().Bind((container) =>
            this.DeleteIfExistsAsync(container.GetBlobClient(blobName)));

        public Task<Result<Uri>> SaveAudio(Stream audioStream, string blobName) =>
            this.GetAudioContainer().Bind((container) =>
            this.UploadStreamAsync(
                container.GetBlobClient(blobName),
                audioStream));

        private Task<Result<BlobContainerClient>> GetAudioContainer() =>
            this.GetOrCreateContainer(AudioContainer);
    }
}