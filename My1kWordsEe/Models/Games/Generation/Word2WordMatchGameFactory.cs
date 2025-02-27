using My1kWordsEe.Models.Games.Generation;

using static My1kWordsEe.Models.Games.Word2WordMatchGame;

namespace My1kWordsEe.Models.Games
{
    public class Word2WordMatchGameFactory
    {
        private readonly NextWordSelector nextWordSelector;

        public Word2WordMatchGameFactory(NextWordSelector nextWordSelector)
        {
            this.nextWordSelector = nextWordSelector;
        }

        /// <summary>
        /// Generate a new game.
        /// </summary>
        public Task<Word2WordMatchGame> Generate() => Task.Run(async () =>
        {
            var rn = new Random(Environment.TickCount);
            var eeWords = new Dictionary<string, Pair>();
            var enWords = new Dictionary<string, Pair>();
            var pairs = new List<Pair>();

            while (eeWords.Count < 5)
            {
                var nextWord = await nextWordSelector.GetNextWord();

                if (eeWords.ContainsKey(nextWord.DefaultSense.Word.Et) || enWords.ContainsKey(nextWord.DefaultSense.Word.En))
                {
                    continue;
                }

                var pair = new Pair
                {
                    EeWord = nextWord.DefaultSense.Word.Et,
                    EnWord = nextWord.DefaultSense.Word.En
                };

                eeWords.Add(pair.EeWord, pair);
                enWords.Add(pair.EnWord, pair);
                pairs.Add(pair);
            }

            return new Word2WordMatchGame(pairs, eeWords, enWords);
        });
    }
}