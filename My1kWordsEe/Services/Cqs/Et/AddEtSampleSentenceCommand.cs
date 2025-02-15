using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Unicode;

using CSharpFunctionalExtensions;

using My1kWordsEe.Models;
using My1kWordsEe.Models.Semantics;
using My1kWordsEe.Services.Db;

using static My1kWordsEe.Models.Conventions;
using static My1kWordsEe.Services.Db.SamplesStorageClient;

namespace My1kWordsEe.Services.Cqs.Et
{
    public class AddEtSampleSentenceCommand
    {
        public const int MaxSamples = 6;

        public static readonly string Prompt =
            "Sa oled keeleõppe süsteemi abiline, mis aitab õppida enim levinud eesti keele sõnu.\n" +
            "Sisendiks on järgmine JSON-objekt, mis kirjeldab eesti keele sõna põhivormi, tähendust ja kõneosa:\n" +
            $"{JsonSchemaRecord.For(typeof(Input))}\n" +
            "Sinu ülesanne on:\n" +
            "1. Vaadata sisendit ja määrata, kuidas antud sõna sobivas grammatilises vormis lauses kasutada.\n" +
            "2. Genereerida JSON-objekt, mis sisaldab ühte näidislause paari eesti ja inglise keeles.\n";

        private readonly SamplesStorageClient samplesStorageClient;
        private readonly ImageStorageClient imageStorageClient;
        private readonly OpenAiClient openAiClient;
        private readonly AddAudioCommand addAudioCommand;
        private readonly StabilityAiClient stabilityAiClient;

        public AddEtSampleSentenceCommand(
            SamplesStorageClient samplesStorageClient,
            ImageStorageClient imageStorageClient,
            OpenAiClient openAiService,
            AddAudioCommand createAudioCommand,
            StabilityAiClient stabilityAiService)
        {
            this.samplesStorageClient = samplesStorageClient;
            this.imageStorageClient = imageStorageClient;
            this.openAiClient = openAiService;
            this.addAudioCommand = createAudioCommand;
            this.stabilityAiClient = stabilityAiService;
        }

        public async Task<Result<SampleSentenceWithMedia[]>> Invoke(EtWord word, uint senseIndex)
        {
            var containerId = new SamplesContainerId { SenseIndex = senseIndex, Word = word.Value };
            var existingSamples = await this.samplesStorageClient.GetEtSampleData(containerId);

            if (existingSamples.IsFailure)
            {
                return Result.Failure<SampleSentenceWithMedia[]>(existingSamples.Error);
            }

            if (existingSamples.Value.Length >= MaxSamples)
            {
                return Result.Failure<SampleSentenceWithMedia[]>($"Too many samples. {MaxSamples} is a maximum");
            }

            var sentence = await this.GetSampleSentence(word, senseIndex);

            if (sentence.IsFailure)
            {
                return Result.Failure<SampleSentenceWithMedia[]>($"Sentence generation failed: {sentence.Error}");
            }

            var sampleId = Guid.NewGuid();
            var imageGeneration = this.GenerateImage(sampleId, sentence.Value);
            var speechGeneration = this.GenerateSpeech(sampleId, sentence.Value);
            await Task.WhenAll(imageGeneration, speechGeneration);

            if (imageGeneration.Result.IsFailure)
            {
                return Result.Failure<SampleSentenceWithMedia[]>($"Image generation failed: {imageGeneration.Result.Error}");
            }

            if (speechGeneration.Result.IsFailure)
            {
                return Result.Failure<SampleSentenceWithMedia[]>($"Speech generation failed: {speechGeneration.Result.Error}");
            }

            var updatedSamples = existingSamples.Value.Append(new SampleSentenceWithMedia
            {
                Id = sampleId,
                Sentence = new TranslatedString
                {
                    En = sentence.Value.Sentence.En,
                    Et = sentence.Value.Sentence.Et,
                },
            }).ToArray();

            return (await this.samplesStorageClient
                .SaveEtSamplesData(containerId, updatedSamples))
                .Bind(r => Result.Success(updatedSamples));
        }

        // todo: move to separate command
        private Task<Result<Uri>> GenerateImage(Guid sampleId, SampleEtSentence sentence) =>
            this.openAiClient.GetDallEPrompt(sentence.Sentence.En).BindZip(
            this.stabilityAiClient.GenerateImage).Bind(p =>
            this.imageStorageClient.SaveImage(sampleId, p.First, p.Second));

        // todo: rename addAudioCommand
        private Task<Result<Uri>> GenerateSpeech(Guid sampleId, SampleEtSentence sentence) =>
            this.addAudioCommand.Invoke(
                text: sentence.Sentence.Et,
                fileName: $"{sampleId}.{AudioFormat}");

        private async Task<Result<SampleEtSentence>> GetSampleSentence(EtWord word, uint senseIndex)
        {
            var input = JsonSerializer.Serialize(new Input
            {
                Sõna = word.Value,
                Tähendus = word.Senses[senseIndex].Definition.Et,
                Kõneosa = word.Senses[senseIndex].PartOfSpeech.Et
            }, options: new JsonSerializerOptions
            {
                WriteIndented = false,
                Encoder = JavaScriptEncoder.Create(UnicodeRanges.All),
            });

            var result = await this.openAiClient.CompleteJsonSchemaAsync<SampleEtSentence>(
                instructions: Prompt,
                input: input,
                schema: JsonSchemaRecord.For(typeof(SampleEtSentence)),
                temperature: 0.3f);

            return result;
        }

        private struct Input
        {
            public required string Sõna { get; init; }
            public required string Tähendus { get; init; }
            public required string Kõneosa { get; init; }
        }
    }
}