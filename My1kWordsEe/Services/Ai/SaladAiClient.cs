using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

using CSharpFunctionalExtensions;

namespace My1kWordsEe.Services.Ai
{
    public class SaladAiClient
    {

        private readonly IHttpClientFactory httpClientFactory;
        private readonly ILogger<StabilityAiClient> logger;

        public SaladAiClient(
            IHttpClientFactory httpClientFactory,
            ILogger<StabilityAiClient> logger)
        {
            this.httpClientFactory = httpClientFactory;
            this.logger = logger;
        }

        private class GenerationResponse
        {
            [JsonPropertyName("images")]
            public required List<string> images { get; set; } = new List<string>();
        }

        public async Task<Result<MemoryStream>> GenerateImage(string prompt)
        {
            //if (string.IsNullOrWhiteSpace(this.config[ApiSecretKey]))
            //{
            //    return Result.Failure<MemoryStream>("Stability AI API key is missing");
            //};

            using HttpClient client = httpClientFactory.CreateClient(nameof(StabilityAiClient));
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            client.DefaultRequestHeaders.Add("Salad-Api-Key", "xxx");

            var requestBody = new
            {
                input = new
                {
                    prompt = prompt
                }
            };

            try
            {
                var response = await client.PostAsync(
                    $"https://tahini-tubers-1aqj2to1zzr3hgk6.salad.cloud/workflow/flux/txt2img",
                    new StringContent(JsonSerializer.Serialize(requestBody), Encoding.UTF8, "application/json")
                );

                if (!response.IsSuccessStatusCode)
                {
                    return Result.Failure<MemoryStream>($"Non-200 response: {await response.Content.ReadAsStringAsync()}");
                }

                var responseContent = await response.Content.ReadAsStringAsync();
                var generationResponse = JsonSerializer.Deserialize<GenerationResponse>(responseContent);

                if (generationResponse?.images != null)
                {
                    for (int index = 0; index < generationResponse.images.Count;)
                    {
                        return Result.Success(new MemoryStream(Convert.FromBase64String(generationResponse.images[0])));
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

    }
}
