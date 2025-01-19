namespace My1kWordsEe.Models.Games
{
    public class GameComposite
    {
        private readonly object[] gameSlides;

        public ushort CalcResuls()
        {
            // var prog = 100 * ((float)this.Slides.Sum(s => s.CheckResult.Value.Value.Match) / (float)(this.Slides.Length * 5));
            return (ushort)Math.Round(1.1, 0);
        }
        public ushort CurrentSlideIndex { get; private set; } = 0;

        //public GameSlide CurrentSlide => Slides[CurrentSlideIndex];

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
    }
}