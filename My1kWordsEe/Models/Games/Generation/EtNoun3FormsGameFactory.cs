using CSharpFunctionalExtensions;

using My1kWordsEe.Models;
using My1kWordsEe.Services;

public class EtNoun3FormsGameFactory
{
    public const string Prompt = "todo";

    private readonly OpenAiClient openAiClient;

    public EtNoun3FormsGameFactory(OpenAiClient openAiClient)
    {
        this.openAiClient = openAiClient;
    }

    public async Task<Result<EtNoun3FormsGame>> Generate(string etNoun)
    {
        var response = await this.openAiClient.CompleteJsonSchemaAsync<EtNoun3FormsGame>(
            Prompt,
            etNoun,
            JsonSchemaRecord.For(typeof(EtNoun3FormsGame)),
            temperature: 0.1f);

        return response;
    }
}