using System.ComponentModel;

using My1kWordsEe.Models.Grammar.Forms;

namespace My1kWordsEe.Models.Grammar
{
    public struct NounForms : IGrammarForms
    {
        [Description("Kõneosa")]
        public TranslatedString PartOfSpeech { get; init; }

        [Description("Sõna grammatika põhivorm (nt nimisõna ainsuse nimetav)")]
        public required string BaseForm { get; init; }

        [Description("Ainsuse vormid")]
        public required Dictionary<EtGrammaticalCase, TranslatedString> Singular { get; init; }

        [Description("Mitmuse vormid")]
        public required Dictionary<EtGrammaticalCase, TranslatedString> Plural { get; init; }
    }
}