using CSharpFunctionalExtensions;

using My1kWordsEe.Services.Cqs;

namespace My1kWordsEe.Models.Games
{
    public class TranslateToEnGameFactory
    {
        private readonly GetOrAddSampleWordCommand getOrAddSampleWordCommand;
        private readonly AddSampleSentenceCommand addSampleSentenceCommand;
        private readonly CheckEnTranslationCommand checkEnTranslationCommand;

        public TranslateToEnGameFactory(
            GetOrAddSampleWordCommand getOrAddSampleWordCommand,
            AddSampleSentenceCommand addSampleSentenceCommand,
            CheckEnTranslationCommand checkEnTranslationCommand)
        {
            this.getOrAddSampleWordCommand = getOrAddSampleWordCommand;
            this.addSampleSentenceCommand = addSampleSentenceCommand; ;
            this.checkEnTranslationCommand = checkEnTranslationCommand;
        }

        public async Task<Result<TranslateToEnGame>> Generate(string? eeWord, int? wordIndex)
        {
            eeWord = (eeWord ?? GetRandomEeWord()).ToLower();
            var sampleWord = await getOrAddSampleWordCommand.Invoke(eeWord);

            if (sampleWord.IsFailure)
            {
                return Result.Failure<TranslateToEnGame>(sampleWord.Error);
            }

            if (sampleWord.Value.Samples.Any())
            {
                return new TranslateToEnGame(eeWord, 0, sampleWord.Value.Samples.First(), this.checkEnTranslationCommand);
            }
            else
            {
                var addSampleResult = await addSampleSentenceCommand.Invoke(sampleWord.Value);

                if (addSampleResult.IsSuccess)
                {
                    return new TranslateToEnGame(eeWord, 0, addSampleResult.Value.Samples.First(), this.checkEnTranslationCommand);
                }

                return Result.Failure<TranslateToEnGame>(addSampleResult.Error);
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
