using System.ComponentModel;

using My1kWordsEe.Models.Grammar.Forms;

namespace My1kWordsEe.Models.Grammar
{
    public struct NounForms : IGrammarForms
    {
        public TranslatedString PartOfSpeech { get; init; }

        public required string BaseForm { get; init; }

        //public required NounForm[] List { get; init; } = Array.Empty<NounForm>();

        [Description("ainsus")]
        public required Dictionary<GrammaticalCase, TranslatedString> Singular { get; init; }

        [Description("mitmus")]
        public required Dictionary<GrammaticalCase, TranslatedString> Plural { get; init; }
    }

    public struct NounForm
    {
        // use Et cases? or translated words?
        public required GrammaticalCase Case { get; init; }

        //// use Et numbers?
        //public required GrammaticalNumber Number { get; init; }

        public required TranslatedString Value { get; init; }
    }
}