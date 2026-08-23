using System.ComponentModel;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.Unicode;

using CSharpFunctionalExtensions;

using Microsoft.ApplicationInsights;

namespace My1kWordsEe.Feature.Games
{
    public class CheckEtTranslationCommand
    {
        public static readonly string Prompt =
$@"Sinu ülesandeks on kontrollida õpilase tõlget inglise keelest eesti keelde.
Ignoreeri oma tšekis suurtähti (ülemine või alumine) ja kirjavahemärke.
Kasuta väljundis alati korrektset eesti õigekirja (ä, ö, õ, ü, š, ž). Ära asenda täpitähti numbrite ega muude sümbolitega.
Teie sisend on JSON-objekt:
{JsonSchemaRecord.For(typeof(Input))}";

        private readonly OpenAiClient openAiClient;
        private readonly TelemetryClient telemetry;

        public CheckEtTranslationCommand(TelemetryClient telemetry, OpenAiClient openAiClient)
        {
            this.telemetry = telemetry;
            this.openAiClient = openAiClient;
        }

        public virtual async Task<Result<EtTranslationCheckResult>> Invoke(string etSentence, string enSentence, string etExpectedSentence)
        {
            var input = JsonSerializer.Serialize(new Input
            {
                EnSentence = enSentence.Trim('.', ' ').ToLowerInvariant(),
                EtUserSentence = etSentence.Trim('.', ' ').ToLowerInvariant(),
                EtExpectedSentence = etExpectedSentence.Trim('.', ' ').ToLowerInvariant(),
            }, new JsonSerializerOptions
            {
                WriteIndented = false,
                Encoder = JavaScriptEncoder.Create(UnicodeRanges.All),
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

            // Canonical Estonian/reference fields come from local truth, not the AI echo,
            // which can garble diacritics (e.g. "värske" -> "ve4rske"). Only Match and EnComment are trusted from the model.
            if (result.IsSuccess)
            {
                return Result.Success(result.Value with
                {
                    EnSentence = enSentence,
                    EtUserSentence = etSentence,
                    EtExpectedSentence = etExpectedSentence,
                });
            }

            return result;
        }

        public struct Input
        {
            [Description("The sentence shown to the user for translation to Estonian")]
            public string EnSentence { get; init; }

            [Description("Kasutaja poolt tehtud tõlge eesti keelde")]
            public string EtUserSentence { get; init; }

            [Description("Eeldatav eestikeelne lause")]
            public string EtExpectedSentence { get; init; }
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
            EtUserSentence = etSentence,
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