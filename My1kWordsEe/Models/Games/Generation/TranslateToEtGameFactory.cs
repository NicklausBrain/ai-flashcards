using CSharpFunctionalExtensions;

using My1kWordsEe.Models.Games.Generation;
using My1kWordsEe.Services.Cqs;
using My1kWordsEe.Services.Cqs.Et;
using My1kWordsEe.Services.Scoped;

namespace My1kWordsEe.Models.Games
{
    public class TranslateToEtGameFactory
    {
        private readonly NextEtWordSelector nextWordSelector;
        private readonly GetOrAddEtWordCommand getOrAddEtWordCommand;
        private readonly GetEtSampleSentencesQuery getEtSampleSentencesQuery;
        private readonly AddEtSampleSentenceCommand addEtSampleSentenceCommand;
        private readonly CheckEtTranslationCommand checkEtTranslationCommand;
        private readonly FavoritesStateContainer favoritesStateContainer;

        public TranslateToEtGameFactory(
            NextEtWordSelector nextWordSelector,
            GetOrAddEtWordCommand getOrAddEtWordCommand,
            GetEtSampleSentencesQuery getEtSampleSentencesQuery,
            AddEtSampleSentenceCommand addEtSampleSentenceCommand,
            CheckEtTranslationCommand checkEtTranslationCommand,
            FavoritesStateContainer favoritesStateContainer)
        {
            this.nextWordSelector = nextWordSelector;
            this.getOrAddEtWordCommand = getOrAddEtWordCommand;
            this.getEtSampleSentencesQuery = getEtSampleSentencesQuery;
            this.addEtSampleSentenceCommand = addEtSampleSentenceCommand;
            this.checkEtTranslationCommand = checkEtTranslationCommand;
            this.favoritesStateContainer = favoritesStateContainer;
        }

        public async Task<Result<TranslateToEtGame>> Generate(string? word, int? wordIndex)
        {
            const int senseIndex = 0;
            word = (word ?? await GetRandomEtWord()).ToLower();
            var etWord = await getOrAddEtWordCommand.Invoke(word);

            if (etWord.IsFailure)
            {
                return Result.Failure<TranslateToEtGame>(etWord.Error);
            }

            var samples = await getEtSampleSentencesQuery.Invoke(etWord.Value, senseIndex);

            if (samples.IsFailure)
            {
                return Result.Failure<TranslateToEtGame>(samples.Error);
            }

            if (samples.Value.Any())
            {
                return new TranslateToEtGame(
                    word, 0, samples.Value.First(), this.checkEtTranslationCommand, this.favoritesStateContainer);
            }
            else
            {
                var addSampleResult = await addEtSampleSentenceCommand.Invoke(etWord.Value, senseIndex);

                if (addSampleResult.IsSuccess)
                {
                    return new TranslateToEtGame(
                        word, 0, addSampleResult.Value.First(), this.checkEtTranslationCommand, this.favoritesStateContainer);
                }

                return Result.Failure<TranslateToEtGame>(addSampleResult.Error);
            }
        }

        private async Task<string> GetRandomEtWord()
        {
            var etWord = await nextWordSelector.GetNextWord();
            return etWord.Value;
        }
    }
}