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

        [Description("Eesti keeles 14 käänet (Ainsus)")]
        public required NounForm[] Singular { get; init; }

        [Description("Eesti keeles 14 käänet (Mitmus)")]
        public required NounForm[] Plural { get; init; }
    }

    public struct NounForm
    {
        [Description("Kääne")]
        public required EtGrammaticalCase GrammaticalCase { get; init; }

        [Description("Sõna antud grammatika vormis ja selle otsetõlge")]
        public required TranslatedString CaseForm { get; init; }

        [Description("Küsisõna")]
        public required TranslatedString Question { get; init; }
    }
}