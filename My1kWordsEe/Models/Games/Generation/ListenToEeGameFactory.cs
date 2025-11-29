using CSharpFunctionalExtensions;

using My1kWordsEe.Services.Cqs;
using My1kWordsEe.Services.Cqs.Et;

namespace My1kWordsEe.Models.Games
{
    public class ListenToEeGameFactory
    {
        private readonly EtWordsCache etWordsCache;
        private readonly GetOrAddEtWordCommand getOrAddEtWordCommand;
        private readonly GetEtSampleSentencesQuery getEtSampleSentencesQuery;
        private readonly AddEtSampleSentenceCommand addEtSampleSentenceCommand;
        private readonly CheckEeListeningCommand checkEeListeningCommand;

        public ListenToEeGameFactory(
            EtWordsCache etWordsCache,
            GetOrAddEtWordCommand getOrAddEtWordCommand,
            GetEtSampleSentencesQuery getEtSampleSentencesQuery,
            AddEtSampleSentenceCommand addSampleSentenceCommand,
            CheckEeListeningCommand checkEeListeningCommand)
        {
            this.etWordsCache = etWordsCache;
            this.getOrAddEtWordCommand = getOrAddEtWordCommand;
            this.getEtSampleSentencesQuery = getEtSampleSentencesQuery;
            this.addEtSampleSentenceCommand = addSampleSentenceCommand;
            this.checkEeListeningCommand = checkEeListeningCommand;
        }


        public async Task<Result<ListenToEeGame>> Generate(
            string? eeWord,
            int? sampleIndex = 0)
        {
            const int senseIndex = 0;
            eeWord = (eeWord ?? GetRandomEtWord()).ToLower();

            if (!eeWord.ValidateWord())
            {
                return Result.Failure<ListenToEeGame>("Not an Estonian word");
            }

            var etWord = await getOrAddEtWordCommand.Invoke(eeWord);

            if (etWord.IsFailure)
            {
                return Result.Failure<ListenToEeGame>(etWord.Error);
            }

            var samples = await getEtSampleSentencesQuery.Invoke(etWord.Value, senseIndex);

            if (samples.IsFailure)
            {
                return Result.Failure<ListenToEeGame>(samples.Error);
            }

            if (samples.Value.Any())
            {
                return new ListenToEeGame(eeWord, 0, samples.Value.First(), checkEeListeningCommand);
            }
            else
            {
                var addSampleResult = await addEtSampleSentenceCommand.Invoke(etWord.Value, senseIndex);

                if (addSampleResult.IsSuccess)
                {
                    return new ListenToEeGame(eeWord, 0, addSampleResult.Value.First(), checkEeListeningCommand);
                }

                return Result.Failure<ListenToEeGame>(addSampleResult.Error);
            }
        }

        private string GetRandomEtWord()
        {
            var rn = new Random(Environment.TickCount);
            var etWord = this.etWordsCache.AllWords[rn.Next(0, this.etWordsCache.AllWords.Length)];
            return etWord.Value;
        }
    }
}