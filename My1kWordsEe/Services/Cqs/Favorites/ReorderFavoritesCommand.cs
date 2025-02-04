using CSharpFunctionalExtensions;

using My1kWordsEe.Models;
using My1kWordsEe.Models.Semantics;
using My1kWordsEe.Services.Db;

namespace My1kWordsEe.Services.Cqs
{
    public class ReorderFavoritesCommand
    {
        private readonly GetFavoritesQuery getFavoritesCommand;
        private readonly AzureStorageClient azureBlobService;

        public ReorderFavoritesCommand(
            GetFavoritesQuery getFavoritesCommand,
            AzureStorageClient azureBlobService)
        {
            this.getFavoritesCommand = getFavoritesCommand;
            this.azureBlobService = azureBlobService;
        }

        public async Task<Result<Favorites>> Invoke(string userId, IEnumerable<EtWord> sampleWords)
        {
            return await this.getFavoritesCommand.Invoke(userId).Bind(async favorites =>
            {
                var reorderedFavorites = new Favorites(
                    userId: favorites.UserId,
                    words: sampleWords.ToDictionary(w => w.Value),
                    sentences: favorites.Sentences);
                return await this.azureBlobService.SaveFavorites(reorderedFavorites).Bind(_ =>
                    Result.Success(reorderedFavorites));
            });
        }
    }
}