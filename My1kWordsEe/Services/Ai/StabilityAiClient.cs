using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

using CSharpFunctionalExtensions;

namespace My1kWordsEe.Services
{
    /// <summary>
    /// Facade for https://platform.stability.ai/docs/getting-started/stable-image
    /// </summary>
    public class StabilityAiClient
    {
        public const string ApiHost = "https://api.stability.ai";
        public const string ApiSecretKey = "Secrets:StabilityAiKey";

        private const string EngineId = "stable-diffusion-v1-6";

        private readonly IConfiguration config;
        private readonly IHttpClientFactory httpClientFactory;
        private readonly ILogger<StabilityAiClient> logger;

        public StabilityAiClient(
            IConfiguration config,
            IHttpClientFactory httpClientFactory,
            ILogger<StabilityAiClient> logger)
        {
            this.config = config;
            this.httpClientFactory = httpClientFactory;
            this.logger = logger;
        }

        public async Task<Result<MemoryStream>> GenerateImage(string prompt)
        {
            if (string.IsNullOrWhiteSpace(this.config[ApiSecretKey]))
            {
                return Result.Failure<MemoryStream>("Stability AI API key is missing");
            };

            using HttpClient client = httpClientFactory.CreateClient(nameof(StabilityAiClient));
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", this.config[ApiSecretKey]);

            var requestBody = new
            {
                text_prompts = new[]
                {
                    new { text = prompt }
                },
                cfg_scale = 7,
                height = 512,
                width = 512,
                steps = 30,
                samples = 1
            };

            try
            {
                var response = await client.PostAsync(
                    $"{ApiHost}/v1/generation/{EngineId}/text-to-image",
                    new StringContent(JsonSerializer.Serialize(requestBody), Encoding.UTF8, "application/json")
                );

                if (!response.IsSuccessStatusCode)
                {
                    return Result.Failure<MemoryStream>($"Non-200 response: {await response.Content.ReadAsStringAsync()}");
                }

                var responseContent = await response.Content.ReadAsStringAsync();
                var generationResponse = JsonSerializer.Deserialize<GenerationResponse>(responseContent);

                if (generationResponse?.Artifacts != null)
                {
                    for (int index = 0; index < generationResponse.Artifacts.Count;)
                    {
                        var image = generationResponse.Artifacts[index];

                        return Result.Success(new MemoryStream(Convert.FromBase64String(image.Base64)));
                    }
                }
            }
            catch (Exception exception)
            {
                this.logger.LogError(exception, "Error calling Stability AI API");
                return Result.Failure<MemoryStream>(exception.Message);
            }

            return Result.Failure<MemoryStream>("No artifacts found in response");
        }

        private class GenerationResponse
        {
            [JsonPropertyName("artifacts")]
            public required List<Artifact> Artifacts { get; set; } = new List<Artifact>();
        }

        private class Artifact
        {
            [JsonPropertyName("base64")]
            public required string Base64 { get; set; }

            [JsonPropertyName("finishReason")]
            public required string FinishReason { get; set; }
        }
    }
}

