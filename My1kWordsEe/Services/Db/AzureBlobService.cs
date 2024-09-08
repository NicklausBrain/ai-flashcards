using My1kWordsEe.Models;
using Azure.Storage.Blobs;
using System.Text.Json;

namespace My1kWordsEe.Services.Db
{
    public class AzureBlobService
    {
        private readonly string connectionString;

        public AzureBlobService(string connectionString)
        {
            this.connectionString = connectionString;
        }

        public async void SaveWordData(SampleWord word)
        {
            BlobContainerClient container = new BlobContainerClient(
                this.connectionString,
                "words");
            await container.CreateIfNotExistsAsync();

            // Get a reference to a blob
            BlobClient blob = container.GetBlobClient(word.EeWord);

            // Upload file data
            await blob.UploadAsync(
                new MemoryStream(JsonSerializer.SerializeToUtf8Bytes(word)),
                overwrite: true);
        }

        // static string BlobName(string word) => $"{word}.json";
    }
}
