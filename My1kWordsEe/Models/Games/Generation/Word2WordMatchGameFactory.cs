using My1kWordsEe.Models.Games.Generation;

using static My1kWordsEe.Models.Games.Word2WordMatchGame;

namespace My1kWordsEe.Models.Games
{
    public class Word2WordMatchGameFactory
    {
        private readonly NextEtWordSelector nextWordSelector;

        public Word2WordMatchGameFactory(NextEtWordSelector nextWordSelector)
        {
            this.nextWordSelector = nextWordSelector;
        }

        /// <summary>
        /// Generate a new game.
        /// </summary>
        public Task<Word2WordMatchGame> Generate() => Task.Run(async () =>
        {
            var rn = new Random(Environment.TickCount);
            var etWords = new Dictionary<string, Pair>();
            var enWords = new Dictionary<string, Pair>();
            var pairs = new List<Pair>();

            while (etWords.Count < 5)
            {
                var nextWord = await nextWordSelector.GetNextWord();

                if (etWords.ContainsKey(nextWord.DefaultSense.Word.Et) || enWords.ContainsKey(nextWord.DefaultSense.Word.En))
                {
                    continue;
                }

                var pair = new Pair
                {
                    EtWord = nextWord.DefaultSense.Word.Et,
                    EnWord = nextWord.DefaultSense.Word.En
                };

                etWords.Add(pair.EtWord, pair);
                enWords.Add(pair.EnWord, pair);
                pairs.Add(pair);
            }

            return new Word2WordMatchGame(pairs, etWords, enWords);
        });
    }
}