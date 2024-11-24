using CSharpFunctionalExtensions;

using My1kWordsEe.Models;
using My1kWordsEe.Services.Db;

namespace My1kWordsEe.Services.Cqs
{
    public class AddToFavoritesCommand
    {
        private readonly GetFavoritesQuery getFavoritesCommand;
        private readonly AzureStorageClient azureBlobService;

        public AddToFavoritesCommand(
            GetFavoritesQuery getFavoritesCommand,
            AzureStorageClient azureBlobService)
        {
            this.getFavoritesCommand = getFavoritesCommand;
            this.azureBlobService = azureBlobService;
        }

        public async Task<Result> Invoke(string userId, SampleWord sampleWord)
        {
            return await this.getFavoritesCommand.Invoke(userId).Bind(async favorites =>
            {
                favorites.Words.Add(sampleWord.EeWord.ToLower(), sampleWord);
                return await this.azureBlobService.SaveFavorites(favorites);
            });
        }
    }
}
