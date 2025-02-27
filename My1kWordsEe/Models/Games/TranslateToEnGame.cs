using CSharpFunctionalExtensions;

using My1kWordsEe.Models.Semantics;
using My1kWordsEe.Services.Cqs;

namespace My1kWordsEe.Models.Games
{
    public class TranslateToEnGame
    {
        private readonly CheckEnTranslationCommand checkEnTranslationCommand;

        public TranslateToEnGame(
            string eeWord,
            int sampleIndex,
            SampleSentenceWithMedia sampleSentence,
            CheckEnTranslationCommand checkEnTranslationCommand)
        {
            this.EeWord = eeWord;
            this.SampleIndex = sampleIndex;
            this.SampleSentence = sampleSentence;
            this.checkEnTranslationCommand = checkEnTranslationCommand;
        }

        public string EeWord { get; init; }

        public int SampleIndex { get; init; }

        public SampleSentenceWithMedia SampleSentence { get; init; }

        public Maybe<Result<EnTranslationCheckResult>> CheckResult { get; private set; }

        public bool IsFinished => CheckResult.HasValue;

        public string EtSentence => SampleSentence.Sentence.Et;

        public Uri ImageUrl => SampleSentence.ImageUrl;

        public Uri AudioUrl => SampleSentence.AudioUrl;

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
            var defaultEnTranslation = SampleSentence.Sentence.En.Trim('.', ' ');

            if (string.Equals(
                userInput,
                defaultEnTranslation,
                StringComparison.InvariantCultureIgnoreCase))
            {
                CheckResult = Result.Success(EnTranslationCheckResult.Success(
                    eeSentence: EtSentence,
                    enSentence: defaultEnTranslation));
            }
            else
            {
                IsCheckInProgress = true;
                CheckResult = await checkEnTranslationCommand.Invoke(
                    eeSentence: EtSentence,
                    enSentence: userInput);
                IsCheckInProgress = false;
            }
        }

        public void GiveUp()
        {
            CheckResult = Result.Success(EnTranslationCheckResult.Fail(
                eeSentence: EtSentence,
                enSentence: SampleSentence.Sentence.En));
        }
    }
}