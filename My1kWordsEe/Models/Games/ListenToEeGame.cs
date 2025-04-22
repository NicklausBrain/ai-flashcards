using CSharpFunctionalExtensions;

using My1kWordsEe.Models.Semantics;
using My1kWordsEe.Services.Cqs;

namespace My1kWordsEe.Models.Games
{
    public class ListenToEeGame
    {
        private readonly CheckEeListeningCommand checkEeListeningCommand;

        public ListenToEeGame(
            string etWord,
            int sampleIndex,
            SampleSentenceWithMedia sampleSentence,
            CheckEeListeningCommand checkEeListeningCommand)
        {
            this.SampleSentence = sampleSentence;
            this.EtWord = etWord;
            this.SampleIndex = sampleIndex;
            var rnWords = OrderedWords;
            Random.Shared.Shuffle(rnWords);
            this.RandomizedWords = rnWords;
            this.checkEeListeningCommand = checkEeListeningCommand;
        }

        public SampleSentenceWithMedia SampleSentence { get; init; }

        public string EtWord { get; init; }

        public int SampleIndex { get; init; }

        public string[] OrderedWords => SampleSentence.Sentence.Et.Split([" ", "."], StringSplitOptions.RemoveEmptyEntries).ToArray();

        public string[] RandomizedWords { get; init; }

        public Maybe<Result<EeListeningCheckResult>> CheckResult { get; private set; }

        public bool IsFinished => CheckResult.HasValue;

        public string EtSentence => SampleSentence.Sentence.Et;

        public Uri ImageUrl => SampleSentence.ImageUrl;

        public Uri AudioUrl => SampleSentence.AudioUrl;

        public string UserInput { get; set; } = string.Empty;

        public bool IsCheckInProgress { get; private set; }

        public async Task Submit()
        {
            if (!UserInput.ValidateSentence())
            {
                CheckResult = Result.Failure<EeListeningCheckResult>("Bad input");
                return;
            }

            var userInput = UserInput.Trim('.', ' ');
            var etSampleSentence = SampleSentence.Sentence.Et.Trim('.', ' ');

            if (string.Equals(
                userInput,
                etSampleSentence,
                StringComparison.InvariantCultureIgnoreCase))
            {
                CheckResult = Result.Success(EeListeningCheckResult.Success(
                    eeSentence: SampleSentence.Sentence.Et,
                    enSentence: SampleSentence.Sentence.En,
                    eeUserSentence: userInput));
            }
            else
            {
                IsCheckInProgress = true;
                CheckResult = await checkEeListeningCommand.Invoke(
                    etSentence: etSampleSentence,
                    userInput: userInput);
                IsCheckInProgress = false;
            }
        }
    }
}