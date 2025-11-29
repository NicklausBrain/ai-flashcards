using CSharpFunctionalExtensions;

using My1kWordsEe.Models.Semantics;
using My1kWordsEe.Services.Cqs;
using My1kWordsEe.Services.Scoped;

namespace My1kWordsEe.Models.Games
{
    public class TranslateToEtGame
    {
        private readonly CheckEtTranslationCommand checkEtTranslationCommand;
        private readonly FavoritesStateContainer favoritesStateContainer;

        public TranslateToEtGame(
            string etWord,
            int sampleIndex,
            SampleSentenceWithMedia sampleSentence,
            CheckEtTranslationCommand checkEtTranslationCommand,
            FavoritesStateContainer favoritesStateContainer)
        {
            this.EtWord = etWord;
            this.SampleIndex = sampleIndex;
            this.SampleSentence = sampleSentence;
            this.checkEtTranslationCommand = checkEtTranslationCommand;
            this.favoritesStateContainer = favoritesStateContainer;
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

            CheckResult.Execute(r => r.Tap(r =>
            {
                var update = r.Match >= 4
                ? UpdateScoreCommand.ScoreUpdate.Up
                : UpdateScoreCommand.ScoreUpdate.Down;
                _ = this.favoritesStateContainer.UpdateScore(EtWord, update);
            }));
        }

        public void GiveUp()
        {
            CheckResult = Result.Success(EtTranslationCheckResult.Fail(
                etSentence: SampleSentence.Sentence.Et,
                enSentence: SampleSentence.Sentence.En));

            _ = favoritesStateContainer.UpdateScore(EtWord, UpdateScoreCommand.ScoreUpdate.Down);
        }
    }
}