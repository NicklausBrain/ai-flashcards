using System.Text.Json;
using System.Text.Json.Serialization;

using CSharpFunctionalExtensions;

using My1kWordsEe.Models;
using My1kWordsEe.Services.Db;

using OpenAI.Chat;

namespace My1kWordsEe.Services.Cqs
{
    public class AddSampleWordCommand
    {
        private readonly OpenAiClient openAiClient;
        private readonly AzureStorageClient azureBlobClient;
        private readonly AddAudioCommand addAudioCommand;

        public AddSampleWordCommand(
            OpenAiClient openAiService,
            AzureStorageClient azureBlobService,
            AddAudioCommand createAudioCommand)
        {
            this.azureBlobClient = azureBlobService;
            this.openAiClient = openAiService;
            this.addAudioCommand = createAudioCommand;
        }

        public async Task<Result<SampleWord>> Invoke(string eeWord, string? comment = null)
        {
            if (!eeWord.ValidateWord())
            {
                return Result.Failure<SampleWord>("Not an Estonian word");
            }

            (await this.GetWordMetadata(eeWord, comment)).Deconstruct(
                out bool _,
                out bool isAiFailure,
                out SampleWord sampleWord,
                out string aiError);

            if (isAiFailure)
            {
                return Result.Failure<SampleWord>(aiError);
            }

            (await this.addAudioCommand.Invoke(eeWord)).Deconstruct(
                out bool isAudioSaved,
                out bool _,
                out Uri audioUri);

            sampleWord = isAudioSaved
                ? sampleWord with { EeAudioUrl = audioUri }
                : sampleWord;

            return (await azureBlobClient.SaveWordData(sampleWord))
                .Bind(_ => Result.Of(sampleWord));
        }

        private async Task<Result<SampleWord>> GetWordMetadata(
           string eeWord,
           string? comment = null)
        {
            const string prompt =
                "Antud eestikeelse sõna kohta tuleb esitada metaandmed\n" +

                "Teie sisend on JSON-objekt:" +
                "```\n{\n" +
                "\"EeWord\": \"<eestikeelne sõna>\", " +
                "\"Comment\": \"<eestikeelse sõna valikuline selgitus eesti või inglise keeles>\n" +
                "}\n```\n" +

                "Kui antud sõna ei ole eestikeelne, tagasta 404\n" +
                "Teie väljund on sõna metaandmed JSON-is vastavalt antud lepingule:\n" +
                "```\n{\n" +
                "\"ee_word\": \"<antud sõna>\",\n" +
                "\"en_word\": \"<english translation>\",\n" +
                "\"en_words\": [<array of alternative english translations if applicable>],\n" +
                "\"en_explanation\": \"<explanation of the word meaning in english>\",\n" +
                "\"ee_explanation\": \"<sõna tähenduse seletus eesti keeles>\"\n" +
                "}\n```\n";

            var input = JsonSerializer.Serialize(new
            {
                EeWord = eeWord,
                Comment = comment
            });

            var response = await this.openAiClient.CompleteAsync(
                prompt,
                input,
                new ChatCompletionOptions
                {
                    ResponseFormat = ChatResponseFormat.CreateJsonObjectFormat(),
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
}