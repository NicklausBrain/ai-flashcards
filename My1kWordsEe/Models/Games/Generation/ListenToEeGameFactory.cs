using CSharpFunctionalExtensions;

using My1kWordsEe.Services.Cqs;

namespace My1kWordsEe.Models.Games
{
    public class ListenToEeGameFactory
    {
        GetOrAddSampleWordCommand getOrAddSampleWordCommand;
        AddSampleSentenceCommand addSampleSentenceCommand;
        CheckEeListeningCommand checkEeListeningCommand;

        public ListenToEeGameFactory(
            GetOrAddSampleWordCommand getOrAddSampleWordCommand,
            AddSampleSentenceCommand addSampleSentenceCommand,
            CheckEeListeningCommand checkEeListeningCommand)
        {
            this.getOrAddSampleWordCommand = getOrAddSampleWordCommand;
            this.addSampleSentenceCommand = addSampleSentenceCommand;
            this.checkEeListeningCommand = checkEeListeningCommand;
        }


        public async Task<Result<ListenToEeGame>> Generate(
            string? eeWord,
            int? wordIndex)
        {
            eeWord = (eeWord ?? GetRandomEeWord()).ToLower();

            if (!eeWord.ValidateWord())
            {
                return Result.Failure<ListenToEeGame>("Not an Estonian word");
            }

            var sampleWord = await getOrAddSampleWordCommand.Invoke(eeWord);

            if (sampleWord.IsFailure)
            {
                return Result.Failure<ListenToEeGame>(sampleWord.Error);
            }

            if (sampleWord.Value.Samples.Any())
            {
                return new ListenToEeGame(eeWord, 0, sampleWord.Value.Samples.First(), checkEeListeningCommand);
            }
            else
            {
                var addSampleResult = await addSampleSentenceCommand.Invoke(sampleWord.Value);

                if (addSampleResult.IsSuccess)
                {
                    return new ListenToEeGame(eeWord, 0, addSampleResult.Value.Samples.First(), checkEeListeningCommand);
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