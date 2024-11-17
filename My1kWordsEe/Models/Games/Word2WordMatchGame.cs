﻿using My1kWordsEe.Services.Cqs;

namespace My1kWordsEe.Models.Games
{
    // should we have a 'base' game
    // so that we can combine different games?
    public class Word2WordMatchGame
    {
        private readonly Pair[] pairs;
        private readonly Dictionary<string, Pair> eeWords;
        private readonly Dictionary<string, Pair> enWords;
        private readonly HashSet<string> eeWords2Match = new HashSet<string>();
        private readonly HashSet<string> enWords2Match = new HashSet<string>();
        private readonly Stack<Pair> matches = new Stack<Pair>();

        public Word2WordMatchGame(Pair[] pairs)
        {
            this.pairs = pairs;
            eeWords = this.pairs.ToDictionary(p => p.EeWord);
            enWords = this.pairs.ToDictionary(p => p.EnWord);
            var eeWords2MatchArray = eeWords.Keys.ToArray();
            var enWords2MatchArray = enWords.Keys.ToArray();
            Random.Shared.Shuffle(eeWords2MatchArray);
            Random.Shared.Shuffle(enWords2MatchArray);
            this.eeWords2Match = eeWords2MatchArray.ToHashSet();
            this.enWords2Match = enWords2MatchArray.ToHashSet();
        }

        public IEnumerable<Pair> Pairs => this.pairs;

        public IEnumerable<Pair> Matches => this.matches;

        public IReadOnlyDictionary<string, Pair> EeWords => this.eeWords;

        public IReadOnlyDictionary<string, Pair> EnWords => this.enWords;

        public IEnumerable<string> EnWords2Match => this.enWords2Match;

        public IEnumerable<string> EeWords2Match => this.eeWords2Match;

        public bool IsFinished => this.pairs.Any() && this.pairs.All(p => p.IsMatched);

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

            // might be not needed if use ''matches''
            public bool IsMatched { get; set; }

            public Uri? EeAudioUrl { get; init; }
        }
    }
}
