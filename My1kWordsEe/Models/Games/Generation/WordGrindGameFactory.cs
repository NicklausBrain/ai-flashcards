using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using CSharpFunctionalExtensions;

using My1kWordsEe.Services;
using My1kWordsEe.Services.Db;

namespace My1kWordsEe.Models.Games.Generation
{
    public class WordGrindGameFactory
    {
        public const string Prompt =
@"See on keeleõppe süsteem (tase A1-A2).
Sisend: Eesti keele sõnade nimekiri.
Ülesanne:
1. Moodusta iga sõna kohta üks lihtne ja loomulik lause eesti keeles.
2. SÕNA PEAB OLEMA LAUSES TÄPSELT SELLISEL KUJUL, NAGU SEE ON SISENDIS (sama kääne, pööre jne).
3. Tõlgi iga lause inglise keelde.
4. Tagasta JSON, mis vastab skeemile.";

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
                // Ensure WordSetId is correctly set in the result
                var gameData = gameDataResult.Value with { WordSetId = wordSet.Id };
                await this.gameStorageClient.SaveGameData(gameId, gameData);
                return new WordGrindGame(gameData);
            }

            return Result.Failure<WordGrindGame>(gameDataResult.Error);
        }
    }
}