using System.ComponentModel;
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
Sisend: JSON-objekt sõna grammatilise vormi ja tõlgete kohta.
Teie ülesanne: 
1️. Täida kõik väljad vastavalt Eesti keele grammatikareeglitele.
2️. Kasuta korrektseid küsisõnu (ainsus ja mitmus) vastavalt käänetele.
3️. Tagasta JSON, mis vastab allolevale skeemile.
Sisend:
{JsonSchemaRecord.For(typeof(EtWordSense))}
Väljund peab olema järgmine JSON-objekt:";

        public AddEtFormsCommand(
            OpenAiClient openAiService,
            FormsStorageClient formsStorageClient)
        {
            this.formsStorageClient = formsStorageClient;
            this.openAiClient = openAiService;
        }

        public async Task<Result<T>> Invoke<T>(EtWord word, uint senseIndex) where T : IGrammarForms
        {
            var sense = word.Senses[senseIndex];
            var containerId = new FormsContainerId { SenseIndex = senseIndex, BaseForm = sense.BaseForm };

            var forms = await this.GetFormsMetadata<T>(word.Senses[senseIndex]);

            if (forms.IsFailure)
            {
                return Result.Failure<T>(forms.Error);
            }

            await this.formsStorageClient.SaveFormsData<T>(containerId, forms.Value);
            return forms;
        }

        private async Task<Result<T>> GetFormsMetadata<T>(WordSense sense) where T : IGrammarForms
        {
            var response = await this.openAiClient.CompleteJsonSchemaAsync<T>(
                Prompt,
                JsonSerializer.Serialize(new EtWordSense
                {
                    EtWord = sense.Word.Et,
                    Definition = sense.Definition.Et,
                    BaseForm = sense.BaseForm,
                    PartOfSpeech = sense.PartOfSpeech.Et,
                }),
                JsonSchemaRecord.For(typeof(T)),
                temperature: 0.1f);

            if (response.IsFailure)
            {
                return Result.Failure<T>(response.Error);
            }

            return response.Value;
        }

        private struct EtWordSense
        {
            [Description("Sama antud sõna ja selle otsetõlge")]
            public required string EtWord { get; init; }

            [Description("Antud sõna tähenduse ja grammatilise vormi selgitus")]
            public required string Definition { get; init; }

            [Description("Sõna grammatika põhivorm")]
            public required string BaseForm { get; init; }

            [Description("Kõneosa")]
            public required string PartOfSpeech { get; init; }
        }
    }
}