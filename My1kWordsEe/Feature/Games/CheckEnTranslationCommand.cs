using System.ComponentModel;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Unicode;

using CSharpFunctionalExtensions;

using Microsoft.ApplicationInsights;

namespace My1kWordsEe.Feature.Games
{
    public class CheckEnTranslationCommand
    {
        public static readonly string Prompt =
$@"Your task is to check the user's translation from Estonian into English.
Ignore letter case (upper or lower) and punctuation in your check.
When your comment quotes Estonian words, always use correct Estonian spelling (ä, ö, õ, ü, š, ž) and never replace Estonian letters with digits or other symbols.
Your input is a JSON object:
{JsonSchemaRecord.For(typeof(Input))}";

        private readonly TelemetryClient telemetry;
        private readonly OpenAiClient openAiClient;

        public CheckEnTranslationCommand(TelemetryClient telemetry, OpenAiClient openAiClient)
        {
            this.telemetry = telemetry;
            this.openAiClient = openAiClient;
        }

        public virtual async Task<Result<EnTranslationCheckResult>> Invoke(string etSentence, string enSentence, string enExpectedSentence)
        {
            var input = JsonSerializer.Serialize(new Input
            {
                EeSentence = etSentence.Trim('.', ' ').ToLowerInvariant(),
                EnUserSentence = enSentence.Trim('.', ' ').ToLowerInvariant(),
                EnExpectedSentence = enExpectedSentence.Trim('.', ' ').ToLowerInvariant(),
            }, new JsonSerializerOptions
            {
                WriteIndented = false,
                Encoder = JavaScriptEncoder.Create(UnicodeRanges.All),
            });

            var result = await this.openAiClient.CompleteJsonSchemaAsync<Response>(
                instructions: Prompt,
                input: input,
                schema: JsonSchemaRecord.For(typeof(Response)));

            telemetry.TrackEvent("CheckEnTranslationCommand-done", new Dictionary<string, string>
            {
                { "etSentence", etSentence },
                { "enSentence", enSentence },
            });

            // Canonical fields come from local truth; the model supplies only Match and EnComment.
            if (result.IsSuccess)
            {
                return Result.Success(new EnTranslationCheckResult
                {
                    EeSentence = etSentence,
                    EnUserSentence = enSentence,
                    EnExpectedSentence = enExpectedSentence,
                    EnComment = result.Value.EnComment,
                    Match = result.Value.Match,
                });
            }

            return Result.Failure<EnTranslationCheckResult>(result.Error);
        }

        public struct Input
        {
            [Description("The Estonian sentence shown to the user")]
            public string EeSentence { get; init; }

            [Description("The user's translation into English")]
            public string EnUserSentence { get; init; }

            [Description("The expected translation into English")]
            public string EnExpectedSentence { get; init; }
        }

        public struct Response
        {
            [Description("Comment explaining to the student his mistake (if any) in English")]
            public string EnComment { get; init; }

            [Description("Correctness level as an integer from 0 to 5")]
            public ushort Match { get; init; }
        }
    }

    public record EnTranslationCheckResult
    {
        public required string EeSentence { get; init; }

        public required string EnUserSentence { get; init; }

        public required string EnExpectedSentence { get; init; }

        public required string EnComment { get; init; } = string.Empty;

        public required ushort Match { get; init; }

        public bool IsMaxMatch => this.Match == 5;

        public static EnTranslationCheckResult Success(string eeSentence, string enSentence) => new EnTranslationCheckResult
        {
            EeSentence = eeSentence,
            EnUserSentence = enSentence,
            EnExpectedSentence = enSentence,
            Match = 5,
            EnComment = string.Empty
        };

        public static EnTranslationCheckResult Fail(string eeSentence, string enSentence) => new EnTranslationCheckResult
        {
            EeSentence = eeSentence,
            EnUserSentence = "",
            EnExpectedSentence = enSentence,
            Match = 0,
            EnComment = string.Empty
        };
    }
}