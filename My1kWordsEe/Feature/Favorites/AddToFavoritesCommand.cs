using CSharpFunctionalExtensions;

using My1kWordsEe.Feature.Samples;
using My1kWordsEe.Feature.Words;

namespace My1kWordsEe.Feature.Favorites
{
    public class AddToFavoritesCommand
    {
        private readonly GetFavoritesQuery getFavoritesCommand;
        private readonly FavoritesStorageClient favoritesStorageClient;

        public AddToFavoritesCommand(
            GetFavoritesQuery getFavoritesCommand,
            FavoritesStorageClient favoritesStorageClient)
        {
            this.getFavoritesCommand = getFavoritesCommand;
            this.favoritesStorageClient = favoritesStorageClient;
        }

        public async Task<Result<Favorites>> Invoke(string userId, EtWord etWord)
        {
            return await this.getFavoritesCommand.Invoke(userId).Bind(async favorites =>
            {
                favorites.Words.Add(etWord.Value.ToLower(), etWord);
                return await this.favoritesStorageClient.SaveFavorites(favorites).Bind(_ =>
                    Result.Success(favorites));
            });
        }

        public async Task<Result<Favorites>> Invoke(string userId, SampleSentenceWithMedia sampleSentence)
        {
            return await this.getFavoritesCommand.Invoke(userId).Bind(async favorites =>
            {
                favorites.Sentences.Add(sampleSentence.Sentence.Et.ToLower(), sampleSentence);
                return await this.favoritesStorageClient.SaveFavorites(favorites).Bind(_ =>
                    Result.Success(favorites));
            });
        }
    }
}
