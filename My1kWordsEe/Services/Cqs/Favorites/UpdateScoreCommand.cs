using CSharpFunctionalExtensions;

using My1kWordsEe.Models;
using My1kWordsEe.Services.Db;

namespace My1kWordsEe.Services.Cqs
{
    public class UpdateScoreCommand
    {
        private readonly GetFavoritesQuery getFavoritesCommand;
        private readonly FavoritesStorageClient favoritesStorageClient;

        public UpdateScoreCommand(
            GetFavoritesQuery getFavoritesCommand,
            FavoritesStorageClient favoritesStorageClient)
        {
            this.getFavoritesCommand = getFavoritesCommand;
            this.favoritesStorageClient = favoritesStorageClient;
        }

        public async Task<Result<Favorites>> Invoke(string userId, string etWord, ScoreUpdate update)
        {
            etWord = etWord.ToLowerInvariant();
            return await this.getFavoritesCommand.Invoke(userId).Bind(async favorites =>
            {
                favorites.Stats.TryGetValue(etWord, out var score);

                if (update == ScoreUpdate.Up && score < Favorites.MaxWordScore)
                {
                    favorites.Stats[etWord] = score + 1;
                }
                else if (update == ScoreUpdate.Down && score > Favorites.MinWordScore)
                {
                    favorites.Stats[etWord] = score - 1;
                }
                else if (update == ScoreUpdate.Max)
                {
                    favorites.Stats[etWord] = Favorites.MaxWordScore;
                }

                return await this.favoritesStorageClient.SaveFavorites(favorites).Bind(_ => Result.Success(favorites));
            });
        }

        public enum ScoreUpdate
        {
            Up,
            Down,
            Max
        }
    }
}