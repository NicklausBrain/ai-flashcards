using CSharpFunctionalExtensions;

using My1kWordsEe.Models;
using My1kWordsEe.Models.Semantics;
using My1kWordsEe.Services.Db;

using static My1kWordsEe.Models.Extensions;

namespace My1kWordsEe.Services.Cqs.Et
{
    public class AddEtWordCommand
    {
        private readonly OpenAiClient openAiClient;
        private readonly AzureStorageClient azureBlobClient;
        private readonly AddAudioCommand addAudioCommand;

        public static readonly string Prompt =
            "See on keeleõppe süsteem.\n" +
            "Teie sisestus on eestikeelne sõna (ja ainult eestikeelne sõna).\n" +
            "Ärge lisage ingliskeelsetel homonüümidel põhinevaid sõnade tähendusi.\n" +
            "Sõna tähendused peavad pärinema ainult eesti keelest.\n" +
            "Iga sõnafunktsiooni tuleks kirjeldada ainult ühes kirjes.\n" +
            "Kui mitu kirjeldust selgitavad sama tähendust, ühendage need üheks selgituseks.\n" +
            "Teie väljund on JSON-objekt vastavalt antud skeemile.\n";

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
            var response = await this.openAiClient.CompleteJsonSchemaAsync<WordSenses>(
                Prompt,
                etWord,
                GetJsonSchema(typeof(WordSenses)),
                temperature: 0.1f);

            if (response.IsFailure)
            {
                return Result.Failure<EtWord>(response.Error);
            }

            return Result.Success(new EtWord
            {
                // todo: make it nicer than that
                Value = etWord.Trim().ToLower(),
                Senses = response.Value.Array
            });
        }
    }
}