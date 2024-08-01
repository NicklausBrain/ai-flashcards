using System.Net.Http.Headers;

namespace My1kWordsEe.Services
{
    public class TartuNlpService
    {
        public async Task<Stream> GetSpeech(string text)
        {
            using HttpClient client = new HttpClient();

            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, "https://api.tartunlp.ai/text-to-speech/v2");

            request.Headers.Add("accept", "audio/wav");

            request.Content = new StringContent($"{{\n\"text\": \"{text}\",\n\"speaker\": \"mari\",\n\"speed\": 0.64\n}}");
            request.Content.Headers.ContentType = MediaTypeHeaderValue.Parse("application/json; charset=utf-8");

            HttpResponseMessage response = await client.SendAsync(request);

            var stream = await response.Content.ReadAsStreamAsync();
            return stream;
        }
    }
}
