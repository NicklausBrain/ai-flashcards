using System.Text.Json;
using System.Text.Json.Serialization;

using CSharpFunctionalExtensions;

using My1kWordsEe.Models;
using My1kWordsEe.Services.Db;

namespace My1kWordsEe.Services.Cqs
{
    public class AddSampleSentenceCommand
    {
        public const int MaxSamples = 6;

        private readonly AzureStorageClient azureBlobClient;
        private readonly OpenAiClient openAiClient;
        private readonly AddAudioCommand addAudioCommand;
        private readonly StabilityAiClient stabilityAiClient;

        public AddSampleSentenceCommand(
            AzureStorageClient azureBlobService,
            OpenAiClient openAiService,
            AddAudioCommand createAudioCommand,
            StabilityAiClient stabilityAiService)
        {
            this.azureBlobClient = azureBlobService;
            this.openAiClient = openAiService;
            this.addAudioCommand = createAudioCommand;
            this.stabilityAiClient = stabilityAiService;
        }

        public async Task<Result<SampleWord>> Invoke(SampleWord word)
        {
            if (word.Samples.Length >= MaxSamples)
            {
                return Result.Failure<SampleWord>($"Too many samples. {MaxSamples} is a maximum");
            }

            var sentence = await this.GetSampleSentence(word);

            if (sentence.IsFailure)
            {
                return Result.Failure<SampleWord>($"Sentence generation failed: {sentence.Error}");
            }

            var imageGeneration = this.GenerateImage(sentence.Value);
            var speechGeneration = this.GenerateSpeech(sentence.Value);
            await Task.WhenAll(imageGeneration, speechGeneration);

            if (imageGeneration.Result.IsFailure)
            {
                return Result.Failure<SampleWord>($"Image generation failed: {imageGeneration.Result.Error}");
            }

            if (speechGeneration.Result.IsFailure)
            {
                return Result.Failure<SampleWord>($"Speech generation failed: {speechGeneration.Result.Error}");
            }

            var updatedWordData = word with
            {
                Samples = word.Samples.Append(new SampleSentence
                {
                    EeWord = word.EeWord,
                    EeSentence = sentence.Value.Ee,
                    EnSentence = sentence.Value.En,
                    EeAudioUrl = speechGeneration.Result.Value,
                    ImageUrl = imageGeneration.Result.Value,
                }).ToArray()
            };

            return (await this.azureBlobClient
                .SaveWordData(updatedWordData))
                .Bind(r => Result.Success(updatedWordData));
        }

        private Task<Result<Uri>> GenerateImage(Sentence sentence) =>
            this.openAiClient.GetDallEPrompt(sentence.En).Bind(
            this.stabilityAiClient.GenerateImage).Bind(
            this.azureBlobClient.SaveImage);

        private Task<Result<Uri>> GenerateSpeech(Sentence sentence) =>
            this.addAudioCommand.Invoke(sentence.Ee);

        private async Task<Result<Sentence>> GetSampleSentence(SampleWord word)
        {
            var prompt =
                "Sa oled keele�ppe s�steemi abiline, mis aitab �ppida enim levinud eesti keele s�nu.\n" +

                "Teie sisend on JSON-objekt:" +
                "```\n{\n" +
                "\"EeWord\": \"<eestikeelne s�na>\", " +
                "\"EnWord\": \"<default english translation>\n" +
                "\"EnExplanation\": \"<explanation of the estonian word in english>\n" +
                "}\n```\n" +

                "Sinu sisend on �ks eestikeelne s�na ja selle rakenduse kontekst: <s�na> (<kontekst>).\n" +
                "Sinu �lesanne on kirjutada selle kasutamise kohta lihtne l�hike n�itelause, kasutades seda s�na.\n" +
                "Lauses kasuta k�ige levinuimaid ja lihtsamaid s�nu eesti keeles et toetada keele�pet.\n" +
                "Eelistan SVO-lausete s�naj�rge, kus esikohal on subjekt (S), seej�rel tegus�na (V) ja objekt (O)\n" +
                "Lausel peaks olema praktiline tegelik elu m�te\n" +
                "Teie v�ljundiks on JSON-objekt koos eestikeelse n�idislausega ja sellele vastav t�lge inglise keelde vastavalt lepingule:\n" +
                "```\n{\n" +
                "\"ee_sentence\": \"<n�ide eesti keeles>\", \"en_sentence\": \"<n�ide inglise keeles>\"" +
                "\n}\n```\n";

            var input = JsonSerializer.Serialize(new
            {
                word.EeWord,
                word.EnWord,
                word.EnExplanation
            });

            var result = await this.openAiClient.CompleteJsonAsync<Sentence>(prompt, input, temperature: 0.7f);

            return result;
        }

        private class Sentence
        {
            [JsonPropertyName("ee_sentence")]
            public required string Ee { get; set; }

            [JsonPropertyName("en_sentence")]
            public required string En { get; set; }
        }
    }
}
