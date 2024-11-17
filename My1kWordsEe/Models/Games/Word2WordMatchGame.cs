using My1kWordsEe.Services.Cqs;

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
            var pair = this.eeWords[eeWord];
            if (pair.EnWord == enWord)
            {
                pair.IsMatched = true;
                return true;
            }
            return false;
        }

        public IEnumerable<Pair> Pairs => this.pairs;

        public IReadOnlyDictionary<string, Pair> EeWords => this.eeWords;

        public IReadOnlyDictionary<string, Pair> EnWords => this.enWords;

        public bool IsFinished => this.pairs.Any() && this.pairs.All(p => p.IsMatched);

        public static Task<Word2WordMatchGame> Generate(GetOrAddSampleWordCommand ensureWordCommand) => Task.Run(async () =>
        {
            // todo: possible bug: what if 1 ee word correspond to 2 en words? or vice versa?
            var rn = new Random(Environment.TickCount);

            var eeWords = new EeWord[]
            {
                Ee1kWords.AllWords[rn.Next(0,Ee1kWords.AllWords.Length)],
                Ee1kWords.AllWords[rn.Next(0,Ee1kWords.AllWords.Length)],
                Ee1kWords.AllWords[rn.Next(0,Ee1kWords.AllWords.Length)],
                Ee1kWords.AllWords[rn.Next(0,Ee1kWords.AllWords.Length)],
                Ee1kWords.AllWords[rn.Next(0,Ee1kWords.AllWords.Length)],
            };

            var pairs = await Task.WhenAll(eeWords
                .AsParallel()
                .Select(async w => new Pair
                {
                    EeAudioUrl = (await ensureWordCommand.Invoke(w.Value)).Value.EeAudioUrl,
                    EeWord = w.Value,
                    EnWord = w.EnWord
                }));

            return new Word2WordMatchGame(pairs);
        });

        public class Pair
        {
            public required string EeWord { get; init; }

            public required string EnWord { get; init; }

            public bool IsMatched { get; set; }

            public Uri? EeAudioUrl { get; init; }
        }
    }
}
