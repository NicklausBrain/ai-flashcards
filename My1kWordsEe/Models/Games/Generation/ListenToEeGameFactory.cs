using CSharpFunctionalExtensions;

using My1kWordsEe.Services.Cqs;
using My1kWordsEe.Services.Cqs.Et;

namespace My1kWordsEe.Models.Games
{
    public class ListenToEeGameFactory
    {
        private readonly GetOrAddEtWordCommand getOrAddEtWordCommand;
        private readonly GetEtSampleSentencesQuery getEtSampleSentencesQuery;
        private readonly AddEtSampleSentenceCommand addEtSampleSentenceCommand;
        private readonly CheckEeListeningCommand checkEeListeningCommand;

        public ListenToEeGameFactory(
            GetOrAddEtWordCommand getOrAddSampleWordCommand,
            GetEtSampleSentencesQuery getEtSampleSentencesQuery,
            AddEtSampleSentenceCommand addSampleSentenceCommand,
            CheckEeListeningCommand checkEeListeningCommand)
        {
            this.getOrAddEtWordCommand = getOrAddSampleWordCommand;
            this.getEtSampleSentencesQuery = getEtSampleSentencesQuery;
            this.addEtSampleSentenceCommand = addSampleSentenceCommand;
            this.checkEeListeningCommand = checkEeListeningCommand;
        }


        public async Task<Result<ListenToEeGame>> Generate(
            string? eeWord,
            int? sampleIndex = 0)
        {
            const int senseIndex = 0;
            eeWord = (eeWord ?? GetRandomEeWord()).ToLower();

            if (!eeWord.ValidateWord())
            {
                return Result.Failure<ListenToEeGame>("Not an Estonian word");
            }

            var sampleWord = await getOrAddEtWordCommand.Invoke(eeWord);

            if (sampleWord.IsFailure)
            {
                return Result.Failure<ListenToEeGame>(sampleWord.Error);
            }

            var samples = await getEtSampleSentencesQuery.Invoke(sampleWord.Value, senseIndex);

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
                var addSampleResult = await addEtSampleSentenceCommand.Invoke(sampleWord.Value, senseIndex);

                if (addSampleResult.IsSuccess)
                {
                    return new ListenToEeGame(eeWord, 0, addSampleResult.Value.First(), checkEeListeningCommand);
                }

                return Result.Failure<ListenToEeGame>(addSampleResult.Error);
            }
        }

        private static string GetRandomEeWord()
        {
            var rn = new Random(Environment.TickCount);
            var eeWord = Ee1kWords.AllWords[rn.Next(0, Ee1kWords.AllWords.Length)];
            return eeWord.EeWord;
        }
    }
}