using CSharpFunctionalExtensions;

using My1kWordsEe.Services.Cqs;
using My1kWordsEe.Services.Cqs.Et;

namespace My1kWordsEe.Models.Games
{
    public class TranslateToEnGameFactory
    {
        private readonly EtWordsCache etWordsCache;
        private readonly GetOrAddEtWordCommand getOrAddEtWordCommand;
        private readonly GetEtSampleSentencesQuery getEtSampleSentencesQuery;
        private readonly AddEtSampleSentenceCommand addEtSampleSentenceCommand;
        private readonly CheckEnTranslationCommand checkEnTranslationCommand;

        public TranslateToEnGameFactory(
            EtWordsCache etWordsCache,
            GetOrAddEtWordCommand getOrAddEtWordCommand,
            GetEtSampleSentencesQuery getEtSampleSentencesQuery,
            AddEtSampleSentenceCommand addEtSampleSentenceCommand,
            CheckEnTranslationCommand checkEnTranslationCommand)
        {
            this.etWordsCache = etWordsCache;
            this.getOrAddEtWordCommand = getOrAddEtWordCommand;
            this.getEtSampleSentencesQuery = getEtSampleSentencesQuery;
            this.addEtSampleSentenceCommand = addEtSampleSentenceCommand;
            this.checkEnTranslationCommand = checkEnTranslationCommand;
        }

        public async Task<Result<TranslateToEnGame>> Generate(string? eeWord, int? wordIndex)
        {
            const int senseIndex = 0;
            eeWord = (eeWord ?? GetRandomEeWord()).ToLower();
            var sampleWord = await getOrAddEtWordCommand.Invoke(eeWord);

            if (sampleWord.IsFailure)
            {
                return Result.Failure<TranslateToEnGame>(sampleWord.Error);
            }

            var samples = await getEtSampleSentencesQuery.Invoke(sampleWord.Value, senseIndex);

            if (samples.IsFailure)
            {
                return Result.Failure<TranslateToEnGame>(samples.Error);
            }

            if (samples.Value.Any())
            {
                return new TranslateToEnGame(eeWord, 0, samples.Value.First(), this.checkEnTranslationCommand);
            }
            else
            {
                var addSampleResult = await addEtSampleSentenceCommand.Invoke(sampleWord.Value, senseIndex);

                if (addSampleResult.IsSuccess)
                {
                    return new TranslateToEnGame(eeWord, 0, addSampleResult.Value.First(), this.checkEnTranslationCommand);
                }

                return Result.Failure<TranslateToEnGame>(addSampleResult.Error);
            }
        }

        private string GetRandomEeWord()
        {
            var rn = new Random(Environment.TickCount);
            var eeWord = etWordsCache.AllWords[rn.Next(0, etWordsCache.AllWords.Length)];
            return eeWord.Value;
        }
    }
}