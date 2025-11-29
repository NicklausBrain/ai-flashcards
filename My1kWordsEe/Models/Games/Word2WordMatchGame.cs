namespace My1kWordsEe.Models.Games
{
    public class Word2WordMatchGame
    {
        private readonly IEnumerable<Pair> pairs;
        private readonly IReadOnlyDictionary<string, Pair> etWords;
        private readonly IReadOnlyDictionary<string, Pair> enWords;
        private readonly ISet<string> etWords2Match = new HashSet<string>();
        private readonly ISet<string> enWords2Match = new HashSet<string>();
        private readonly Stack<Pair> matches = new Stack<Pair>();

        public Word2WordMatchGame(
            IEnumerable<Pair> pairs,
            IReadOnlyDictionary<string, Pair> etWords,
            IReadOnlyDictionary<string, Pair> enWords)
        {
            this.pairs = pairs;
            this.etWords = etWords;
            this.enWords = enWords;
            var etWords2MatchArray = etWords.Keys.ToArray();
            var enWords2MatchArray = enWords.Keys.ToArray();
            Random.Shared.Shuffle(etWords2MatchArray);
            Random.Shared.Shuffle(enWords2MatchArray);
            this.etWords2Match = etWords2MatchArray.ToHashSet();
            this.enWords2Match = enWords2MatchArray.ToHashSet();
        }

        /// <summary>
        /// Successfully matched pairs.
        /// </summary>
        public IEnumerable<Pair> Matches => this.matches;

        /// <summary>
        /// Gets pair by Estonian word.
        /// </summary>
        public IReadOnlyDictionary<string, Pair> EtWords => this.etWords;

        /// <summary>
        /// Gets pair by English word.
        /// </summary>
        public IReadOnlyDictionary<string, Pair> EnWords => this.enWords;

        /// <summary>
        /// All English words to match.
        /// </summary>
        public IEnumerable<string> EnWords2Match => this.enWords2Match;

        /// <summary>
        /// All Estonian words to match.
        /// </summary>
        public IEnumerable<string> EtWords2Match => this.etWords2Match;

        /// <summary>
        /// The game is initialized and ready to play.
        /// </summary>
        public bool IsReady => this.pairs.Any();

        /// <summary>
        /// The game is finished. All pairs are matched.
        /// </summary>
        public bool IsFinished => IsReady && this.pairs.All(p => p.IsMatched);

        /// <summary>
        /// Try to match the given words.
        /// </summary>
        /// <returns>True if words are correspond to each other. Otherwise False.</returns>
        public bool TryMatch(string eeWord, string enWord)
        {
            var pair = this.etWords[eeWord];
            if (pair.EnWord == enWord)
            {
                this.matches.Push(pair);
                this.etWords2Match.Remove(pair.EtWord);
                this.enWords2Match.Remove(pair.EnWord);
                return pair.IsMatched = true;
            }
            return false;
        }

        /// <summary>
        /// Null object pattern.
        /// </summary>
        public static readonly Word2WordMatchGame Empty = new Word2WordMatchGame(
            Array.Empty<Pair>(),
            new Dictionary<string, Pair>(),
            new Dictionary<string, Pair>());

        /// <summary>
        /// Pair of words to match.
        /// </summary>
        public class Pair
        {
            public required string EtWord { get; init; }

            public required string EnWord { get; init; }

            public bool IsMatched { get; set; }

            public Uri? EtAudioUrl { get; init; }
        }
    }
}