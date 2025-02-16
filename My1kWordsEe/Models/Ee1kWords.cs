using System.Globalization;
using System.Text;

using My1kWordsEe.Models.Semantics;

namespace My1kWordsEe.Models
{
    /// <summary>
    /// Represents a collection of 1k most common Estonian words.
    /// </summary>
    public class Ee1kWords // todo: rename
    {
        private readonly EtWordsCache etWordsCache;

        public Ee1kWords(EtWordsCache etWordsCache)
        {
            this.etWordsCache = etWordsCache;
            this.Search = null;
            this.SelectedWords = etWordsCache.AllWords;
        }

        /// <summary>
        /// Modifies SelectedWords to contain only words that contain the search string.
        /// </summary>
        public Ee1kWords WithSearch(string search)
        {
            if (search.ValidateWord())
            {
                Search = search;
                SelectedWords = etWordsCache.AllWords.Where(w =>
                    etWordsCache.AllWordsDiacriticsFree[w.Value].Contains(search, StringComparison.InvariantCultureIgnoreCase) ||
                    w.DefaultSense.Word.En.Contains(search, StringComparison.InvariantCultureIgnoreCase) ||
                    w.DefaultSense.Word.Et.Contains(search, StringComparison.InvariantCultureIgnoreCase));
            }
            else
            {
                Search = search;
                SelectedWords = etWordsCache.AllWords;
            }
            return this;
        }

        // public Ee1kWords WithSelectedWord(string selectedWord)
        // {
        //     return new Ee1kWords(etWordsCache)
        //     {
        //         Search = this.Search,
        //         SelectedWords = this.SelectedWords
        //     };
        // }

        public IEnumerable<EtWord> SelectedWords { get; private set; }

        public string? Search { get; private set; }
    }
}