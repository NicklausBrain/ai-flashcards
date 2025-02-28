using CSharpFunctionalExtensions;

using My1kWordsEe.Models;
using My1kWordsEe.Models.Semantics;
using My1kWordsEe.Services.Db;

namespace My1kWordsEe.Services.Cqs
{
    public class ReorderFavoritesCommand
    {
        private readonly GetFavoritesQuery getFavoritesCommand;
        private readonly FavoritesStorageClient favoritesStorageClient;

        public ReorderFavoritesCommand(
            GetFavoritesQuery getFavoritesCommand,
            FavoritesStorageClient favoritesStorageClient)
        {
            this.getFavoritesCommand = getFavoritesCommand;
            this.favoritesStorageClient = favoritesStorageClient;
        }

        public async Task<Result<Favorites>> Invoke(string userId, IEnumerable<EtWord> etWords)
        {
            return await this.getFavoritesCommand.Invoke(userId).Bind(async favorites =>
            {
                var reorderedFavorites = favorites with { Words = etWords.ToDictionary(w => w.Value) };

                return await this.favoritesStorageClient.SaveFavorites(reorderedFavorites).Bind(_ =>
                    Result.Success(reorderedFavorites));
            });
        }
    }
}