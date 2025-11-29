using CSharpFunctionalExtensions;

using My1kWordsEe.Services;
using My1kWordsEe.Services.Db;

public class GenerateImageCommand
{
    private readonly ImageStorageClient imageStorageClient;
    private readonly OpenAiClient openAiClient;
    private readonly StabilityAiClient stabilityAiClient;

    public GenerateImageCommand(
        ImageStorageClient imageStorageClient,
        OpenAiClient openAiClient,
        StabilityAiClient stabilityAiClient
    )
    {
        this.imageStorageClient = imageStorageClient;
        this.openAiClient = openAiClient;
        this.stabilityAiClient = stabilityAiClient;
    }

    public Task<Result<Uri>> Invoke(Guid sampleId, string sentence) =>
    this.openAiClient.GetDallEPrompt(sentence).BindZip(
        this.stabilityAiClient.GenerateImage).Bind(p =>
        this.imageStorageClient.SaveImage(sampleId, p.First, p.Second));
}
