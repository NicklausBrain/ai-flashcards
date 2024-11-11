using CSharpFunctionalExtensions;

using My1kWordsEe.Models;
using My1kWordsEe.Services.Db;

namespace My1kWordsEe.Services.Cqs
{
    public class DeleteSampleSentenceCommand
    {
        private readonly AzureStorageClient azureBlobService;

        public DeleteSampleSentenceCommand(
            AzureStorageClient azureBlobService)
        {
            this.azureBlobService = azureBlobService;
        }

        public async Task<Result> Invoke(SampleSentence sampleToRemove)
        {
            var imageRemoval = this.azureBlobService.DeleteIfExistsAsync(sampleToRemove.ImageUrl);
            var audioRemoval = this.azureBlobService.DeleteIfExistsAsync(sampleToRemove.EeAudioUrl);

            await Task.WhenAll(imageRemoval, audioRemoval);

            if (imageRemoval.Result.IsFailure)
            {
                return Result.Failure($"Image removal failed: {imageRemoval.Result.Error}");
            }

            if (audioRemoval.Result.IsFailure)
            {
                return Result.Failure($"Speech removal failed: {audioRemoval.Result.Error}");
            }

            var wordData = await this.azureBlobService.GetWordData(sampleToRemove.EeWord);

            if (wordData.IsFailure)
            {
                return Result.Failure("Sample word not found");
            }

            var updatedWordData = wordData.Value.Value with
            {
                Samples = wordData.Value.Value.Samples.Where(s => s != sampleToRemove).ToArray()
            };

            await this.azureBlobService.SaveWordData(updatedWordData);

            return Result.Success(updatedWordData);
        }
    }
}