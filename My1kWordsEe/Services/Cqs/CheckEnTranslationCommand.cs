using System.Text.Json;
using System.Text.Json.Serialization;

using CSharpFunctionalExtensions;

namespace My1kWordsEe.Services.Cqs
{
    public class CheckEnTranslationCommand
    {
        private readonly OpenAiClient openAiClient;

        public CheckEnTranslationCommand(OpenAiClient openAiClient)
        {
            this.openAiClient = openAiClient;
        }

        public virtual async Task<Result<EnTranslationCheckResult>> Invoke(string eeSentence, string enSentence)
        {
            var prompt = "Your task is to check user's translation from Estonian into English.\n" +
                         "Ignore the letters case (upper or lower) and punctuation symbols in your check.\n" +
                         $"Your input is JSON object:\n" +
                         "```\n{\n" +
                         "\"ee_sentence\": \"<eestikeelne lause>\", \"en_user_sentence\": \"<user translation to English>\n" +
                         "}\n```\n" +
                         "Your outpur is JSON object:\n" +
                         "```\n{\n" +
                         "\"ee_sentence\": \"<eestikeelne lause>\",\n" +
                         "\"en_user_sentence\": \"<user translation to English>\",\n" +
                         "\"en_expected_sentence\": \"<expected translation to English>\",\n" +
                         "\"en_comment\": \"<comment explaining to the student his mistake (if any)>\",\n" +
                         "\"match_level\": <correctes level in integer from 0 to 5>\n" +
                         "}\n```\n";

            var input = JsonSerializer.Serialize(new
            {
                ee_sentence = eeSentence.Trim('.', ' ').ToLowerInvariant(),
                en_user_sentence = enSentence.Trim('.', ' ').ToLowerInvariant(),
            });

            var result = await this.openAiClient.CompleteJsonAsync<EnTranslationCheckResult>(prompt, input);

            return result;
        }
    }

    public record EnTranslationCheckResult
    {
        [JsonPropertyName("ee_sentence")]
        public required string EeSentence { get; init; }

        [JsonPropertyName("en_user_sentence")]
        public required string EnUserSentence { get; init; }

        [JsonPropertyName("en_expected_sentence")]
        public required string EnExpectedSentence { get; init; }

        [JsonPropertyName("en_comment")]
        public required string EnComment { get; init; } = string.Empty;

        [JsonPropertyName("match_level")]
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