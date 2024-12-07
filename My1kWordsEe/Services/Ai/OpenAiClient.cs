using System.Text.Json;
using System.Text.Json.Serialization;

using CSharpFunctionalExtensions;

using My1kWordsEe.Models;

using OpenAI.Chat;

namespace My1kWordsEe.Services
{
    public class OpenAiClient
    {
        public const string ApiSecretKey = "Secrets:OpenAiKey";
        public const string Model = "gpt-4o";

        private readonly IConfiguration config;
        private readonly ILogger logger;

        public OpenAiClient(
            IConfiguration config,
            ILogger<OpenAiClient> logger)
        {
            this.config = config;
            this.logger = logger;
        }

        public async Task<Result<string>> CompleteAsync(string instructions, string input, ChatCompletionOptions? options = null)
        {
            if (string.IsNullOrWhiteSpace(this.config[ApiSecretKey]))
            {
                return Result.Failure<string>("Open AI API key is missing");
            };

            ChatClient client = new(model: Model, this.config[ApiSecretKey]);

            try
            {
                ChatCompletion chatCompletion = await client.CompleteChatAsync([
                    new SystemChatMessage(instructions),
                    new UserChatMessage(input)],
                    options);
                return Result.Success(chatCompletion.Content[0].Text);
            }
            catch (Exception exception)
            {
                this.logger.LogError(exception, "Error calling Open AI API");
                return Result.Failure<string>(exception.Message);
            }
        }

        public async Task<Result<T>> CompleteJsonAsync<T>(string instructions, string input)
        {
            var response = await this.CompleteAsync(instructions, input, new ChatCompletionOptions
            {
                ResponseFormat = ChatResponseFormat.JsonObject,
            });

            if (response.IsFailure)
            {
                return Result.Failure<T>(response.Error);
            }

            return this.ParseJsonResponse<T>(response);
        }

        public Result<T> ParseJsonResponse<T>(Result<string> textResult)
        {
            if (textResult.IsFailure)
            {
                return Result.Failure<T>(textResult.Error);
            }

            var jsonStr = textResult.Value;

            try
            {
                var result = JsonSerializer.Deserialize<T>(jsonStr);

                if (result == null)
                {
                    return Result.Failure<T>("Empty response");
                }

                return result;
            }
            catch (Exception jsonException)
            {
                this.logger.LogError(jsonException, "Failed to deserialize JSON: {jsonStr}", jsonStr);
                return Result.Failure<T>("Unexpected data returned by AI");
            }
        }
    }

    public static class OpenAiClientExtensions
    {
        public static async Task<Result<string>> GetDallEPrompt(this OpenAiClient openAiClient, string sentence)
        {
            const string prompt =
                "You are part of the language learning system.\n" +
                "Your task is to generate a DALL-E prompt so that it will create a picture to illustrate the sentence provided by the user.\n" +
                "The image should be sketchy, mostly shades of blue, black, and white.\n" +
                "Your response is a DALL-E prompt as a plain string.\n";

            return await openAiClient.CompleteAsync(prompt, sentence, new ChatCompletionOptions
            {
                ResponseFormat = ChatResponseFormat.Text,
                Temperature = (float)Math.PI / 2,
                // limit the number of tokens to avoid long prompts that crush stability ai
                MaxTokens = 400,
            });
        }

