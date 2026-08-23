using CSharpFunctionalExtensions;

namespace My1kWordsEe.Feature.Favorites
{
    public class RemoveFromFavoritesCommand
    {
        private readonly GetFavoritesQuery getFavoritesCommand;
        private readonly FavoritesStorageClient favoritesStorageClient;

        public RemoveFromFavoritesCommand(
            GetFavoritesQuery getFavoritesCommand,
            FavoritesStorageClient favoritesStorageClient)
        {
            this.getFavoritesCommand = getFavoritesCommand;
            this.favoritesStorageClient = favoritesStorageClient;
        }

        public async Task<Result<Favorites>> Invoke(string userId, EtWord sample)
        {
            return await this.getFavoritesCommand.Invoke(userId).Bind(async favorites =>
            {
                favorites.Words.Remove(sample.Value.ToLower());
                return await this.favoritesStorageClient.SaveFavorites(favorites).Bind(_ =>
                    Result.Success(favorites));
            });
        }

        public async Task<Result<Favorites>> Invoke(string userId, ISampleEtSentence sample)
        {
            return await this.getFavoritesCommand.Invoke(userId).Bind(async favorites =>
            {
                favorites.Sentences.Remove(sample.Sentence.Et.ToLower());
                return await this.favoritesStorageClient.SaveFavorites(favorites).Bind(_ =>
                    Result.Success(favorites));
            });
        }
    }
}
