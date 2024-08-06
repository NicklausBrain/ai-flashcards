using CSharpFunctionalExtensions;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace My1kWordsEe.Services
{
    public class StabilityAiService
    {
        public const string ApiHost = "https://api.stability.ai";
        private readonly string engineId = "stable-diffusion-v1-6";
        private readonly string apiKey;

        public StabilityAiService(string apiKey)
        {
            this.apiKey = apiKey;
        }

        public async Task<Result<MemoryStream>> GenerateImage(string prompt)
        {
            if (string.IsNullOrEmpty(apiKey))
            {
                return Result.Failure<MemoryStream>("API key is missing");
            };

            using HttpClient client = new HttpClient();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", apiKey);

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

            var response = await client.PostAsync(
                $"{ApiHost}/v1/generation/{engineId}/text-to-image",
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
                for (int index = 0; index < generationResponse.Artifacts.Count; index++)
                {
                    var image = generationResponse.Artifacts[index];

                    return Result.Success(new MemoryStream(Convert.FromBase64String(image.Base64)));
                }
            }

            return Result.Failure<MemoryStream>("No artifacts found in response");
        }

        public class GenerationResponse
        {
            [JsonPropertyName("artifacts")]
            public List<Artifact> Artifacts { get; set; }
        }

        public class Artifact
        {
            [JsonPropertyName("base64")]
            public string Base64 { get; set; }

            //throws exception
            //[JsonPropertyName("seed")]
            //public int Seed { get; set; }

            [JsonPropertyName("finishReason")]
            public string FinishReason { get; set; }
        }
    }
}
