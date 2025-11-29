using System.Net.Http.Headers;

using CSharpFunctionalExtensions;

namespace My1kWordsEe.Services
{
    /// <summary>
    /// Facade for https://neurokone.ee/.
    /// </summary>
    public class TartuNlpClient
    {
        private readonly IHttpClientFactory httpClientFactory;
        private readonly ILogger<TartuNlpClient> logger;

        public TartuNlpClient(
            IHttpClientFactory httpClientFactory,
            ILogger<TartuNlpClient> logger)
        {
            this.httpClientFactory = httpClientFactory;
            this.logger = logger;
        }

        public async Task<Result<Stream>> GetSpeech(string text)
        {
            using HttpClient client = this.httpClientFactory.CreateClient(nameof(TartuNlpClient));

            HttpRequestMessage request = new(HttpMethod.Post, "https://api.tartunlp.ai/text-to-speech/v2");

            request.Headers.Add("accept", "audio/wav");

            request.Content = new StringContent(
                $"{{\n\"text\": \"{text}\",\n\"speaker\": \"mari\",\n\"speed\": 0.64\n}}");
            request.Content.Headers.ContentType = MediaTypeHeaderValue.Parse("application/json; charset=utf-8");

            try
            {
                HttpResponseMessage response = await client.SendAsync(request);

                if (response.IsSuccessStatusCode)
                {
                    var stream = await response.Content.ReadAsStreamAsync();
                    return Result.Success(stream);
                }
                else
                {
                    var errorStr = await response.Content.ReadAsStringAsync();
                    this.logger.LogError(
                        "Tartu NLP HTTP error. Reason phrase: {reason}. Content: {content}",
                        response.ReasonPhrase,
                        errorStr);
                    return Result.Failure<Stream>($"Tartu NLP HTTP error. {response.ReasonPhrase}. {errorStr}");
                }
            }
            catch (Exception httpException)
            {
                this.logger.LogError(httpException, "Tartu NLP HTTP exception");
                return Result.Failure<Stream>(httpException.Message);
            }
        }
    }
}