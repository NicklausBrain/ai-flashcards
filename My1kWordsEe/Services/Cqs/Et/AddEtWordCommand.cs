using System.Text.Json;
using System.Text.Json.Serialization;

using CSharpFunctionalExtensions;

using My1kWordsEe.Models;
using My1kWordsEe.Models.Semantics;
using My1kWordsEe.Services.Db;

using OpenAI.Chat;

namespace My1kWordsEe.Services.Cqs.Et
{
    public class AddEtWordCommand
    {
        private readonly OpenAiClient openAiClient;
        private readonly AzureStorageClient azureBlobClient;
        private readonly AddAudioCommand addAudioCommand;

        public AddEtWordCommand(
            OpenAiClient openAiService,
            AzureStorageClient azureBlobService,
            AddAudioCommand createAudioCommand)
        {
            this.azureBlobClient = azureBlobService;
            this.openAiClient = openAiService;
            this.addAudioCommand = createAudioCommand;
        }

        public async Task<Result<EtWord>> Invoke(string eeWord, string? comment = null)
        {
            if (!eeWord.ValidateWord())
            {
                return Result.Failure<EtWord>("Not an Estonian word");
            }

            (await this.GetWordMetadata(eeWord)).Deconstruct(
                out bool _,
                out bool isAiFailure,
                out EtWord etWord,
                out string aiError);

            if (isAiFailure)
            {
                return Result.Failure<EtWord>(aiError);
            }

            (await this.addAudioCommand.Invoke(eeWord)).Deconstruct(
                out bool isAudioSaved,
                out bool _,
                out Uri audioUri);

            etWord = isAudioSaved
                ? etWord with { AudioUrl = audioUri }
                : etWord;

            return (await azureBlobClient.SaveEtWordData(etWord))
                .Bind(_ => Result.Of(etWord));
        }

        private async Task<Result<EtWord>> GetWordMetadata(string etWord)
        {
            const string prompt =
                // Metadata must be provided for a given Estonian word.
                "Antud eestikeelse sõna kohta tuleb esitada metaandmed\n" +
                // Your input is a JSON object
                "Teie sisend on JSON-objekt:\n" +
                "```\n{\n" +
                "\"EtWord\": \"<eestikeelne sõna>\" " +
                "\n}\n```\n" +
                // If the given word is not in Estonian, return 404
                "Kui antud sõna ei ole eestikeelne, tagasta 404\n" +
                // Your output is the word metadata in JSON according to the given contract
                "Teie väljund on sõna metaandmed JSON-is vastavalt antud lepingule:\n" +
                // IT SHOULD BE ARRAY OF WORD SENESES
                "```\n[\n{" +
                $"\"{nameof(WordSense.BaseForm)}\": \"<antud sõna>\",\n" +
                "\"en_word\": \"<english translation>\",\n" +
                "\"en_words\": [<array of alternative english translations if applicable>],\n" +
                "\"en_explanation\": \"<explanation of the word meaning in english>\",\n" +
                "\"ee_explanation\": \"<sõna tähenduse seletus eesti keeles>\"\n" +
                "}]\n```\n";

            var input = JsonSerializer.Serialize(new
            {
                EtWord = etWord,
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
                return Result.Failure<EtWord>(response.Error);
            }

            // could be ommited if we integrate an EE dictionary within the app
            if (response.Value.Contains("404"))
            {
                return Result.Failure<EtWord>("Not an Estonian word");
            }

            openAiClient.ParseJsonResponse<WordMetadata>(response).Deconstruct(
                out bool _,
                out bool isParsingError,
                out WordMetadata wordMetadata,
                out string parsingError);

            if (isParsingError)
            {
                return Result.Failure<EtWord>(parsingError);
            }

            return Result.Success(new EtWord
            {
                Value = etWord.Normalize(),

                // EeWord = wordMetadata.EeWord,
                // EnWord = wordMetadata.EnWord,
                // EnWords = wordMetadata.EnWords,
                // EnExplanation = wordMetadata.EnExplanation,
                // EeExplanation = wordMetadata.EeExplanation,
            });
        }

        private class Input
        {
            public string EtWord { get; }

            public string Serialize() => JsonSerializer.Serialize(this);
        }

        [Obsolete]
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