        public static async Task<Result<SampleWord>> GetWordMetadata(
            this OpenAiClient openAiClient,
            string eeWord,
            string? comment = null)
        {
            const string prompt =
                "Teie sisend on eestikeelne sõna (ja selle sõna valikuline selgitus).\n" +
                "Kui antud sõna ei ole eestikeelne, tagasta 404\n" +
                "Teie väljund on sõna metaandmed JSON-is vastavalt antud lepingule:\n" +
                "```\n{\n" +
                "\"ee_word\": \"<antud sõna>\",\n" +
                "\"en_word\": \"<english translation>\",\n" +
                "\"en_words\": [<array of alternative english translations if applicable>],\n" +
                "\"en_explanation\": \"<explanation of the word meaning in english>\",\n" +
                "\"ee_explanation\": \"<sõna tähenduse seletus eesti keeles>\"\n" +
                "}\n```\n";

            var response = await openAiClient.CompleteAsync(
                prompt,
                string.IsNullOrEmpty(comment)
                    ? eeWord
                    : $"{eeWord} ({comment})",
                new ChatCompletionOptions
                {
                    ResponseFormat = ChatResponseFormat.JsonObject,
                    Temperature = 0.333f
                });

            if (response.IsFailure)
            {
                return Result.Failure<SampleWord>(response.Error);
            }

            // could be ommited if we integrate an EE dictionary within the app
            if (response.Value.Contains("404"))
            {
                return Result.Failure<SampleWord>("Not an Estonian word");
            }

            openAiClient.ParseJsonResponse<WordMetadata>(response).Deconstruct(
                out bool _,
                out bool isParsingError,
                out WordMetadata wordMetadata,
                out string parsingError);

            if (isParsingError)
            {
                return Result.Failure<SampleWord>(parsingError);
            }

            return Result.Success(new SampleWord
            {
                EeWord = wordMetadata.EeWord,
                EnWord = wordMetadata.EnWord,
                EnWords = wordMetadata.EnWords,
                EnExplanation = wordMetadata.EnExplanation,
                EeExplanation = wordMetadata.EeExplanation,
            });
        }

        public static async Task<Result<Sentence>> GetSampleSentence(this OpenAiClient openAiClient, string eeWord, string explanation, string[]? existingSamples = null)
        {
            var prompt =
                "Sa oled keeleõppe süsteemi abiline, mis aitab õppida enim levinud eesti keele sõnu.\n" +
                "Sinu sisend on üks eestikeelne sõna ja selle rakenduse kontekst: <sõna> (<kontekst>).\n" +
                "Sinu ülesanne on kirjutada selle kasutamise kohta lihtne lühike näitelause, kasutades seda sõna.\n" +
                "Lauses kasuta kõige levinuimaid ja lihtsamaid sõnu eesti keeles et toetada keeleõpet.\n" +
                "Eelistan SVO-lausete sõnajärge, kus esikohal on subjekt (S), seejärel tegusõna (V) ja objekt (O)\n" +
                "Lausel peaks olema praktiline tegelik elu mõte\n" +
                "Teie väljundiks on JSON-objekt koos eestikeelse näidislausega ja sellele vastav tõlge inglise keelde vastavalt lepingule:\n" +
                "```\n{\n" +
                "\"ee_sentence\": \"<näide eesti keeles>\", \"en_sentence\": \"<näide inglise keeles>\"" +
                "\n}\n```\n" +
                ((existingSamples != null && existingSamples.Any())
                 ? "PS: Ärge korrake järgmisi näidiseid, olge erinevad:\n" + string.Join(",", existingSamples.Select(s => $"'{s}'"))
                 : string.Empty);

            return await openAiClient.CompleteJsonAsync<Sentence>(prompt, $"{eeWord} (${explanation})");
        }

        private class WordMetadata
        {
            [JsonPropertyName("ee_word")]
            public required string EeWord { get; set; }

            [JsonPropertyName("en_word")]
            public required string EnWord { get; set; }

            [JsonPropertyName("en_explanation")]
            public required string EnExplanation { get; set; }

            [JsonPropertyName("ee_explanation")]
            public required string EeExplanation { get; set; }

            [JsonPropertyName("en_words")]
            public required string[] EnWords { get; set; } = Array.Empty<string>();
        }
    }

    public class Sentence
    {
        [JsonPropertyName("ee_sentence")]
        public required string Ee { get; set; }

        [JsonPropertyName("en_sentence")]
        public required string En { get; set; }
    }
}
