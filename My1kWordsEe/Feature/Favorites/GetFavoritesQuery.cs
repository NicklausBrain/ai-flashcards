using CSharpFunctionalExtensions;

namespace My1kWordsEe.Feature.Favorites
{
    public class GetFavoritesQuery
    {
        private readonly FavoritesStorageClient favoritesStorageClient;

        public GetFavoritesQuery(
            FavoritesStorageClient favoritesStorageClient)
        {
            this.favoritesStorageClient = favoritesStorageClient;
        }

        public async Task<Result<Favorites>> Invoke(string userId)
        {
            if (string.IsNullOrWhiteSpace(userId))
            {
                return Result.Failure<Favorites>("Empty user ID");
            }

            (await this.favoritesStorageClient.GetFavorites(userId)).Deconstruct(
                out bool _,
                out bool isBlobAccessFailure,
                out Maybe<Favorites> favorites,
                out string blobAccessError);

            if (isBlobAccessFailure)
            {
                return Result.Failure<Favorites>(blobAccessError);
            }

            if (favorites.HasValue)
            {
                return favorites.Value;
            }

            return new Favorites { UserId = userId };
        }
    }
}
