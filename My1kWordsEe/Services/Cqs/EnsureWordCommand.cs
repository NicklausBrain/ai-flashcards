using My1kWordsEe.Models;
using My1kWordsEe.Services.Db;

namespace My1kWordsEe.Services.Cqs
{
    public class EnsureWordCommand
    {
        private readonly AzureBlobService azureBlobService;
        private readonly OpenAiService openAiService;

        public EnsureWordCommand(
            AzureBlobService azureBlobService,
            OpenAiService openAiService)
        {
            this.azureBlobService = azureBlobService;
            this.openAiService = openAiService;
        }

        public SampleWord Invoke(string eeWord)
        {
            var existingRecord = azureBlobService.GetWordData(eeWord);
        }
    }
}