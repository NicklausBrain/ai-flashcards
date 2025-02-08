using Azure.Storage.Blobs;

using CSharpFunctionalExtensions;

using My1kWordsEe.Models;

using static My1kWordsEe.Models.Conventions;

namespace My1kWordsEe.Services.Db
{
    public class FavoritesStorageClient
    {
        private readonly AzureStorageClient azureStorageClient;

        public FavoritesStorageClient(AzureStorageClient azureStorageClient)
        {
            this.azureStorageClient = azureStorageClient;
        }

        public Task<Result<Maybe<Favorites>>> GetFavorites(string userId) =>
            this.GetFavoritesContainer().Bind(container =>
            this.azureStorageClient.DownloadJsonAsync<Favorites>(
                container.GetBlobClient($"{userId}.{JsonFormat}")));

        public Task<Result<Uri>> SaveFavorites(Favorites favorites) =>
            this.GetFavoritesContainer().Bind(container =>
            this.azureStorageClient.UploadJsonAsync(
                blob: container.GetBlobClient($"{favorites.UserId}.{JsonFormat}"),
                record: favorites));

        private Task<Result<BlobContainerClient>> GetFavoritesContainer() =>
            this.azureStorageClient.GetOrCreateContainer("favorites");
    }
}