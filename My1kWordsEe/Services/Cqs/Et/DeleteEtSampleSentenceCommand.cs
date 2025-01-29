using CSharpFunctionalExtensions;

using My1kWordsEe.Models.Semantics;

using My1kWordsEe.Services.Db;

namespace My1kWordsEe.Services.Cqs.Et
{
    public class DeleteEtSampleSentenceCommand
    {
        private readonly AzureStorageClient azureBlobService;

        public DeleteEtSampleSentenceCommand(
            AzureStorageClient azureBlobService)
        {
            this.azureBlobService = azureBlobService;
        }

        public async Task<Result<SampleSentence[]>> Invoke(SampleSentence sampleToRemove)
        {
            var imageRemoval = this.azureBlobService.DeleteImage(sampleToRemove.ImageUrl.Segments.Last());
            var audioRemoval = this.azureBlobService.DeleteAudio(sampleToRemove.AudioUrl.Segments.Last());

            await Task.WhenAll(imageRemoval, audioRemoval);

            if (imageRemoval.Result.IsFailure)
            {
                return Result.Failure<SampleSentence[]>($"Image removal failed: {imageRemoval.Result.Error}");
            }

            if (audioRemoval.Result.IsFailure)
            {
                return Result.Failure<SampleSentence[]>($"Speech removal failed: {audioRemoval.Result.Error}");
            }

            // todo: fix it
            var wordData = await this.azureBlobService.GetEtSampleData("eeee");

            if (wordData.IsFailure)
            {
                return Result.Failure<SampleSentence[]>("Sample word not found");
            }

            // todo: fix it
            return new SampleSentence[] { };
            // var updatedWordData = wordData.Value.Value with
            // {
            //     Samples = wordData.Value.Value.Samples.Where(s => s != sampleToRemove).ToArray()
            // };

            // await this.azureBlobService.SaveWordData(updatedWordData);

            // return Result.Success(updatedWordData);
        }
    }
}