using CSharpFunctionalExtensions;

using My1kWordsEe.Models;
using My1kWordsEe.Services.Db;

namespace My1kWordsEe.Services.Cqs
{
    public class CreateSampleCommand
    {
        private readonly AzureBlobService azureBlobService;
        private readonly OpenAiService openAiService;
        private readonly TartuNlpService tartuNlpService;
        private readonly StabilityAiService stabilityAiService;

        public CreateSampleCommand(
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
            var sentence = await this.openAiService.GetSampleSentence(word.EnWord);
            var imageGeneration = this.GenerateImage(sentence.Value);
            var speechGeneration = this.GenerateSpeech(sentence.Value);
            await Task.WhenAll(imageGeneration, speechGeneration);

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

        private async Task<Result<Uri>> GenerateImage(Sentence sentence)
        {
            var prompt = await this.openAiService.GetDallEPrompt(sentence.En);
            var image = await this.stabilityAiService.GenerateImage(prompt.Value);
            var url = await this.azureBlobService.SaveImage(image.Value);
            return url;
        }

        private async Task<Result<Uri>> GenerateSpeech(Sentence sentence)
        {
            var speech = await this.tartuNlpService.GetSpeech(sentence.Ee);
            var url = await this.azureBlobService.SaveAudio(speech);
            return url;
        }
    }
}