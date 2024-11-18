using CSharpFunctionalExtensions;

using My1kWordsEe.Services.Cqs;

namespace My1kWordsEe.Models.Games
{
    public class TranslateToEnGame
    {
        private readonly SampleSentence sampleSentence;

        public TranslateToEnGame(SampleSentence sampleSentence)
        {
            this.sampleSentence = sampleSentence;
        }

        public Maybe<Result<EnTranslationCheckResult>> CheckResult { get; private set; }

        public bool IsFinished => CheckResult.HasValue;

        public string EeSentence => sampleSentence.EeSentence;

        public Uri ImageUrl => sampleSentence.ImageUrl;

        public string UserTranslation { get; set; } = string.Empty;

        public bool IsCheckInProgress { get; private set; }

        public static async Task<TranslateToEnGame> Generate(
            GetOrAddSampleWordCommand getOrAddSampleWordCommand,
            AddSampleSentenceCommand addSampleSentenceCommand)
        {
            var rn = new Random(Environment.TickCount);
            var eeWord = Ee1kWords.AllWords[rn.Next(0, Ee1kWords.AllWords.Length)];
            var sampleWord = await getOrAddSampleWordCommand.Invoke(eeWord.Value);

            if (sampleWord.Value.Samples.Any())
            {
                return new TranslateToEnGame(sampleWord.Value.Samples.First());
            }
            else
            {
                return new TranslateToEnGame((await addSampleSentenceCommand.Invoke(sampleWord.Value)).Value.Samples.First());
            }
        }

        public async Task Submit(CheckEnTranslationCommand checkEnTranslationCommand)
        {
            if (!UserTranslation.ValidateSentence())
            {
                return;
            }

            var userInput = UserTranslation.Trim('.', ' ');
            var defaultEnTranslation = sampleSentence.EnSentence.Trim('.', ' ');

            if (string.Equals(
                userInput,
                defaultEnTranslation,
                StringComparison.InvariantCultureIgnoreCase))
            {
                CheckResult = Result.Success(EnTranslationCheckResult.Success(
                    eeSentence: EeSentence,
                    enSentence: defaultEnTranslation));
            }
            else
            {
                IsCheckInProgress = true;
                CheckResult = await checkEnTranslationCommand.Invoke(
                    eeSentence: EeSentence,
                    enSentence: userInput);
                IsCheckInProgress = false;
            }
        }
    }
}