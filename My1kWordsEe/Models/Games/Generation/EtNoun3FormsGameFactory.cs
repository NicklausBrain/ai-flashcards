using CSharpFunctionalExtensions;

using My1kWordsEe.Models;
using My1kWordsEe.Services;

public class EtNoun3FormsGameFactory
{
    public const string Prompt =
@"See on keeleõppe süsteem.
Sisend: Eesti keele nimisõna (nimetav kääne).
Ülesanne:
1. Moodusta antud sõnast kolm käänet: ainsuse Nimetav, ainsuse Omastav ja ainsuse Osastav.
2. Moodusta iga käände kohta üks lihtne ja loomulik lause (tase A1-A2), kus seda sõna on kasutatud vastavas käändes.
3. Tagasta JSON, mis vastab skeemile.";

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