using Azure.Storage.Blobs;

using CSharpFunctionalExtensions;

using My1kWordsEe.Models.Grammar.Forms;

using static My1kWordsEe.Models.Conventions;

namespace My1kWordsEe.Services.Db
{
    public partial class FormsStorageClient
    {
        public struct FormsContainerId
        {
            public required string Word { get; init; }

            public required uint SenseIndex { get; init; }

            public override string ToString() => $"{Word}-{SenseIndex}";

            public static implicit operator string(FormsContainerId id) => id.ToString();
        }

        private readonly AzureStorageClient azureStorageClient;

        public FormsStorageClient(AzureStorageClient azureStorageClient)
        {
            this.azureStorageClient = azureStorageClient;
        }

        public Task<Result<Maybe<T>>> GetFormsData<T>(FormsContainerId containerId) where T : IGrammarForms =>
            this.GetEtWordsContainer().Bind(container =>
            this.azureStorageClient.DownloadJsonAsync<T>(
                container.GetBlobClient($"{containerId}.{JsonFormat}")));

        public Task<Result<Uri>> SaveFormsData(FormsContainerId containerId, IGrammarForms forms) =>
            this.GetEtWordsContainer().Bind(container =>
            this.azureStorageClient.UploadJsonAsync(
                blob: container.GetBlobClient($"{containerId}.{JsonFormat}"),
                record: forms));

        private Task<Result<BlobContainerClient>> GetEtWordsContainer() =>
            this.azureStorageClient.GetOrCreateContainer("et-forms");
    }
}