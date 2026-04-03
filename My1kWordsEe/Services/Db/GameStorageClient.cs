using Azure.Storage.Blobs;

using CSharpFunctionalExtensions;

using static My1kWordsEe.Models.Conventions;

namespace My1kWordsEe.Services.Db
{
    public class GameStorageClient
    {
        private readonly AzureStorageClient azureStorageClient;

        public GameStorageClient(AzureStorageClient azureStorageClient)
        {
            this.azureStorageClient = azureStorageClient;
        }

        public virtual Task<Result<Maybe<T>>> GetGameData<T>(string gameId) where T : class =>
            this.GetGameContainer().Bind(container =>
            this.azureStorageClient.DownloadJsonAsync<T>(
                container.GetBlobClient($"{gameId}.{JsonFormat}")));

        public virtual Task<Result<Uri>> SaveGameData<T>(string gameId, T gameData) where T : class =>
            this.GetGameContainer().Bind(container =>
            this.azureStorageClient.UploadJsonAsync<T>(
                blob: container.GetBlobClient($"{gameId}.{JsonFormat}"),
                record: gameData));

        private Task<Result<BlobContainerClient>> GetGameContainer() =>
            this.azureStorageClient.GetOrCreateContainer("cached-games");
    }
}
