using System.Text.RegularExpressions;

namespace My1kWordsEe.Models.Games
{
    /// <summary>
    /// Shared helper for matching and replacing exact word forms in sentences.
    /// Uses Unicode-letter-aware boundaries to correctly handle Estonian text.
    /// </summary>
    public static class WordGrindTextMatcher
    {
        /// <summary>
        /// Checks whether the sentence contains the word as a standalone form
        /// (not embedded inside a larger word).
        /// </summary>
        public static bool ContainsExactWord(string sentence, string word)
        {
            if (string.IsNullOrEmpty(sentence) || string.IsNullOrEmpty(word))
            {
                return false;
            }

            return Regex.IsMatch(sentence, BuildPattern(word), RegexOptions.IgnoreCase);
        }

        /// <summary>
        /// Replaces the exact word form in the sentence with the given replacement string.
        /// Returns the original sentence if no match is found.
        /// </summary>
        public static string ReplaceExactWord(string sentence, string word, string replacement)
        {
            if (string.IsNullOrEmpty(sentence) || string.IsNullOrEmpty(word))
            {
                return sentence;
            }

            return Regex.Replace(sentence, BuildPattern(word), replacement, RegexOptions.IgnoreCase);
        }

        /// <summary>
        /// Builds a Unicode-letter-aware boundary pattern.
        /// Uses negative lookbehind/lookahead for \p{L} so that the word
        /// must not be preceded or followed by any Unicode letter.
        /// </summary>
        private static string BuildPattern(string word)
        {
            return @"(?<!\p{L})" + Regex.Escape(word) + @"(?!\p{L})";
        }
    }
}
