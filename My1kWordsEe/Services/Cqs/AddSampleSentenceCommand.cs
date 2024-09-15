using CSharpFunctionalExtensions;

using My1kWordsEe.Models;
using My1kWordsEe.Services.Db;

namespace My1kWordsEe.Services.Cqs
{
    public class AddSampleSentenceCommand
    {
        private readonly AzureBlobService azureBlobService;
        private readonly OpenAiService openAiService;
        private readonly TartuNlpService tartuNlpService;
        private readonly StabilityAiService stabilityAiService;

        public AddSampleSentenceCommand(
            AzureBlobService azureBlobService,
            OpenAiService openAiService,
            TartuNlpService tartuNlpService,
            StabilityAiService stabilityAiService)
        {
            this.azureBlobService = azureBlobService;
            this.openAiService = openAiService;
            this.tartuNlpService = tartuNlpService;
            this.stabilityAiService = stabilityAiService;
        }

        public async Task<Result<SampleWord>> Invoke(SampleWord word)
        {
            var sentence = await this.openAiService.GetSampleSentence(word.EeWord);
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

            await this.azureBlobService.SaveWordData(updatedWordData);

            return Result.Success(updatedWordData);
        }

        private Task<Result<Uri>> GenerateImage(Sentence sentence) =>
            this.openAiService.GetDallEPrompt(sentence.En)
                .Bind(this.stabilityAiService.GenerateImage)
                .Bind(image => Result.Of(this.azureBlobService.SaveImage(image)));

        private Task<Result<Uri>> GenerateSpeech(Sentence sentence) =>
            this.tartuNlpService.GetSpeech(sentence.Ee)
                .Bind(speech => Result.Of(this.azureBlobService.SaveAudio(speech)));
    }
}