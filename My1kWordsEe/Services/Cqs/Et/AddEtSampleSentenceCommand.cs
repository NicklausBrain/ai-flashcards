using System.Text.Json;

using CSharpFunctionalExtensions;

using My1kWordsEe.Models.Semantics;

using My1kWordsEe.Services.Db;

using static My1kWordsEe.Models.Extensions;

namespace My1kWordsEe.Services.Cqs.Et
{
    public class AddEtSampleSentenceCommand
    {
        public const int MaxSamples = 6;

        // todo: test it
        public static readonly string Prompt =
            "Sa oled keeleõppe süsteemi abiline, mis aitab õppida enim levinud eesti keele sõnu.\n" +
            "Sisendiks on JSON-objekt, mis kirjeldab eesti keele sõna tähendust ja grammatilist vormi:\n" +
            $"```\n{GetJsonSchema(typeof(WordSense))}\n```\n" +
            "Teie ülesanne on genereerida JSON-objekt, mis sisaldab näidislauseid eesti ja inglise keeles, kasutades antud sõna sobivas grammatilises vormis:\n" +
            $"```\n{GetJsonSchema(typeof(SampleSentence))}\n```\n";

        private readonly AzureStorageClient azureBlobClient;
        private readonly OpenAiClient openAiClient;
        private readonly AddAudioCommand addAudioCommand;
        private readonly StabilityAiClient stabilityAiClient;

        public AddEtSampleSentenceCommand(
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

        public async Task<Result<SampleSentenceWithMedia[]>> Invoke(WordSense word)
        {
            var existingSamples = new SampleSentenceWithMedia[] { };

            if (existingSamples.Length >= MaxSamples)
            {
                return Result.Failure<SampleSentenceWithMedia[]>($"Too many samples. {MaxSamples} is a maximum");
            }

            var sentence = await this.GetSampleSentence(word);

            if (sentence.IsFailure)
            {
                return Result.Failure<SampleSentenceWithMedia[]>($"Sentence generation failed: {sentence.Error}");
            }

            var imageGeneration = this.GenerateImage(sentence.Value);
            var speechGeneration = this.GenerateSpeech(sentence.Value);
            await Task.WhenAll(imageGeneration, speechGeneration);

            if (imageGeneration.Result.IsFailure)
            {
                return Result.Failure<SampleSentenceWithMedia[]>($"Image generation failed: {imageGeneration.Result.Error}");
            }

            if (speechGeneration.Result.IsFailure)
            {
                return Result.Failure<SampleSentenceWithMedia[]>($"Speech generation failed: {speechGeneration.Result.Error}");
            }

            // todo: fix it
            return new SampleSentenceWithMedia[] { };
            // var updatedWordData = word with
            // {
            //     Samples = word.Samples.Append(new SampleSentence
            //     {
            //         EeWord = word.EeWord,
            //         EeSentence = sentence.Value.Ee,
            //         EnSentence = sentence.Value.En,
            //         EeAudioUrl = speechGeneration.Result.Value,
            //         ImageUrl = imageGeneration.Result.Value,
            //     }).ToArray()
            // };

            // return (await this.azureBlobClient
            //     .SaveWordData(updatedWordData))
            //     .Bind(r => Result.Success(updatedWordData));
        }

        private Task<Result<Uri>> GenerateImage(SampleSentence sentence) =>
            this.openAiClient.GetDallEPrompt(sentence.Sentence.En).Bind(
            this.stabilityAiClient.GenerateImage).Bind(
            this.azureBlobClient.SaveImage);

        private Task<Result<Uri>> GenerateSpeech(SampleSentence sentence) =>
            this.addAudioCommand.Invoke(sentence.Sentence.Et);

        private async Task<Result<SampleSentence>> GetSampleSentence(WordSense word)
        {
            var input = JsonSerializer.Serialize(word);

            var result = await this.openAiClient.CompleteJsonAsync<SampleSentence>(
                Prompt,
                input,
                temperature: 0.7f);

            return result;
        }
    }
}