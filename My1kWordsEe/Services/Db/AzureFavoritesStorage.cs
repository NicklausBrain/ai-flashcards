using System.Text.Json;

using Azure;
using Azure.Storage.Blobs;

using CSharpFunctionalExtensions;

using My1kWordsEe.Models;

namespace My1kWordsEe.Services.Db
{
    public partial class AzureStorageClient
    {
        public async Task<Result<Maybe<Favorites>>> GetFavorites(string userId)
        {
            var container = await GetFavoritesContainer();

            if (container.IsFailure)
            {
                return Result.Failure<Maybe<Favorites>>(container.Error);
            }

            BlobClient blob = container.Value.GetBlobClient(JsonBlobName(userId));

            if (!await blob.ExistsAsync())
            {
                return Maybe<Favorites>.None;
            }

            try
            {
                var response = await blob.DownloadContentAsync();
                if (response != null && response.HasValue)
                {
                    var favorites = JsonSerializer.Deserialize<Favorites>(response.Value.Content);
                    return Maybe<Favorites>.From(favorites);
                }
                else
                {
                    return Maybe<Favorites>.None;
                }
            }
            catch (RequestFailedException ex)
            {
                this.logger.LogError(ex, "Failure to download data from blob {name}", blob.Name);
                return Result.Failure<Maybe<Favorites>>("Failure to download data from blob");
            }
            catch (Exception ex) when (ex is JsonException || ex is NotSupportedException)
            {
                this.logger.LogError(ex, "Failure to parse JSON from blob {name}", blob.Name);
                return Result.Failure<Maybe<Favorites>>("Failure to parse JSON data from blob");
            }
        }

        public Task<Result<Uri>> SaveFavorites(Favorites favorites) =>
            this.GetFavoritesContainer().Bind(container =>
            this.UploadStreamAsync(
                container.GetBlobClient(JsonBlobName(favorites.UserId)),
                new MemoryStream(JsonSerializer.SerializeToUtf8Bytes(favorites))));

        private Task<Result<BlobContainerClient>> GetFavoritesContainer() =>
            this.GetOrCreateContainer("favorites");
    }
}
