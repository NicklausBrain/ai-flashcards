using Azure.Storage.Blobs;

using CSharpFunctionalExtensions;

using My1kWordsEe.Feature.Samples;

using static My1kWordsEe.Common.Conventions;

namespace My1kWordsEe.Feature.Words
{
    public class WordStorageClient
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
                blob: container.GetBlobClient($"{word.Value}.{JsonFormat}"),
                record: word));

        private Task<Result<BlobContainerClient>> GetEtWordsContainer() =>
            this.azureStorageClient.GetOrCreateContainer("et-word");
    }
}