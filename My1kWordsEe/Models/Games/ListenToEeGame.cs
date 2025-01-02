using CSharpFunctionalExtensions;

using My1kWordsEe.Services.Cqs;

namespace My1kWordsEe.Models.Games
{
    public class ListenToEeGame
    {
        public ListenToEeGame(string eeWord, int sampleIndex, SampleSentence sampleSentence)
        {
            this.SampleSentence = sampleSentence;
            this.EeWord = eeWord;
            this.SampleIndex = sampleIndex;
            var rnWords = sampleSentence.EeSentence.Split(' ');
            Random.Shared.Shuffle(rnWords);
            this.RandomizedWords = rnWords;
        }

        public SampleSentence SampleSentence { get; init; }

        public string EeWord { get; private set; }

        public int SampleIndex { get; private set; }

        public string[] RandomizedWords { get; private set; }

        public Maybe<Result<EeListeningCheckResult>> CheckResult { get; private set; }

        public bool IsReady => this != Empty;

        public bool IsFinished => CheckResult.HasValue;

        public string EeSentence => SampleSentence.EeSentence;

        public Uri ImageUrl => SampleSentence.ImageUrl;

        public Uri AudioUrl => SampleSentence.EeAudioUrl;

        public string UserInput { get; set; } = string.Empty;

        public bool IsCheckInProgress { get; private set; }

        public async Task Submit(CheckEeListeningCommand checkEeListeningCommand)
        {
            if (!UserInput.ValidateSentence())
            {
                CheckResult = Result.Failure<EeListeningCheckResult>("Bad input");
                return;
            }

            var userInput = UserInput.Trim('.', ' ');
            var eeSampleSentence = SampleSentence.EeSentence.Trim('.', ' ');

            if (string.Equals(
                userInput,
                eeSampleSentence,
                StringComparison.InvariantCultureIgnoreCase))
            {
                CheckResult = Result.Success(EeListeningCheckResult.Success(
                    eeSentence: SampleSentence.EeSentence,
                    enSentence: SampleSentence.EnSentence,
                    eeUserSentence: userInput));
            }
            else
            {
                IsCheckInProgress = true;
                CheckResult = await checkEeListeningCommand.Invoke(
                    eeSentence: eeSampleSentence,
                    userInput: userInput);
                IsCheckInProgress = false;
            }
        }

        public static async Task<Result<ListenToEeGame>> Generate(
            GetOrAddSampleWordCommand getOrAddSampleWordCommand,
            AddSampleSentenceCommand addSampleSentenceCommand,
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
                return new ListenToEeGame(eeWord, 0, sampleWord.Value.Samples.First());
            }
            else
            {
                var addSampleResult = await addSampleSentenceCommand.Invoke(sampleWord.Value);

                if (addSampleResult.IsSuccess)
                {
                    return new ListenToEeGame(eeWord, 0, addSampleResult.Value.Samples.First());
                }

                return Result.Failure<ListenToEeGame>(addSampleResult.Error);
            }
        }

        /// <summary>
        /// Null object pattern.
        /// </summary>
        public static readonly ListenToEeGame Empty = new ListenToEeGame(string.Empty, 0, SampleSentence.Empty);

        private static string GetRandomEeWord()
        {
            var rn = new Random(Environment.TickCount);
            var eeWord = Ee1kWords.AllWords[rn.Next(0, Ee1kWords.AllWords.Length)];
            return eeWord.EeWord;
        }
    }
}
