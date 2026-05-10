using CSharpFunctionalExtensions;

using My1kWordsEe.Services;
using My1kWordsEe.Services.Db;

namespace My1kWordsEe.Models.Games.Generation
{
    public class WordGrindGameFactory
    {
        public const string Prompt =
@"See on keeleõppe süsteem (tase B1).
Sisend: Eesti keele sõnade nimekiri.
Ülesanne:
1. Moodusta iga sõna kohta üks lihtne ja loomulik lause eesti keeles.
2. OLULINE: Sõna PEAB esinema lauses TÄPSELT SAMAL KUJUL nagu sisendis — sama kääne, pööre, arv jne.
   Ära muuda sõna vormi (nt ära kasuta 'kulutada' kui sisendis on 'kulutama').
   Kui vaja, kohanda lauset nii, et sõna sobiks täpselt sellel kujul.
3. Ära kasuta lauses teisi sõnu samast sisendnimekirjast ega nende käände-/pöördevorme.
4. Tõlgi iga lause inglise keelde.
5. Tagasta JSON, mis vastab skeemile.";

        private readonly OpenAiClient openAiClient;
        private readonly GameStorageClient gameStorageClient;

        public WordGrindGameFactory(
            OpenAiClient openAiClient,
            GameStorageClient gameStorageClient)
        {
            this.openAiClient = openAiClient;
            this.gameStorageClient = gameStorageClient;
        }

        public async Task<Result<WordGrindGame>> Generate(WordSet wordSet)
        {
            var gameId = $"{nameof(WordGrindGame)}-{wordSet.Id}";
            var cachedGame = await this.gameStorageClient.GetGameData<WordGrindGameData>(gameId);
            if (cachedGame.IsSuccess && cachedGame.Value.HasValue)
            {
                return new WordGrindGame(cachedGame.Value.Value);
            }

            var input = string.Join(", ", wordSet.Words);
            var gameDataResult = await this.openAiClient.CompleteJsonSchemaAsync<WordGrindGameData>(
                instructions: Prompt,
                input: input,
                schema: JsonSchemaRecord.For(typeof(WordGrindGameData)),
                temperature: 0.1f);

            if (gameDataResult.IsSuccess)
            {
                var gameData = gameDataResult.Value with { WordSetId = wordSet.Id };
                gameData = FilterInvalidItems(gameData);
                await this.gameStorageClient.SaveGameData(gameId, gameData);
                return new WordGrindGame(gameData);
            }

            return Result.Failure<WordGrindGame>(gameDataResult.Error);
        }

        /// <summary>
        /// Filters out items where the AI-generated sentence does not contain the exact word form.
        /// </summary>
        public static WordGrindGameData FilterInvalidItems(WordGrindGameData data)
        {
            var validItems = data.Items
                .Where(item => WordGrindTextMatcher.ContainsExactWord(item.Sentence.Et, item.Word))
                .ToList();

            return data with { Items = validItems };
        }
    }
}