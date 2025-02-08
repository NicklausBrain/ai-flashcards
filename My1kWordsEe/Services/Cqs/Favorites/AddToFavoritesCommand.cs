using CSharpFunctionalExtensions;

using My1kWordsEe.Models;
using My1kWordsEe.Models.Semantics;
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

        public async Task<Result<Favorites>> Invoke(string userId, EtWord sampleWord)
        {
            return await this.getFavoritesCommand.Invoke(userId).Bind(async favorites =>
            {
                favorites.Words.Add(sampleWord.Value.ToLower(), sampleWord);
                return await this.azureBlobService.SaveFavorites(favorites).Bind(_ =>
                    Result.Success(favorites));
            });
        }

        public async Task<Result<Favorites>> Invoke(string userId, SampleSentenceWithMedia sampleSentence)
        {
            return await this.getFavoritesCommand.Invoke(userId).Bind(async favorites =>
            {
                favorites.Sentences.Add(sampleSentence.Sentence.Et.ToLower(), sampleSentence);
                return await this.azureBlobService.SaveFavorites(favorites).Bind(_ =>
                    Result.Success(favorites));
            });
        }
    }
}