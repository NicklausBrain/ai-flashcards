using System.Text.Json;
using System.Text.Json.Serialization;

using CSharpFunctionalExtensions;

namespace My1kWordsEe.Services.Cqs
{
    public class CheckEeListeningCommand
    {
        private readonly OpenAiClient openAiClient;

        public CheckEeListeningCommand(OpenAiClient openAiClient)
        {
            this.openAiClient = openAiClient;
        }

        public virtual async Task<Result<EeListeningCheckResult>> Invoke(string eeSentence, string userInput)
        {
            var prompt = "Your task is to check user's listening to Estonian speech.\n" +
                         "Ignore the letters case (upper or lower) and termination symbols in your check.\n" +
                         "Ignore the  diacritical marks.\n" +
                         $"Your input is JSON object:\n" +
                         "```\n{\n" +
                         "\"ee_sentence\": \"<eestikeelne lause>\", \"ee_user_sentence\": \"<mida kasutaja ära tundis>\n" +
                         "}\n```\n" +
                         "Your outpur is JSON object:\n" +
                         "```\n{\n" +
                         "\"ee_sentence\": \"<eestikeelne lause>\",\n" +
                         "\"ee_user_sentence\": \"<mida kasutaja ära tundis>\",\n" +
                         "\"en_sentence\": \"<translation to English>\",\n" +
                         "\"en_comment\": \"<comment explaining to the student his mistake (if any)>\",\n" +
                         "\"match_level\": <correctes level in integer from 0 to 5>\n" +
                         "}\n```\n";

            var input = JsonSerializer.Serialize(new
            {
                ee_sentence = eeSentence.Trim('.', ' ').ToLowerInvariant(),
                ee_user_sentence = userInput.Trim('.', ' ').ToLowerInvariant(),
            });

            var result = await this.openAiClient.CompleteJsonAsync<EeListeningCheckResult>(prompt, input);

            return result;
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
}