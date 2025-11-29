using CSharpFunctionalExtensions;

using My1kWordsEe.Services.Db;

namespace My1kWordsEe.Services.Cqs
{
    public class GenerateSpeechCommand
    {
        private readonly TartuNlpClient tartuNlpService;
        private readonly AudioStorageClient audioStorageClient;

        public GenerateSpeechCommand(
            TartuNlpClient tartuNlpService,
            AudioStorageClient audioStorageClient)
        {
            this.tartuNlpService = tartuNlpService;
            this.audioStorageClient = audioStorageClient;
        }

        public Task<Result<Uri>> Invoke(string text, string fileName) =>
          this.tartuNlpService.GetSpeech(text).Bind(stream =>
          this.audioStorageClient.SaveAudio(stream, fileName));
    }
}