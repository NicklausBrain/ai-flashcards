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

        public async Task<Result<TranslateToEnGame>> Generate(string? etWord, uint senseIndex)
        {
            etWord = (string.IsNullOrEmpty(etWord) ? await GetRandomEtWord() : etWord).ToLower();
            var etWordObj = await getOrAddEtWordCommand.Invoke(etWord);

            if (etWordObj.IsFailure)
            {
                return Result.Failure<TranslateToEnGame>(etWordObj.Error);
            }

            var samples = await getEtSampleSentencesQuery.Invoke(etWordObj.Value, senseIndex);

            if (samples.IsFailure)
            {
                return Result.Failure<TranslateToEnGame>(samples.Error);
            }

            if (samples.Value.Any())
            {
                return new TranslateToEnGame(
                    etWord, 0, samples.Value.First(), this.checkEnTranslationCommand, this.favoritesStateContainer);
            }
            else
            {
                var addSampleResult = await addEtSampleSentenceCommand.Invoke(etWordObj.Value, senseIndex);

                if (addSampleResult.IsSuccess)
                {
                    return new TranslateToEnGame(
                        etWord, 0, addSampleResult.Value.First(), this.checkEnTranslationCommand, this.favoritesStateContainer);
                }

                return Result.Failure<TranslateToEnGame>(addSampleResult.Error);
            }
        }

        private async Task<string> GetRandomEtWord()
        {
            var etWord = await nextWordSelector.GetNextWord();
            return etWord.Value;
        }
    }
}