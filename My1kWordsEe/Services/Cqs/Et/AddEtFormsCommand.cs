using System.Text.Json;

using CSharpFunctionalExtensions;

using My1kWordsEe.Models;
using My1kWordsEe.Models.Grammar.Forms;
using My1kWordsEe.Models.Semantics;
using My1kWordsEe.Services.Db;

using static My1kWordsEe.Services.Db.FormsStorageClient;

namespace My1kWordsEe.Services.Cqs.Et
{
    public class AddEtFormsCommand
    {
        private readonly OpenAiClient openAiClient;
        private readonly FormsStorageClient formsStorageClient;

        public static readonly string Prompt =
@$"See on keeleõppe süsteem.
Teie sisend on:
```{JsonSchemaRecord.For(typeof(WordSense))}```
Väljund peab olema JSON-objekt vastavalt antud skeemile.";

        public AddEtFormsCommand(
            OpenAiClient openAiService,
            FormsStorageClient formsStorageClient)
        {
            this.formsStorageClient = formsStorageClient;
            this.openAiClient = openAiService;
        }

        public async Task<Result<T>> Invoke<T>(EtWord word, uint senseIndex) where T : IGrammarForms
        {
            var containerId = new FormsContainerId { SenseIndex = senseIndex, Word = word.Value };

            var forms = await this.GetFormsMetadata<T>(word.Senses[senseIndex]);

            if (forms.IsFailure)
            {
                return Result.Failure<T>(forms.Error);
            }

            await this.formsStorageClient.SaveFormsData(containerId, forms.Value);
            return forms;
        }

        private async Task<Result<T>> GetFormsMetadata<T>(WordSense sense) where T : IGrammarForms
        {
            var response = await this.openAiClient.CompleteJsonSchemaAsync<T>(
                Prompt,
                JsonSerializer.Serialize(sense),
                JsonSchemaRecord.For(typeof(T)),
                temperature: 0.1f);

            if (response.IsFailure)
            {
                return Result.Failure<T>(response.Error);
            }

            return response.Value;
        }
    }
}