using CSharpFunctionalExtensions;

using My1kWordsEe.Services.Db;

namespace My1kWordsEe.Services.Cqs
{
    public class AddAudioCommand
    {
        private readonly TartuNlpService tartuNlpService;
        private readonly AzureBlobService azureBlobService;

        public AddAudioCommand(
            TartuNlpService tartuNlpService,
            AzureBlobService azureBlobService)
        {
            this.tartuNlpService = tartuNlpService;
            this.azureBlobService = azureBlobService;
        }

        public Task<Result<Uri>> Invoke(string text) =>
          this.tartuNlpService.GetSpeech(text).Bind(
          this.azureBlobService.SaveAudio);
    }
}
