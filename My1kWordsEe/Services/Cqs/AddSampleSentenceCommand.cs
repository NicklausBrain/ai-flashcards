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
                "Sa oled keeleõppe süsteemi abiline, mis aitab õppida enim levinud eesti keele sõnu.\n" +

                "Teie sisend on JSON-objekt:" +
                "```\n{\n" +
                "\"EeWord\": \"<eestikeelne sõna>\", " +
                "\"EnWord\": \"<default english translation>\n" +
                "\"EnExplanation\": \"<explanation of the estonian word in english>\n" +
                "}\n```\n" +

                "Sinu sisend on üks eestikeelne sõna ja selle rakenduse kontekst: <sõna> (<kontekst>).\n" +
                "Sinu ülesanne on kirjutada selle kasutamise kohta lihtne lühike näitelause, kasutades seda sõna.\n" +
                "Lauses kasuta kõige levinuimaid ja lihtsamaid sõnu eesti keeles et toetada keeleõpet.\n" +
                "Eelistan SVO-lausete sõnajärge, kus esikohal on subjekt (S), seejärel tegusõna (V) ja objekt (O)\n" +
                "Lausel peaks olema praktiline tegelik elu mõte\n" +
                "Teie väljundiks on JSON-objekt koos eestikeelse näidislausega ja sellele vastav tõlge inglise keelde vastavalt lepingule:\n" +
                "```\n{\n" +
                "\"ee_sentence\": \"<näide eesti keeles>\", \"en_sentence\": \"<näide inglise keeles>\"" +
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