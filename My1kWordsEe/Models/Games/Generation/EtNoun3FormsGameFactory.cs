using System.ComponentModel;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Unicode;

using CSharpFunctionalExtensions;

using My1kWordsEe.Models.Grammar;
using My1kWordsEe.Services;
using My1kWordsEe.Services.Cqs.Et;
using My1kWordsEe.Services.Db;

namespace My1kWordsEe.Models.Games
{
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
        private readonly GetOrAddEtWordCommand getOrAddEtWordCommand;
        private readonly GetOrAddEtFormsCommand getOrAddEtFormsCommand;
        private readonly GameStorageClient gameStorageClient;

        public EtNoun3FormsGameFactory(
            OpenAiClient openAiClient,
            GetOrAddEtWordCommand getOrAddEtWordCommand,
            GetOrAddEtFormsCommand getOrAddEtFormsCommand,
            GameStorageClient gameStorageClient)
        {
            this.openAiClient = openAiClient;
            this.getOrAddEtWordCommand = getOrAddEtWordCommand;
            this.getOrAddEtFormsCommand = getOrAddEtFormsCommand;
            this.gameStorageClient = gameStorageClient;
        }

        public async Task<Result<EtNoun3FormsGame>> Generate(string etNoun)
        {
            etNoun = etNoun.Trim().ToLower();
            var gameId = $"{nameof(EtNoun3FormsGame)}-{etNoun}";
            var cachedGame = await this.gameStorageClient.GetGameData<EtNoun3FormsGameData>(gameId);
            if (cachedGame.IsSuccess && cachedGame.Value.HasValue)
            {
                return MapToGame(cachedGame.Value.Value);
            }

            var gameDataResult = await this.getOrAddEtWordCommand.Invoke(etNoun)
                .BindIf(word => word.DefaultSense.IsNoun, word => word)
                .Bind(word => this.getOrAddEtFormsCommand.Invoke<NounForms>(word, 0))
                .Bind(forms => this.openAiClient.CompleteJsonSchemaAsync<EtNoun3FormsGameData>(
                    instructions: Prompt,
                    input: GetJsonInput(forms),
                    schema: JsonSchemaRecord.For(typeof(EtNoun3FormsGameData)),
                    temperature: 0.1f));

            if (gameDataResult.IsSuccess)
            {
                await this.gameStorageClient.SaveGameData(gameId, gameDataResult.Value);
            }

            return gameDataResult.Map(MapToGame);
        }

        private static EtNoun3FormsGame MapToGame(EtNoun3FormsGameData r) =>
            new EtNoun3FormsGame(
                nimetavSõna: r.NimetavSõna,
                nimetavLause: r.NimetavLause,
                omastavSõna: r.OmastavSõna,
                omastavLause: r.OmastavLause,
                osastavSõna: r.OsastavSõna,
                osastavLause: r.OsastavLause
            );

        private static string GetJsonInput(NounForms nounForms)
        {
            TranslatedString GetCaseForm(EtGrammaticalCase grammaticalCase) =>
                nounForms.Singular.FirstOrDefault(f => f.GrammaticalCase == grammaticalCase).CaseForm;

            var inputJson = JsonSerializer.Serialize(new
            {
                Nimetav = GetCaseForm(EtGrammaticalCase.Nimetav),
                Omastav = GetCaseForm(EtGrammaticalCase.Omastav),
                Osastav = GetCaseForm(EtGrammaticalCase.Osastav)
            }, new JsonSerializerOptions
            {
                Encoder = JavaScriptEncoder.Create(UnicodeRanges.All),
            });

            return inputJson;
        }

        public struct EtNoun3FormsGameData
        {
            [Description("Sõna ainsuse nimetavas käändes")]
            public required TranslatedString NimetavSõna { get; set; }

            [Description("Lihtne lause, kus sõna on ainsuse nimetavas käändes")]
            public required TranslatedString NimetavLause { get; set; }

            [Description("Sõna ainsuse omastavas käändes")]
            public required TranslatedString OmastavSõna { get; set; }

            [Description("Lihtne lause, kus sõna on ainsuse omastavas käändes")]
            public required TranslatedString OmastavLause { get; set; }

            [Description("Sõna ainsuse osastavas käändes")]
            public required TranslatedString OsastavSõna { get; set; }

            [Description("Lihtne lause, kus sõna on ainsuse osastavas käändes")]
            public required TranslatedString OsastavLause { get; set; }
        }
    }
}