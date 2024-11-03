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

        public GameSlide CurrentSlide => Slides[CurrentSlideIndex];

        public bool IsFinished => Slides.Any() && Slides.All(s => s.CheckResult.HasValue);

        public ushort CalcResuls()
        {
            var prog = 100 * ((float)this.Slides.Sum(s => s.CheckResult.Value.Value.Match) / (float)(this.Slides.Length * 5));
            return (ushort)Math.Round(prog, 0);
        }

        public void NextSlide()
        {
            if (CurrentSlideIndex < gameSlides.Length - 1)
            {
                CurrentSlideIndex++;
            }
        }

        public void PrevSlide()
        {
            if (CurrentSlideIndex > 0)
            {
                CurrentSlideIndex--;
            }
        }

        public void GoToSlide(ushort index)
        {
            if (index < gameSlides.Length && index >= 0)
            {
                CurrentSlideIndex = index;
            }
        }

        public static async Task<TranslateToEnGame> Generate(
            GetOrAddSampleWordCommand getOrAddSampleWordCommand,
            AddSampleSentenceCommand addSampleSentenceCommand)
        {
            var rn = new Random(Environment.TickCount);

            var eeWords = new List<SampleWord>
            {
                Ee1kWords.AllWords[rn.Next(0,Ee1kWords.AllWords.Length)],
                Ee1kWords.AllWords[rn.Next(0,Ee1kWords.AllWords.Length)],
                Ee1kWords.AllWords[rn.Next(0,Ee1kWords.AllWords.Length)],
            };

            var sampleWords = await Task.WhenAll(eeWords.AsParallel().Select(async word =>
            {
                var sampleWord = await getOrAddSampleWordCommand.Invoke(word.EeWord);
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
            this.sampleSentence = sampleWord.Samples[0];
        }

        public string EeSentence => sampleSentence.EeSentence;

        public Uri ImageUrl => sampleSentence.ImageUrl;

        public string UserTranslation { get; set; }

        public Maybe<Result<EnTranslationCheckResult>> CheckResult { get; private set; }

        public bool IsCheckInProgress { get; private set; }

        public async Task Submit(CheckEnTranslationCommand checkEnTranslationCommand)
        {
            if (!UserTranslation.ValidateSentence())
            {
                return;
            }

            var userInput = UserTranslation.Trim('.', ' ');
            var defaultEnTranslation = sampleSentence.EnSentence.Trim('.', ' ');

            if (string.Equals(
                userInput,
                defaultEnTranslation,
                StringComparison.InvariantCultureIgnoreCase))
            {
                CheckResult = Result.Success(EnTranslationCheckResult.Success(
                    eeSentence: EeSentence,
                    enSentence: defaultEnTranslation));
            }
            else
            {
                IsCheckInProgress = true;
                CheckResult = await checkEnTranslationCommand.Invoke(
                    eeSentence: EeSentence,
                    enSentence: userInput);
                IsCheckInProgress = false;
            }
        }
    }
}
