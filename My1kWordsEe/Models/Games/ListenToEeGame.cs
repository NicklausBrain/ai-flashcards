using CSharpFunctionalExtensions;

using My1kWordsEe.Services.Cqs;

namespace My1kWordsEe.Models.Games
{
    public class ListenToEeGameFactory
    {
        GetOrAddSampleWordCommand getOrAddSampleWordCommand;
        AddSampleSentenceCommand addSampleSentenceCommand;
        CheckEeListeningCommand checkEeListeningCommand;

        public ListenToEeGameFactory(
            GetOrAddSampleWordCommand getOrAddSampleWordCommand,
            AddSampleSentenceCommand addSampleSentenceCommand,
            CheckEeListeningCommand checkEeListeningCommand)
        {
            this.getOrAddSampleWordCommand = getOrAddSampleWordCommand;
            this.addSampleSentenceCommand = addSampleSentenceCommand;
            this.checkEeListeningCommand = checkEeListeningCommand;
        }


        public async Task<Result<ListenToEeGame>> Generate(
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
                return new ListenToEeGame(eeWord, 0, sampleWord.Value.Samples.First(), checkEeListeningCommand);
            }
            else
            {
                var addSampleResult = await addSampleSentenceCommand.Invoke(sampleWord.Value);

                if (addSampleResult.IsSuccess)
                {
                    return new ListenToEeGame(eeWord, 0, addSampleResult.Value.Samples.First(), checkEeListeningCommand);
                }

                return Result.Failure<ListenToEeGame>(addSampleResult.Error);
            }
        }

        private static string GetRandomEeWord()
        {
            var rn = new Random(Environment.TickCount);
            var eeWord = Ee1kWords.AllWords[rn.Next(0, Ee1kWords.AllWords.Length)];
            return eeWord.EeWord;
        }
    }

    public class ListenToEeGame
    {
        private readonly CheckEeListeningCommand checkEeListeningCommand;

        public ListenToEeGame(
            string eeWord,
            int sampleIndex,
            SampleSentence sampleSentence,
            CheckEeListeningCommand checkEeListeningCommand)
        {
            this.SampleSentence = sampleSentence;
            this.EeWord = eeWord;
            this.SampleIndex = sampleIndex;
            var rnWords = sampleSentence.EeSentence.Split(' ');
            Random.Shared.Shuffle(rnWords);
            this.RandomizedWords = rnWords;
            this.checkEeListeningCommand = checkEeListeningCommand;
        }

        public SampleSentence SampleSentence { get; init; }

        public string EeWord { get; init; }

        public int SampleIndex { get; init; }

        public string[] RandomizedWords { get; init; }

        public Maybe<Result<EeListeningCheckResult>> CheckResult { get; private set; }

        public bool IsFinished => CheckResult.HasValue;

        public string EeSentence => SampleSentence.EeSentence;

        public Uri ImageUrl => SampleSentence.ImageUrl;

        public Uri AudioUrl => SampleSentence.EeAudioUrl;

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
    }
}
