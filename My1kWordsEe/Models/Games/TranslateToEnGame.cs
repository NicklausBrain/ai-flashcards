using CSharpFunctionalExtensions;

using My1kWordsEe.Services.Cqs;

namespace My1kWordsEe.Models.Games
{
    public class TranslateToEnGame
    {
        private readonly SampleSentence sampleSentence;

        public TranslateToEnGame(string eeWord, int sampleIndex, SampleSentence sampleSentence)
        {
            this.EeWord = eeWord;
            this.SampleIndex = sampleIndex;
            this.sampleSentence = sampleSentence;
        }

        public string EeWord { get; private set; }

        public int SampleIndex { get; private set; }

        public Maybe<Result<EnTranslationCheckResult>> CheckResult { get; private set; }

        public bool IsReady => this != Empty;

        public bool IsFinished => CheckResult.HasValue;

        public string EeSentence => sampleSentence.EeSentence;

        public Uri ImageUrl => sampleSentence.ImageUrl;

        public Uri AudioUrl => sampleSentence.EeAudioUrl;

        public string UserTranslation { get; set; } = string.Empty;

        public bool IsCheckInProgress { get; private set; }

        public async Task Submit(CheckEnTranslationCommand checkEnTranslationCommand)
        {
            if (!UserTranslation.ValidateSentence())
            {
                CheckResult = Result.Failure<EnTranslationCheckResult>("Bad input");
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

        public static async Task<Result<TranslateToEnGame>> Generate(
            GetOrAddSampleWordCommand getOrAddSampleWordCommand,
            AddSampleSentenceCommand addSampleSentenceCommand,
            string? eeWord,
            int? wordIndex)
        {
            eeWord = (eeWord ?? GetRandomEeWord()).ToLower();
            var sampleWord = await getOrAddSampleWordCommand.Invoke(eeWord);

            if (sampleWord.IsFailure)
            {
                return Result.Failure<TranslateToEnGame>(sampleWord.Error);
            }

            if (sampleWord.Value.Samples.Any())
            {
                return new TranslateToEnGame(eeWord, 0, sampleWord.Value.Samples.First());
            }
            else
            {
                var addSampleResult = await addSampleSentenceCommand.Invoke(sampleWord.Value);

                if (addSampleResult.IsSuccess)
                {
                    return new TranslateToEnGame(eeWord, 0, addSampleResult.Value.Samples.First());
                }

                return Result.Failure<TranslateToEnGame>(addSampleResult.Error);
            }
        }

        /// <summary>
        /// Null object pattern.
        /// </summary>
        public static readonly TranslateToEnGame Empty = new TranslateToEnGame(string.Empty, 0, SampleSentence.Empty);

        private static string GetRandomEeWord()
        {
            var rn = new Random(Environment.TickCount);
            var eeWord = Ee1kWords.AllWords[rn.Next(0, Ee1kWords.AllWords.Length)];
            return eeWord.EeWord;
        }
    }
}