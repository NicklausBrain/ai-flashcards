using System.ComponentModel;

using CSharpFunctionalExtensions;

using My1kWordsEe.Models.Grammar;
using My1kWordsEe.Services;
using My1kWordsEe.Services.Cqs.Et;

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

        public EtNoun3FormsGameFactory(
            OpenAiClient openAiClient,
            GetOrAddEtWordCommand getOrAddEtWordCommand,
            GetOrAddEtFormsCommand getOrAddEtFormsCommand)
        {
            this.openAiClient = openAiClient;
            this.getOrAddEtWordCommand = getOrAddEtWordCommand;
            this.getOrAddEtFormsCommand = getOrAddEtFormsCommand;
        }

        public async Task<Result<EtNoun3FormsGame>> Generate(string etNoun)
        {
            var gameData = await this.getOrAddEtWordCommand.Invoke(etNoun)
                .BindIf(word => word.DefaultSense.IsNoun, word => word)
                .Bind(word => this.getOrAddEtFormsCommand.Invoke<NounForms>(word, 0))
                .Bind(forms => this.openAiClient.CompleteJsonSchemaAsync<EtNoun3FormsGameData>(
                    Prompt,
                    forms.BaseForm,
                    JsonSchemaRecord.For(typeof(EtNoun3FormsGameData)),
                    temperature: 0.1f));

            // todo: save gameData in storage

            var game = gameData.Map((r) => new EtNoun3FormsGame(
                nimetavSõna: r.NimetavSõna,
                nimetavLause: r.NimetavLause,
                omastavSõna: r.OmastavSõna,
                omastavLause: r.OmastavLause,
                osastavSõna: r.OsastavSõna,
                osastavLause: r.OsastavLause
            ));

            return game;
        }

        public struct EtNoun3FormsGameData
        {
            [Description("Sõna ainsuse nimetavas käändes")]
            public string NimetavSõna { get; set; }

            [Description("Lihtne lause, kus sõna on ainsuse nimetavas käändes")]
            public string NimetavLause { get; set; }

            [Description("Sõna ainsuse omastavas käändes")]
            public string OmastavSõna { get; set; }

            [Description("Lihtne lause, kus sõna on ainsuse omastavas käändes")]
            public string OmastavLause { get; set; }

            [Description("Sõna ainsuse osastavas käändes")]
            public string OsastavSõna { get; set; }

            [Description("Lihtne lause, kus sõna on ainsuse osastavas käändes")]
            public string OsastavLause { get; set; }
        }
    }
}