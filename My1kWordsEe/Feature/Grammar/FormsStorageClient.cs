using Azure.Storage.Blobs;

using CSharpFunctionalExtensions;

using static My1kWordsEe.Common.Conventions;

namespace My1kWordsEe.Feature.Grammar
{
    public class FormsStorageClient
    {
        public struct FormsContainerId
        {
            public required string BaseForm { get; init; }

            public required uint SenseIndex { get; init; }

            public override string ToString() => $"{BaseForm}-{SenseIndex}";

            public static implicit operator string(FormsContainerId id) => id.ToString();
        }

        private readonly AzureStorageClient azureStorageClient;

        public FormsStorageClient(AzureStorageClient azureStorageClient)
        {
            this.azureStorageClient = azureStorageClient;
        }

        public virtual Task<Result<Maybe<T>>> GetFormsData<T>(FormsContainerId containerId) where T : IGrammarForms =>
            this.GetEtFormsContainer().Bind(container =>
            this.azureStorageClient.DownloadJsonAsync<T>(
                container.GetBlobClient($"{containerId}.{JsonFormat}")));

        public virtual Task<Result<Uri>> SaveFormsData<T>(FormsContainerId containerId, T forms) where T : IGrammarForms =>
            this.GetEtFormsContainer().Bind(container =>
            this.azureStorageClient.UploadJsonAsync<T>(
                blob: container.GetBlobClient($"{containerId}.{JsonFormat}"),
                record: forms));

        private Task<Result<BlobContainerClient>> GetEtFormsContainer() =>
            this.azureStorageClient.GetOrCreateContainer("et-forms");
    }
}