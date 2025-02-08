using My1kWordsEe.Services.Db;

namespace My1kWordsEe.Services
{
    public class UrlService
    {
        private readonly AzureStorageClient azureStorage;

        public UrlService(AzureStorageClient azureStorage)
        {
            this.azureStorage = azureStorage;
        }

        public Uri Resolve(Uri relativeUrl) => new Uri(this.azureStorage.AzureBlobEndpoint, relativeUrl);
    }
}
