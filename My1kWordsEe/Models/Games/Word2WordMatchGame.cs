﻿namespace My1kWordsEe.Models.Games
{
    public class Word2WordMatchGame
    {
        private readonly IEnumerable<Pair> pairs;
        private readonly IReadOnlyDictionary<string, Pair> eeWords;
        private readonly IReadOnlyDictionary<string, Pair> enWords;
        private readonly ISet<string> eeWords2Match = new HashSet<string>();
        private readonly ISet<string> enWords2Match = new HashSet<string>();
        private readonly Stack<Pair> matches = new Stack<Pair>();

        private Word2WordMatchGame(
            IEnumerable<Pair> pairs,
            IReadOnlyDictionary<string, Pair> eeWords,
            IReadOnlyDictionary<string, Pair> enWords)
        {
            this.pairs = pairs;
            this.eeWords = eeWords;
            this.enWords = enWords;
            var eeWords2MatchArray = eeWords.Keys.ToArray();
            var enWords2MatchArray = enWords.Keys.ToArray();
            Random.Shared.Shuffle(eeWords2MatchArray);
            Random.Shared.Shuffle(enWords2MatchArray);
            this.eeWords2Match = eeWords2MatchArray.ToHashSet();
            this.enWords2Match = enWords2MatchArray.ToHashSet();
        }

        /// <summary>
        /// Successfully matched pairs.
        /// </summary>
        public IEnumerable<Pair> Matches => this.matches;

        /// <summary>
        /// Gets pair by Estonian word.
        /// </summary>
        public IReadOnlyDictionary<string, Pair> EeWords => this.eeWords;

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
        public IEnumerable<string> EeWords2Match => this.eeWords2Match;

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
            var pair = this.eeWords[eeWord];
            if (pair.EnWord == enWord)
            {
                this.matches.Push(pair);
                this.eeWords2Match.Remove(pair.EeWord);
                this.enWords2Match.Remove(pair.EnWord);
                return pair.IsMatched = true;
            }
            return false;
        }

        /// <summary>
        /// Generate a new game.
        /// </summary>
        public static Task<Word2WordMatchGame> Generate() => Task.Run(() =>
        {
            var rn = new Random(Environment.TickCount);
            var eeWords = new Dictionary<string, Pair>();
            var enWords = new Dictionary<string, Pair>();
            var pairs = new List<Pair>();

            while (eeWords.Count < 5)
            {
                var nextWord = Ee1kWords.AllWords[rn.Next(0, Ee1kWords.AllWords.Length)];

                if (eeWords.ContainsKey(nextWord.Value) || enWords.ContainsKey(nextWord.EnWord))
                {
                    continue;
                }

                var pair = new Pair
                {
                    EeWord = nextWord.Value,
                    EnWord = nextWord.EnWord
                };

                eeWords.Add(pair.EeWord, pair);
                enWords.Add(pair.EnWord, pair);
                pairs.Add(pair);
            }

            return new Word2WordMatchGame(pairs, eeWords, enWords);
        });

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
            public required string EeWord { get; init; }

            public required string EnWord { get; init; }

            public bool IsMatched { get; set; }

            public Uri? EeAudioUrl { get; init; }
        }
    }
}
