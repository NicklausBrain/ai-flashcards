using System.ComponentModel;
using System.Text.Json;

using CSharpFunctionalExtensions;

using My1kWordsEe.Models;
using My1kWordsEe.Models.Semantics;
using My1kWordsEe.Services.Db;

using OpenAI.Chat;

using static My1kWordsEe.Models.Extensions;

namespace My1kWordsEe.Services.Cqs.Et
{
    public class AddEtWordCommand
    {
        private readonly OpenAiClient openAiClient;
        private readonly AzureStorageClient azureBlobClient;
        private readonly AddAudioCommand addAudioCommand;

        public static readonly string Prompt =
            "See on keeleõppe süsteem\n" +
            "Teie väljund on JSON-i massiiv vastavalt järgmisele skeemile:\n" +
            $"```\n{GetJsonSchema(typeof(WordSense[]))}\n```\n" +
            "Teie sisend on JSON-objekt vastavalt järgmisele skeemile:\n" +
            $"```\n{GetJsonSchema(typeof(Input))}\n```";

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
            var input = JsonSerializer.Serialize(new Input
            {
                EtWord = etWord,
            });

            var response = await this.openAiClient.CompleteAsync(
                Prompt,
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

            // todo: could be ommited if we integrate an EE dictionary within the app
            if (string.IsNullOrWhiteSpace(response.Value))
            {
                // todo: debug/check
                return Result.Failure<EtWord>("Not an Estonian word");
            }

            openAiClient.ParseJsonResponse<WordSense[]>(response).Deconstruct(
                out bool _,
                out bool isParsingError,
                out WordSense[] senses,
                out string parsingError);

            if (isParsingError)
            {
                return Result.Failure<EtWord>(parsingError);
            }

            return Result.Success(new EtWord
            {
                // todo: make it nicer than that
                Value = etWord.Trim().ToLower(),
                Senses = senses
            });
        }

        private class Input
        {
            [Description("Estonian word")]
            public required string EtWord { get; init; }
        }
    }
}