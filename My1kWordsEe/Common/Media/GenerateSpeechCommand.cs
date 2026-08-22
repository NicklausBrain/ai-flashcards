using CSharpFunctionalExtensions;

using My1kWordsEe.Feature.Games;

using static My1kWordsEe.Common.Conventions;

namespace My1kWordsEe.Common.Media
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

        public Task<Result<Uri>> Invoke(string sampleId, string sentence) =>
          this.tartuNlpService.GetSpeech(sentence).Bind(stream =>
          this.audioStorageClient.SaveAudio(stream, $"{sampleId}.{AudioFormat}"));
    }
}
