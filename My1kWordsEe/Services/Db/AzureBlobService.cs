using System.Text.Json;

using Azure.Storage.Blobs;

using CSharpFunctionalExtensions;

using My1kWordsEe.Models;

namespace My1kWordsEe.Services.Db
{
    public class AzureBlobService
    {
        private readonly string connectionString;

        public AzureBlobService(string connectionString)
        {
            this.connectionString = connectionString;
        }

        public async Task<Result<SampleWord>> GetWordData(string word)
        {
            BlobContainerClient container = await GetWordsContainer();
            BlobClient blob = container.GetBlobClient(JsonBlobName(word));

            if (await blob.ExistsAsync())
            {
                var response = await blob.DownloadContentAsync();
                if (response != null && response.HasValue)
                {
                    var sampleWord = JsonSerializer.Deserialize<SampleWord>(response.Value.Content);

                    if (sampleWord != null)
                    {
                        return Result.Success(sampleWord);
                    }
                }
            }

            return Result.Failure<SampleWord>($"Word '{word}' is not recorded");
        }

        public async Task SaveWordData(SampleWord word)
        {
            BlobContainerClient container = await GetWordsContainer();

            // Get a reference to a blob
            BlobClient blob = container.GetBlobClient(JsonBlobName(word.EeWord));

            // Upload file data
            await blob.UploadAsync(
                new MemoryStream(JsonSerializer.SerializeToUtf8Bytes(word)),
                overwrite: true);
        }

        public async Task<Uri> SaveAudio(Stream audioStream)
        {
            BlobContainerClient container = await GetAudioContainer();
            BlobClient blob = container.GetBlobClient(WavBlobName());
            await blob.UploadAsync(audioStream);
            return blob.Uri;
        }

        public async Task<Uri> SaveImage(Stream imageStream)
        {
            BlobContainerClient container = await GetImageContainer();
            BlobClient blob = container.GetBlobClient(JpgBlobName());
            await blob.UploadAsync(imageStream);
            return blob.Uri;
        }


        private async Task<BlobContainerClient> GetWordsContainer() => await this.GetContainer("words");

        private async Task<BlobContainerClient> GetAudioContainer() => await this.GetContainer("audio");

        private async Task<BlobContainerClient> GetImageContainer() => await this.GetContainer("image");

        private async Task<BlobContainerClient> GetContainer(string containerId)
        {
            BlobContainerClient container = new BlobContainerClient(
                            this.connectionString,
                            containerId);
            await container.CreateIfNotExistsAsync();
            return container;
        }

        static string JsonBlobName(string word) => word.ToLower() + ".json";

        static string WavBlobName() => Guid.NewGuid() + ".wav";

        static string JpgBlobName() => Guid.NewGuid() + ".jpg";
    }
}
