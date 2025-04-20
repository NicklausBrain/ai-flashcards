using My1kWordsEe.Models.Semantics;
using My1kWordsEe.Services.Scoped;

namespace My1kWordsEe.Models
{
    /// <summary>
    /// Represents a collection of 1k most common Estonian words.
    /// </summary>
    public class Et1kWords
    {
        private readonly EtWordsCache etWordsCache;
        private readonly FavoritesStateContainer favoritesStateContainer;

        public Et1kWords(EtWordsCache etWordsCache, FavoritesStateContainer favoritesStateContainer)
        {
            this.etWordsCache = etWordsCache;
            this.favoritesStateContainer = favoritesStateContainer;
            this.Search = null;
            this.SelectedWords = etWordsCache.AllWords;
        }

        /// <summary>
        /// Modifies SelectedWords to contain only words that contain the search string.
        /// </summary>
        public Et1kWords WithSearch(string search, Func<string, bool> isIgnoredWord)
        {
            var allWords = etWordsCache.AllWords.Where(w => !isIgnoredWord(w.Value));

            if (search.ValidateWord())
            {
                Search = search;
                SelectedWords = allWords.Where(w =>
                    etWordsCache.AllWordsDiacriticsFree[w.Value].Contains(search, StringComparison.InvariantCultureIgnoreCase) ||
                    w.DefaultSense.Word.En.Contains(search, StringComparison.InvariantCultureIgnoreCase) ||
                    w.DefaultSense.Word.Et.Contains(search, StringComparison.InvariantCultureIgnoreCase));
            }
            else
            {
                Search = search;
                SelectedWords = allWords;
            }
            return this;
        }

        public IEnumerable<EtWord> SelectedWords { get; private set; }

        public IReadOnlyDictionary<EtWord, int> WordIndex => this.etWordsCache.WordIndex;

        public string? Search { get; private set; }
    }
}