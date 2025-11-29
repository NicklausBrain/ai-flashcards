using System.Text.RegularExpressions;

namespace My1kWordsEe.Models
{
    public static class Extensions
    {
        /// <summary>
        /// The longest word in Estonian is 43 characters long
        /// </summary>
        public const int MaxWordLength = 43;

        private static readonly Regex UnicodeWordRegex = new(@"(\p{L}|[0-9]|-?)+", RegexOptions.Compiled);

        public static string TrimToLower(this string word)
        {
            return word.Trim().ToLower();
        }

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
            return m.Success && m.Index == 0 && m.Length == word.Length;
        }

        // todo: return Result with the error message explaining the error
        public static bool ValidateSentence(this string sentence)
        {
            if (string.IsNullOrWhiteSpace(sentence))
            {
                return false;
            }

            if (sentence.Length > 1024)
            {
                return false;
            }

            var areWordsValid = sentence
                .Split((char[])[' ', ':', ';', ',', '.', '?', '!', '-', '\''], StringSplitOptions.RemoveEmptyEntries)
                .All(w => w.ValidateWord());

            return areWordsValid;
        }

        public static bool IsRegistrationEnabled(this IConfiguration configuration)
        {
            var isRegistrationEnabled = configuration["IsRegistrationEnabled"];
            if (string.IsNullOrWhiteSpace(isRegistrationEnabled))
            {
                return false;
            }
            return bool.Parse(isRegistrationEnabled);
        }
    }
}