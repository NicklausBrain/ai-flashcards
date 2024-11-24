using CSharpFunctionalExtensions;

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

        public async Task<Result> Invoke(string userId, string eeWord)
        {
            return await this.getFavoritesCommand.Invoke(userId).Bind(async favorites =>
            {
                favorites.Words.Remove(eeWord.ToLower());
                return await this.azureBlobService.SaveFavorites(favorites);
            });
        }
    }
}
