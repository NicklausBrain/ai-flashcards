using CSharpFunctionalExtensions;

using My1kWordsEe.Models.Games.Generation;
using My1kWordsEe.Services.Cqs;
using My1kWordsEe.Services.Cqs.Et;

namespace My1kWordsEe.Models.Games
{
    public class TranslateToEtGameFactory
    {
        private readonly NextWordSelector nextWordSelector;
        private readonly GetOrAddEtWordCommand getOrAddEtWordCommand;
        private readonly GetEtSampleSentencesQuery getEtSampleSentencesQuery;
        private readonly AddEtSampleSentenceCommand addEtSampleSentenceCommand;
        private readonly CheckEtTranslationCommand checkEtTranslationCommand;

        public TranslateToEtGameFactory(
            NextWordSelector nextWordSelector,
            GetOrAddEtWordCommand getOrAddEtWordCommand,
            GetEtSampleSentencesQuery getEtSampleSentencesQuery,
            AddEtSampleSentenceCommand addEtSampleSentenceCommand,
            CheckEtTranslationCommand checkEtTranslationCommand)
        {
            this.nextWordSelector = nextWordSelector;
            this.getOrAddEtWordCommand = getOrAddEtWordCommand;
            this.getEtSampleSentencesQuery = getEtSampleSentencesQuery;
            this.addEtSampleSentenceCommand = addEtSampleSentenceCommand;
            this.checkEtTranslationCommand = checkEtTranslationCommand;
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
                return new TranslateToEtGame(word, 0, samples.Value.First(), this.checkEtTranslationCommand);
            }
            else
            {
                var addSampleResult = await addEtSampleSentenceCommand.Invoke(etWord.Value, senseIndex);

                if (addSampleResult.IsSuccess)
                {
                    return new TranslateToEtGame(word, 0, addSampleResult.Value.First(), this.checkEtTranslationCommand);
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