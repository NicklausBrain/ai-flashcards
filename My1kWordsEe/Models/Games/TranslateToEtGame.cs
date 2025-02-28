using CSharpFunctionalExtensions;

using My1kWordsEe.Models.Semantics;
using My1kWordsEe.Services.Cqs;

namespace My1kWordsEe.Models.Games
{
    public class TranslateToEtGame
    {
        private readonly CheckEtTranslationCommand checkEtTranslationCommand;

        public TranslateToEtGame(
            string etWord,
            int sampleIndex,
            SampleSentenceWithMedia sampleSentence,
            CheckEtTranslationCommand checkEtTranslationCommand)
        {
            this.EtWord = etWord;
            this.SampleIndex = sampleIndex;
            this.SampleSentence = sampleSentence;
            this.checkEtTranslationCommand = checkEtTranslationCommand;
        }

        public string EtWord { get; init; }

        public int SampleIndex { get; init; }

        public SampleSentenceWithMedia SampleSentence { get; init; }

        public Maybe<Result<EtTranslationCheckResult>> CheckResult { get; private set; }

        public bool IsFinished => CheckResult.HasValue;

        public string EnSentence => SampleSentence.Sentence.En;

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
                CheckResult = Result.Failure<EtTranslationCheckResult>("Bad input");
                return;
            }

            var userInput = UserTranslation.Trim('.', ' ');
            var defaultEtTranslation = SampleSentence.Sentence.Et.Trim('.', ' ');

            if (string.Equals(
                userInput,
                defaultEtTranslation,
                StringComparison.InvariantCultureIgnoreCase))
            {
                CheckResult = Result.Success(EtTranslationCheckResult.Success(
                    enSentence: EnSentence,
                    etSentence: defaultEtTranslation));
            }
            else
            {
                IsCheckInProgress = true;
                CheckResult = await checkEtTranslationCommand.Invoke(
                    etSentence: userInput,
                    enSentence: EnSentence);
                IsCheckInProgress = false;
            }
        }

        public void GiveUp()
        {
            CheckResult = Result.Success(EtTranslationCheckResult.Fail(
                etSentence: SampleSentence.Sentence.Et,
                enSentence: SampleSentence.Sentence.En));
        }
    }
}