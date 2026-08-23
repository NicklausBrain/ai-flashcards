using System.ComponentModel;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Unicode;

using CSharpFunctionalExtensions;

using Microsoft.ApplicationInsights;

namespace My1kWordsEe.Feature.Games
{
    public class CheckEeListeningCommand
    {
        public static readonly string Prompt =
$@"Your task is to check how well the user recognized Estonian speech.
Ignore letter case (upper or lower) and terminal punctuation in your check.
Ignore diacritical marks when judging correctness, but when your comment quotes Estonian words, always use correct Estonian spelling (ä, ö, õ, ü, š, ž) and never replace Estonian letters with digits or other symbols.
Your input is a JSON object:
{JsonSchemaRecord.For(typeof(Input))}";

        private readonly OpenAiClient openAiClient;
        private readonly TelemetryClient telemetry;

        public CheckEeListeningCommand(TelemetryClient telemetry, OpenAiClient openAiClient)
        {
            this.telemetry = telemetry;
            this.openAiClient = openAiClient;
        }

        public virtual async Task<Result<EeListeningCheckResult>> Invoke(string etSentence, string enSentence, string userInput)
        {
            var input = JsonSerializer.Serialize(new Input
            {
                EeSentence = etSentence.Trim('.', ' ').ToLowerInvariant(),
                EeUserSentence = userInput.Trim('.', ' ').ToLowerInvariant(),
            }, new JsonSerializerOptions
            {
                WriteIndented = false,
                Encoder = JavaScriptEncoder.Create(UnicodeRanges.All),
            });

            var result = await this.openAiClient.CompleteJsonSchemaAsync<Response>(
                instructions: Prompt,
                input: input,
                schema: JsonSchemaRecord.For(typeof(Response)));

            telemetry.TrackEvent("CheckEeListeningCommand-done", new Dictionary<string, string>
            {
                { "etSentence", etSentence },
                { "userInput", userInput },
            });

            // Canonical fields come from local truth; the model supplies only Match and EnComment.
            if (result.IsSuccess)
            {
                return Result.Success(new EeListeningCheckResult
                {
                    EeSentence = etSentence,
                    EnSentence = enSentence,
                    EeUserSentence = userInput,
                    EnComment = result.Value.EnComment,
                    Match = result.Value.Match,
                });
            }

            return Result.Failure<EeListeningCheckResult>(result.Error);
        }

        public struct Input
        {
            [Description("The Estonian sentence the user listened to")]
            public string EeSentence { get; init; }

            [Description("What the user recognized and typed in Estonian")]
            public string EeUserSentence { get; init; }
        }

        public struct Response
        {
            [Description("Comment explaining to the student his mistake (if any) in English")]
            public string EnComment { get; init; }

            [Description("Correctness level as an integer from 0 to 5")]
            public ushort Match { get; init; }
        }
    }

    public record EeListeningCheckResult
    {
        public required string EeSentence { get; init; }

        public required string EnSentence { get; init; }

        public required string EeUserSentence { get; init; }

        public required string EnComment { get; init; } = string.Empty;

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
}