﻿using System.Text.RegularExpressions;

namespace My1kWordsEe.Models
{
    public static class Extensions
    {
        // The longest word in Estonian is 43 characters long
        public const int MaxWordLength = 43;

        private static readonly Regex UnicodeWordRegex = new(@"\p{L}+", RegexOptions.Compiled);

        public static bool ValidateWord(this string word)
        {
            if (string.IsNullOrWhiteSpace(word))
            {
                return false;
            }

            if (word.Length > MaxWordLength)
            {
                return false;
            }

            Match m = UnicodeWordRegex.Match(word);

            // We are looking for an exact match, not just a search hit. This matches what
            // the RegularExpressionValidator control does
            return (m.Success && m.Index == 0 && m.Length == word.Length);
        }
    }
}