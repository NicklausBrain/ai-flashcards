using CSharpFunctionalExtensions;

using My1kWordsEe.Models;
using My1kWordsEe.Services.Db;

namespace My1kWordsEe.Services.Cqs
{
    public class AddSampleSentenceCommand
    {
        public const int MaxSamples = 6;

        private readonly AzureStorageClient azureBlobService;
        private readonly OpenAiClient openAiService;
        private readonly AddAudioCommand addAudioCommand;
        private readonly StabilityAiClient stabilityAiService;

        public AddSampleSentenceCommand(
            AzureStorageClient azureBlobService,
            OpenAiClient openAiService,
            AddAudioCommand createAudioCommand,
            StabilityAiClient stabilityAiService)
        {
            this.azureBlobService = azureBlobService;
            this.openAiService = openAiService;
            this.addAudioCommand = createAudioCommand;
            this.stabilityAiService = stabilityAiService;
        }

        public async Task<Result<SampleWord>> Invoke(SampleWord word)
        {
            if (word.Samples.Length >= MaxSamples)
            {
                return Result.Failure<SampleWord>($"Too many samples. {MaxSamples} is a maximum");
            }

            var sentence = await this.openAiService.GetSampleSentence(
                eeWord: word.EeWord,
                existingSamples: word.Samples.Select(s => s.EeSentence).ToArray());
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
            this.openAiService.GetDallEPrompt(sentence.En).Bind(
            this.stabilityAiService.GenerateImage).Bind(
            this.azureBlobService.SaveImage);

        private Task<Result<Uri>> GenerateSpeech(Sentence sentence) =>
            this.addAudioCommand.Invoke(sentence.Ee);
    }
}