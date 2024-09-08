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
            BlobClient blob = container.GetBlobClient(BlobName(word));

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
            BlobClient blob = container.GetBlobClient(BlobName(word.EeWord));

            // Upload file data
            await blob.UploadAsync(
                new MemoryStream(JsonSerializer.SerializeToUtf8Bytes(word)),
                overwrite: true);
        }

        private async Task<BlobContainerClient> GetWordsContainer()
        {
            BlobContainerClient container = new BlobContainerClient(
                this.connectionString,
                "words");
            await container.CreateIfNotExistsAsync();
            return container;
        }

        static string BlobName(string word) => word.ToLower() + ".json";
    }
}
