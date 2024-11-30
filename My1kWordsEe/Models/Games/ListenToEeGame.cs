using System.Text.Json.Serialization;

using CSharpFunctionalExtensions;

using My1kWordsEe.Services.Cqs;

namespace My1kWordsEe.Models.Games
{
    public class ListenToEeGame
    {
        private readonly SampleSentence sampleSentence;

        public ListenToEeGame(SampleSentence sampleSentence)
        {
            this.sampleSentence = sampleSentence;
        }

        public Maybe<Result<EeListeningCheckResult>> CheckResult { get; private set; }

        public bool IsReady => this != Empty;

        public bool IsFinished => CheckResult.HasValue;

        public string EeSentence => sampleSentence.EeSentence;

        public Uri ImageUrl => sampleSentence.ImageUrl;

        public Uri AudioUrl => sampleSentence.EeAudioUrl;

        public string UserInput { get; set; } = string.Empty;

        public bool IsCheckInProgress { get; private set; }

        public async Task Submit(CheckEnTranslationCommand checkEnTranslationCommand)
        {
            if (!UserInput.ValidateSentence())
            {
                CheckResult = Result.Failure<EeListeningCheckResult>("Bad input");
                return;
            }

            var userInput = UserInput.Trim('.', ' ');
            var eeSampleSentence = sampleSentence.EeSentence.Trim('.', ' ');

            if (string.Equals(
                userInput,
                eeSampleSentence,
                StringComparison.InvariantCultureIgnoreCase))
            {
                CheckResult = Result.Success(EeListeningCheckResult.Success(
                    eeSentence: sampleSentence.EeSentence,
                    enSentence: sampleSentence.EnSentence,
                    eeUserSentence: userInput));
            }
            else
            {
                //IsCheckInProgress = true;
                CheckResult = Result.Success(EeListeningCheckResult.Failure(
                     eeSentence: sampleSentence.EeSentence,
                     enSentence: sampleSentence.EnSentence,
                     eeUserSentence: userInput));
                //IsCheckInProgress = false;
            }
        }

        public static async Task<Result<ListenToEeGame>> Generate(
            GetOrAddSampleWordCommand getOrAddSampleWordCommand,
            AddSampleSentenceCommand addSampleSentenceCommand)
        {
            var rn = new Random(Environment.TickCount);
            var eeWord = Ee1kWords.AllWords[rn.Next(0, Ee1kWords.AllWords.Length)];
            var sampleWord = await getOrAddSampleWordCommand.Invoke(eeWord.Value);

            if (sampleWord.IsFailure)
            {
                return Result.Failure<ListenToEeGame>(sampleWord.Error);
            }

            if (sampleWord.Value.Samples.Any())
            {
                return new ListenToEeGame(sampleWord.Value.Samples.First());
            }
            else
            {
                var addSampleResult = await addSampleSentenceCommand.Invoke(sampleWord.Value);

                if (addSampleResult.IsSuccess)
                {
                    return new ListenToEeGame(addSampleResult.Value.Samples.First());
                }

                return Result.Failure<ListenToEeGame>(addSampleResult.Error);
            }
        }

        /// <summary>
        /// Null object pattern.
        /// </summary>
        public static readonly ListenToEeGame Empty = new ListenToEeGame(SampleSentence.Empty);
    }
}


public record EeListeningCheckResult
{
    [JsonPropertyName("ee_sentence")]
    public required string EeSentence { get; init; }

    [JsonPropertyName("en_sentence")]
    public required string EnSentence { get; init; }

    [JsonPropertyName("ee_user_sentence")]
    public required string EeUserSentence { get; init; }

    [JsonPropertyName("en_comment")]
    public required string EnComment { get; init; } = string.Empty;

    [JsonPropertyName("match_level")]
    public required ushort Match { get; init; }

    public bool IsMaxMatch => this.Match == 5;

    public static EeListeningCheckResult Success(
        string eeSentence,
        string enSentence,
        string eeUserSentence) =>
        new EeListeningCheckResult
        {
            EeSentence = eeSentence,
            EnSentence = enSentence,
            EeUserSentence = eeUserSentence,
            Match = 5,
            EnComment = string.Empty
        };

    public static EeListeningCheckResult Failure(
        string eeSentence,
        string enSentence,
        string eeUserSentence) =>
        new EeListeningCheckResult
        {
            EeSentence = eeSentence,
            EnSentence = enSentence,
            EeUserSentence = eeUserSentence,
            Match = 0,
            EnComment = string.Empty
        };
}