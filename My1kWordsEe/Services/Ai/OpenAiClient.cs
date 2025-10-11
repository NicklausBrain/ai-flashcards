using System.Text.Json;

using CSharpFunctionalExtensions;

using My1kWordsEe.Models;

using OpenAI.Chat;

namespace My1kWordsEe.Services
{
    public class OpenAiClient
    {
        public const string ApiSecretKey = "Secrets:OpenAiKey";
        public const string Model = "gpt-4.1";

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
            }

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
                return Result.Failure<string>("An error occurred while communicating with the AI service.");
            }
        }

        public async Task<Result<T>> CompleteJsonAsync<T>(string instructions, string input, float? temperature = null)
        {
            var response = await this.CompleteAsync(instructions, input, new ChatCompletionOptions
            {
                ResponseFormat = ChatResponseFormat.CreateJsonObjectFormat(),
                Temperature = temperature,
            });

            if (response.IsFailure)
            {
                return Result.Failure<T>(response.Error);
            }

            return this.ParseJsonResponse<T>(response);
        }

        public async Task<Result<T>> CompleteJsonSchemaAsync<T>(
            string instructions,
            string input,
            JsonSchemaRecord schema,
            float? temperature = null)
        {
            var response = await this.CompleteAsync(instructions, input, new ChatCompletionOptions
            {
                ResponseFormat = ChatResponseFormat.CreateJsonSchemaFormat(
                    typeof(T).Name,
                    schema.BinaryData,
                    // jsonSchemaFormatDescription: instructions,
                    jsonSchemaIsStrict: false),
                Temperature = temperature,
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
@"You are part of a language learning system.
Your task is to generate a DALL-E prompt for illustrating the given sentence.
The generated image should be a sketchy illustration in shades of blue, black, and white.
The focus should be on accurately depicting the action and elements described in the sentence while maintaining artistic simplicity.
If the sentence contains a human or an animal, ensure it has the correct number of limbs, eyes, and other defining body parts.
Avoid unnecessary elements that could distract from the core meaning of the sentence.
Your response must be only the DALL-E prompt, formatted as a plain string.";

            var result = await openAiClient.CompleteAsync(prompt, sentence, new ChatCompletionOptions
            {
                ResponseFormat = ChatResponseFormat.CreateTextFormat(),
                Temperature = 0.7f,
            });

            return result;
        }
    }
}