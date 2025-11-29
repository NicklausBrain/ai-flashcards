using CSharpFunctionalExtensions;

using My1kWordsEe.Models.Games.Generation;
using My1kWordsEe.Services.Cqs;
using My1kWordsEe.Services.Cqs.Et;
using My1kWordsEe.Services.Scoped;

namespace My1kWordsEe.Models.Games
{
    public class TranslateToEnGameFactory
    {
        private readonly NextEtWordSelector nextWordSelector;
        private readonly GetOrAddEtWordCommand getOrAddEtWordCommand;
        private readonly GetEtSampleSentencesQuery getEtSampleSentencesQuery;
        private readonly AddEtSampleSentenceCommand addEtSampleSentenceCommand;
        private readonly CheckEnTranslationCommand checkEnTranslationCommand;
        private readonly FavoritesStateContainer favoritesStateContainer;

        public TranslateToEnGameFactory(
            NextEtWordSelector nextWordSelector,
            GetOrAddEtWordCommand getOrAddEtWordCommand,
            GetEtSampleSentencesQuery getEtSampleSentencesQuery,
            AddEtSampleSentenceCommand addEtSampleSentenceCommand,
            CheckEnTranslationCommand checkEnTranslationCommand,
            FavoritesStateContainer favoritesStateContainer)
        {
            this.nextWordSelector = nextWordSelector;
            this.getOrAddEtWordCommand = getOrAddEtWordCommand;
            this.getEtSampleSentencesQuery = getEtSampleSentencesQuery;
            this.addEtSampleSentenceCommand = addEtSampleSentenceCommand;
            this.checkEnTranslationCommand = checkEnTranslationCommand;
            this.favoritesStateContainer = favoritesStateContainer;
        }

        // todo: fix mixed naming eeWord vs etWord
        public async Task<Result<TranslateToEnGame>> Generate(string? eeWord, int? wordIndex)
        {
            const int senseIndex = 0;
            eeWord = (eeWord ?? await GetRandomEeWord()).ToLower();
            var etWord = await getOrAddEtWordCommand.Invoke(eeWord);

            if (etWord.IsFailure)
            {
                return Result.Failure<TranslateToEnGame>(etWord.Error);
            }

            var samples = await getEtSampleSentencesQuery.Invoke(etWord.Value, senseIndex);

            if (samples.IsFailure)
            {
                return Result.Failure<TranslateToEnGame>(samples.Error);
            }

            if (samples.Value.Any())
            {
                return new TranslateToEnGame(
                    eeWord, 0, samples.Value.First(), this.checkEnTranslationCommand, this.favoritesStateContainer);
            }
            else
            {
                var addSampleResult = await addEtSampleSentenceCommand.Invoke(etWord.Value, senseIndex);

                if (addSampleResult.IsSuccess)
                {
                    return new TranslateToEnGame(
                        eeWord, 0, addSampleResult.Value.First(), this.checkEnTranslationCommand, this.favoritesStateContainer);
                }

                return Result.Failure<TranslateToEnGame>(addSampleResult.Error);
            }
        }

        private async Task<string> GetRandomEeWord()
        {
            var eeWord = await nextWordSelector.GetNextWord();
            return eeWord.Value;
        }
    }
}