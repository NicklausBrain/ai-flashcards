namespace My1kWordsEe.Models.Games
{
    // should we have a 'base' game
    // so that we can combine different games?

    public class Word2WordMatchGame
    {
        private readonly Pair[] pairs;

        private readonly Dictionary<string, Pair> eeWords;
        private readonly Dictionary<string, Pair> enWords;

        public Word2WordMatchGame(Pair[] pairs)
        {
            this.pairs = pairs;

            eeWords = this.pairs.ToDictionary(p => p.EeWord);
            enWords = this.pairs.ToDictionary(p => p.EnWord);
        }

        public bool TryMatch(string eeWord, string enWord)
        {
            var pair = this.pairs.FirstOrDefault(p => p.EeWord == eeWord && p.EnWord == enWord);
            if (pair != null)
            {
                pair.IsMatched = true;
                return true;
            }
            return false;
        }

        public IEnumerable<Pair> Pairs => this.pairs;

        public IReadOnlyDictionary<string, Pair> EeWords => this.eeWords;

        public IReadOnlyDictionary<string, Pair> EnWords => this.enWords;

        public bool IsFinished => this.pairs.All(p => p.IsMatched);

        public static Task<Word2WordMatchGame> Generate()
        {
            // todo: possible bug: what if 1 ee word correspond to 2 en words? or vice versa?
            var pairs = Ee1kWords.AllWords
                .TakeLast(5)
                .Select(w => new Pair
                {
                    EeWord = w.Value,
                    EnWord = w.EnWord
                }).ToArray();

            return Task.FromResult(new Word2WordMatchGame(pairs));
        }

        public class Pair
        {
            public required string EeWord { get; init; }
            public required string EnWord { get; init; }

            public bool IsMatched { get; set; }
        }
    }
}
