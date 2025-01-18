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

        public async Task<Result<SampleWord>> Invoke(SampleSentence sampleToRemove)
        {
            var imageRemoval = this.azureBlobService.DeleteImage(sampleToRemove.ImageUrl.Segments.Last());
            var audioRemoval = this.azureBlobService.DeleteAudio(sampleToRemove.EeAudioUrl.Segments.Last());

            await Task.WhenAll(imageRemoval, audioRemoval);

            if (imageRemoval.Result.IsFailure)
            {
                return Result.Failure<SampleWord>($"Image removal failed: {imageRemoval.Result.Error}");
            }

            if (audioRemoval.Result.IsFailure)
            {
                return Result.Failure<SampleWord>($"Speech removal failed: {audioRemoval.Result.Error}");
            }

            var wordData = await this.azureBlobService.GetWordData(sampleToRemove.EeWord);

            if (wordData.IsFailure)
            {
                return Result.Failure<SampleWord>("Sample word not found");
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
