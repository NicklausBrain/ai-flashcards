using CSharpFunctionalExtensions;

using My1kWordsEe.Models.Semantics;
using My1kWordsEe.Services.Cqs;
using My1kWordsEe.Services.Scoped;

namespace My1kWordsEe.Models.Games
{
    public class TranslateToEnGame
    {
        private readonly CheckEnTranslationCommand checkEnTranslationCommand;
        private readonly FavoritesStateContainer favoritesStateContainer;

        public TranslateToEnGame(
            string etWord,
            int sampleIndex,
            SampleSentenceWithMedia sampleSentence,
            CheckEnTranslationCommand checkEnTranslationCommand,
            FavoritesStateContainer favoritesStateContainer)
        {
            this.EtWord = etWord;
            this.SampleIndex = sampleIndex;
            this.SampleSentence = sampleSentence;
            this.checkEnTranslationCommand = checkEnTranslationCommand;
            this.favoritesStateContainer = favoritesStateContainer;
        }

        public string EtWord { get; init; }

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
                    etSentence: EtSentence,
                    enSentence: userInput);
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
            CheckResult = Result.Success(EnTranslationCheckResult.Fail(
                eeSentence: EtSentence,
                enSentence: SampleSentence.Sentence.En));

            _ = favoritesStateContainer.UpdateScore(EtWord, UpdateScoreCommand.ScoreUpdate.Down);
        }
    }
}