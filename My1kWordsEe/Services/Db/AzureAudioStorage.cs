using Azure.Storage.Blobs;

using CSharpFunctionalExtensions;

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

        public Task<Result<Uri>> SaveAudio(Stream audioStream) =>
            this.SaveAudio(audioStream, WavBlobName());

        private Task<Result<BlobContainerClient>> GetAudioContainer() =>
            this.GetOrCreateContainer("audio");

        private static string WavBlobName() => Guid.NewGuid() + ".wav";
    }
}

