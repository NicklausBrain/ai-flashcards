using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Unicode;

using Azure.Storage.Blobs;

using CSharpFunctionalExtensions;

using My1kWordsEe.Models.Semantics;

using static My1kWordsEe.Models.Conventions;

namespace My1kWordsEe.Services.Db
{
    public partial class WordStorageClient
    {
        private readonly AzureStorageClient azureStorageClient;

        public WordStorageClient(AzureStorageClient azureStorageClient)
        {
            this.azureStorageClient = azureStorageClient;
        }

        public Task<Result<Maybe<EtWord>>> GetEtWordData(string word) =>
            this.GetEtWordsContainer().Bind(container =>
            this.azureStorageClient.DownloadJsonAsync<EtWord>(
                container.GetBlobClient($"{word}.{JsonFormat}")));

        public Task<Result<Uri>> SaveEtWordData(EtWord word) =>
            this.GetEtWordsContainer().Bind(container =>
            this.azureStorageClient.UploadJsonAsync(
                container.GetBlobClient($"{word.Value}.{JsonFormat}"),
                new MemoryStream(JsonSerializer.SerializeToUtf8Bytes(word, options: new JsonSerializerOptions
                {
                    Encoder = JavaScriptEncoder.Create(UnicodeRanges.All),
                    WriteIndented = false
                }))));

        private Task<Result<BlobContainerClient>> GetEtWordsContainer() =>
            this.azureStorageClient.GetOrCreateContainer("et-word");
    }
}