using CSharpFunctionalExtensions;

using My1kWordsEe.Services.Cqs;

namespace My1kWordsEe.Models.Games
{
    public class TranslateToEnGame
    {
        private readonly CheckEnTranslationCommand checkEnTranslationCommand;

        public TranslateToEnGame(
            string eeWord,
            int sampleIndex,
            SampleSentence sampleSentence,
            CheckEnTranslationCommand checkEnTranslationCommand)
        {
            this.EeWord = eeWord;
            this.SampleIndex = sampleIndex;
            this.SampleSentence = sampleSentence;
            this.checkEnTranslationCommand = checkEnTranslationCommand;
        }

        public string EeWord { get; init; }

        public int SampleIndex { get; init; }

        public SampleSentence SampleSentence { get; init; }

        public Maybe<Result<EnTranslationCheckResult>> CheckResult { get; private set; }

        public bool IsFinished => CheckResult.HasValue;

        public string EeSentence => SampleSentence.EeSentence;

        public Uri ImageUrl => SampleSentence.ImageUrl;

        public Uri AudioUrl => SampleSentence.EeAudioUrl;

        public string UserTranslation { get; set; } = string.Empty;

        public bool IsCheckInProgress { get; private set; }

        public async Task Submit()
        {
            if (string.IsNullOrWhiteSpace(UserTranslation))
            {
                return;
            }

            if (!UserTranslation.ValidateSentence())
            {
                CheckResult = Result.Failure<EnTranslationCheckResult>("Bad input");
                return;
            }

            var userInput = UserTranslation.Trim('.', ' ');
            var defaultEnTranslation = SampleSentence.EnSentence.Trim('.', ' ');

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