using CSharpFunctionalExtensions;

using My1kWordsEe.Services.Cqs;

namespace My1kWordsEe.Models
{
    public class TranslateToEnGame
    {
        private readonly SampleWord[] sampleWords;
        private readonly GameSlide[] gameSlides;

        public TranslateToEnGame(SampleWord[] sampleWords)
        {
            this.sampleWords = sampleWords;
            this.gameSlides = sampleWords.Select(sw => new GameSlide(sw)).ToArray();
        }

        public GameSlide[] Slides => this.gameSlides;

        public ushort CurrentSlideIndex { get; private set; } = 0;

        public static async Task<TranslateToEnGame> Generate(
            GetOrAddSampleWordCommand getOrAddSampleWordCommand,
            AddSampleSentenceCommand addSampleSentenceCommand)
        {
            var rn = new Random(2); // Environment.TickCount

            var eeWords = new List<EeWord>
            {
                Ee1kWords.AllWords[rn.Next(0,Ee1kWords.AllWords.Length)],
                Ee1kWords.AllWords[rn.Next(0,Ee1kWords.AllWords.Length)],
                Ee1kWords.AllWords[rn.Next(0,Ee1kWords.AllWords.Length)],
            };

            var sampleWords = await Task.WhenAll(eeWords.AsParallel().Select(async eeWord =>
            {
                var sampleWord = await getOrAddSampleWordCommand.Invoke(eeWord.Value);
                if (sampleWord.Value.Samples.Any())
                {
                    return sampleWord.Value;
                }
                else
                {
                    return (await addSampleSentenceCommand.Invoke(sampleWord.Value)).Value;
                }

            }));

            return new TranslateToEnGame(sampleWords);
        }
    }

    public class GameSlide
    {
        private readonly SampleSentence sampleSentence;

        public GameSlide(SampleWord sampleWord)
        {
            sampleSentence = sampleWord.Samples[0];
        }

        public string EeSentence => sampleSentence.EeSentence;

        public Uri ImageUrl => sampleSentence.ImageUrl;

        public string UserTranslation { get; set; }

        public Maybe<ushort> Mark { get; private set; }

        public async Task Submit()
        {
            if (string.Equals(UserTranslation, sampleSentence.EnSentence, StringComparison.InvariantCultureIgnoreCase))
            {
                Mark = 5;
            }

            //....
        }
    }
}
