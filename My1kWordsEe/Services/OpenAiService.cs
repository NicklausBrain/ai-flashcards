using System.Text.Json;
using System.Text.Json.Serialization;

using CSharpFunctionalExtensions;

using My1kWordsEe.Models;

using OpenAI.Chat;
using OpenAI.Images;

namespace My1kWordsEe.Services
{
    public class OpenAiService
    {
        public OpenAiService(string apiKey)
        {
            ApiKey = apiKey;
        }

        private string ApiKey { get; }

        public async Task<Result<string>> CompleteAsync(string instructions, string input)
        {
            ChatClient client = new(model: "gpt-4o-mini", ApiKey);

            try
            {
                ChatCompletion chatCompletion = await client.CompleteChatAsync([
                    new SystemChatMessage(instructions),
                    new UserChatMessage(input)]);
                return Result.Success(chatCompletion.Content[0].Text);
            }
            catch (Exception e)
            {
                return Result.Failure<string>(e.Message);
            }
        }

        public async Task<Result<string>> GetDallEPrompt(string sentence)
        {
            ChatClient client = new(model: "gpt-4o-mini", ApiKey);

            try
            {
                ChatCompletion chatCompletion = await client.CompleteChatAsync(
                    [
                        new SystemChatMessage(
                        "You are part of the language learning system.\n" +
                        "Your task is to generate a DALL-E prompt so that it will create a picture to illustrate the sentence provided by the user.\n" +
                        "The image should be sketchy, mostly shades of blue, black, and white.\n" +
                        "Your response is a DALL-E prompt as a plain string.\n"),
                    new UserChatMessage(sentence)]);
                return Result.Success(chatCompletion.Content[0].Text);
            }
            catch (Exception e)
            {
                return Result.Failure<string>(e.Message);
            }
        }

        public async Task<Result<Uri>> GetSampleImageUri(string sentence)
        {
            ImageClient client = new(model: "dall-e-3", ApiKey);
            var prompt = await this.GetDallEPrompt(sentence);

            if (prompt.IsFailure)
            {
                return Result.Failure<Uri>(prompt.Error);
            }

            try
            {
                var imageResponse = await client.GenerateImageAsync(prompt.Value, new ImageGenerationOptions
                {
                    Quality = GeneratedImageQuality.Standard,
                    Size = GeneratedImageSize.W1024xH1024,
                    Style = GeneratedImageStyle.Natural,
                    ResponseFormat = GeneratedImageFormat.Uri,
                });

                return Result.Success(imageResponse.Value.ImageUri);
            }
            catch (Exception e)
            {
                return Result.Failure<Uri>(e.Message);
            }
        }

        public async Task<Result<Sentence>> GetSampleSentence(string eeWord)
        {
            ChatClient client = new(model: "gpt-4o-mini", ApiKey);

            ChatCompletion chatCompletion = await client.CompleteChatAsync(
                [
                    new SystemChatMessage(
                        "Sa oled keeleõppe süsteemi abiline, mis aitab õppida enim levinud eesti keele sõnu. " +
                        "Sinu sisend on üks sõna eesti keeles. " +
                        "Sinu ülesanne on kirjutada selle kasutamise kohta lihtne lühike näitelause, kasutades seda sõna. " +
                        "Lauses kasuta kõige levinuimaid ja lihtsamaid sõnu eesti keeles et toetada keeleõpet. " +
                        "Sinu väljund on JSON objekt, milles on näitelaus eesti keeles ja selle vastav tõlge inglise keelde:\n" +
                        "```\n" +
                        "{\"ee_sentence\": \"<näide eesti keeles>\", \"en_sentence\": \"<näide inglise keeles>\" }" +
                        "\n```" +
                        "\n Tagastab ainult json-objekti!"),
                    new UserChatMessage(eeWord),
                ]);

            foreach (var c in chatCompletion.Content)
            {
                var jsonStr = c.Text.Trim('`', ' ', '\'', '"');
                var sentence = JsonSerializer.Deserialize<Sentence>(jsonStr);
                if (sentence == null)
                {
                    break;
                }
                else
                {
                    return Result.Success(sentence);
                }
            }

            return Result.Failure<Sentence>("Empty response");
        }

        public async Task<Result<SampleWord>> GetWordMetadata(string word)
        {
            ChatClient client = new(model: "gpt-4o-mini", ApiKey);

            ChatCompletion chatCompletion = await client.CompleteChatAsync(
                [
                    new SystemChatMessage(
                        "Your input is an Estonian word. " +
                        "Your output is word metadata in JSON:\n" +
                        "```\n{\n" +
                        "ee_word: \"<given word>\",\n" +
                        "en_word: \"<english translation>\"\n" +
                        "en_words: [<alternative translations if applicable>]\n" +
                        "en_explanation: \"<explanation of the word in english>\"\n" +
                        "}\n```\n" +
                        "If the given word is not Estonian return 404"),
                    new UserChatMessage(word),
                ]);

            foreach (var c in chatCompletion.Content)
            {
                var jsonStr = c.Text.Trim('`', ' ', '\'', '"');

                if (jsonStr.Contains("404"))
                {
                    return Result.Failure<SampleWord>("Not an Estonian word");
                }

                try
                {
                    var wordMetadata = JsonSerializer.Deserialize<WordMetadata>(jsonStr);
                    if (wordMetadata == null)
                    {
                        break;
                    }
                    else
                    {
                        return Result.Success(new SampleWord
                        {
                            EeWord = wordMetadata.EeWord,
                            EnWord = wordMetadata.EnWord,
                            EnWords = wordMetadata.EnWords,
                            EnExplanation = wordMetadata.EnExplanation,
                        });
                    }
                }
                catch (JsonException)
                {
                    return Result.Failure<SampleWord>("Unexpected data returned by AI");
                }
            }

            return Result.Failure<SampleWord>("Empty response");
        }
    }
}

public class Sentence
{
    [JsonPropertyName("ee_sentence")]
    public string Ee { get; set; }

    [JsonPropertyName("en_sentence")]
    public string En { get; set; }
}

public class WordMetadata
{
    [JsonPropertyName("ee_word")]
    public string EeWord { get; set; }

    [JsonPropertyName("en_word")]
    public string EnWord { get; set; }

    [JsonPropertyName("en_explanation")]
    public string EnExplanation { get; set; }

    [JsonPropertyName("en_words")]
    public string[] EnWords { get; set; }
}