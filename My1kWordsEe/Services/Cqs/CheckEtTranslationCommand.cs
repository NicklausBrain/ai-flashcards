using System.ComponentModel;
using System.Text.Json;
using System.Text.Json.Serialization;

using CSharpFunctionalExtensions;

using Microsoft.ApplicationInsights;

using My1kWordsEe.Models;

namespace My1kWordsEe.Services.Cqs
{
    public class CheckEtTranslationCommand
    {
        public static readonly string Prompt =
$@"Sinu ülesandeks on kontrollida õpilase tõlget inglise keelest eesti keelde.
Ignoreeri oma tšekis suurtähti (ülemine või alumine) ja kirjavahemärke.
Teie sisend on JSON-objekt:
{JsonSchemaRecord.For(typeof(Input))}";

        private readonly OpenAiClient openAiClient;
        private readonly TelemetryClient telemetry;

        public CheckEtTranslationCommand(TelemetryClient telemetry, OpenAiClient openAiClient)
        {
            this.telemetry = telemetry;
            this.openAiClient = openAiClient;
        }

        public virtual async Task<Result<EtTranslationCheckResult>> Invoke(string etSentence, string enSentence)
        {
            var input = JsonSerializer.Serialize(new Input
            {
                EnSentence = enSentence.Trim('.', ' ').ToLowerInvariant(),
                EtUserSentence = etSentence.Trim('.', ' ').ToLowerInvariant()
            });

            var result = await this.openAiClient.CompleteJsonSchemaAsync<EtTranslationCheckResult>(
                instructions: Prompt,
                input: input,
                schema: JsonSchemaRecord.For(typeof(EtTranslationCheckResult)));

            telemetry.TrackEvent("CheckEtTranslationCommand-done", new Dictionary<string, string>
            {
                { "etSentence", etSentence },
                { "enSentence", enSentence },
            });

            return result;
        }

        public struct Input
        {
            [Description("The sentence shown to the user for translation to Estonian")]
            public string EnSentence { get; init; }

            [Description("Kasutaja poolt tehtud tõlge eesti keelde")]
            public string EtUserSentence { get; init; }
        }
    }

    public struct EtTranslationCheckResult
    {
        [Description("The sentence shown to the user for translation to Estonian")]
        public required string EnSentence { get; init; }

        [Description("Kasutaja poolt tehtud tõlge eesti keelde")]
        public required string EtUserSentence { get; init; }

        [Description("Eeldatav eestikeelne lause")]
        public required string EtExpectedSentence { get; init; }

        [Description("Comment explaining to the student his mistake (if any) in English")]
        public required string EnComment { get; init; }

        [Description("Correctes level in integer from 0 to 5")]
        public required ushort Match { get; init; }

        [JsonIgnore]
        public bool IsMaxMatch => this.Match == 5;

        public static EtTranslationCheckResult Success(string enSentence, string etSentence) => new EtTranslationCheckResult
        {
            EnSentence = enSentence,
            EtUserSentence = enSentence,
            EtExpectedSentence = etSentence,
            Match = 5,
            EnComment = string.Empty
        };

        public static EtTranslationCheckResult Fail(string enSentence, string etSentence) => new EtTranslationCheckResult
        {
            EnSentence = enSentence,
            EtUserSentence = "",
            EtExpectedSentence = etSentence,
            Match = 0,
            EnComment = string.Empty
        };
    }
}