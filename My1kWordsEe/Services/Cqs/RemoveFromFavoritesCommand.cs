using CSharpFunctionalExtensions;

using My1kWordsEe.Models;
using My1kWordsEe.Services.Db;

namespace My1kWordsEe.Services.Cqs
{
    public class RemoveFromFavoritesCommand
    {
        private readonly GetFavoritesQuery getFavoritesCommand;
        private readonly AzureStorageClient azureBlobService;

        public RemoveFromFavoritesCommand(
            GetFavoritesQuery getFavoritesCommand,
            AzureStorageClient azureBlobService)
        {
            this.getFavoritesCommand = getFavoritesCommand;
            this.azureBlobService = azureBlobService;
        }

        public async Task<Result<Favorites>> Invoke(string userId, SampleWord sample)
        {
            return await this.getFavoritesCommand.Invoke(userId).Bind(async favorites =>
            {
                favorites.Words.Remove(sample.EeWord.ToLower());
                return await this.azureBlobService.SaveFavorites(favorites).Bind(_ =>
                    Result.Success(favorites));
            });
        }
    }
}
