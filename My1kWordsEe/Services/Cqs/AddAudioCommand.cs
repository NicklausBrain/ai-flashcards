using CSharpFunctionalExtensions;

using My1kWordsEe.Services.Db;

namespace My1kWordsEe.Services.Cqs
{
    public class AddAudioCommand
    {
        private readonly TartuNlpClient tartuNlpService;
        private readonly AzureStorageClient azureBlobService;

        public AddAudioCommand(
            TartuNlpClient tartuNlpService,
            AzureStorageClient azureBlobService)
        {
            this.tartuNlpService = tartuNlpService;
            this.azureBlobService = azureBlobService;
        }

        public Task<Result<Uri>> Invoke(string text, string fileName) =>
          this.tartuNlpService.GetSpeech(text).Bind(stream =>
          this.azureBlobService.SaveAudio(stream, fileName));
    }
}