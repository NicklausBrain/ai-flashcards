using System.ComponentModel;
using My1kWordsEe.Models.Grammar.Forms;

namespace My1kWordsEe.Models.Grammar
{
    public struct AdjectiveForms : IGrammarForms
    {
        [Description("Kõneosa")]
        public TranslatedString PartOfSpeech { get; init; }

        [Description("Sõna grammatika põhivorm")]
        public required string BaseForm { get; init; }

        [Description("Algvõre")]
        public required AdjectiveDegreeForms Positive { get; init; }

        [Description("Keskvõre")]
        public required AdjectiveDegreeForms Comparative { get; init; }

        [Description("Ülivõre")]
        public required AdjectiveDegreeForms Superlative { get; init; }

        public static TranslatedString SingularQuestion(EtGrammaticalCase grammaticalCase)
        {
            return grammaticalCase switch
            {
                EtGrammaticalCase.Nimetav => new TranslatedString { Et = "milline?", En = "which?" },
                EtGrammaticalCase.Omastav => new TranslatedString { Et = "millise?", En = "of which?" },
                EtGrammaticalCase.Osastav => new TranslatedString { Et = "millist?", En = "[to] which?" },
                EtGrammaticalCase.Sisseütlev => new TranslatedString { Et = "millisesse?", En = "into which?" },
                EtGrammaticalCase.Seesütlev => new TranslatedString { Et = "millises?", En = "in which?" },
                EtGrammaticalCase.Seestütlev => new TranslatedString { Et = "millisest?", En = "from which?" },
                EtGrammaticalCase.Alaleütlev => new TranslatedString { Et = "millisele?", En = "to which?" },
                EtGrammaticalCase.Alalütlev => new TranslatedString { Et = "millisel?", En = "on which?" },
                EtGrammaticalCase.Alaltütlev => new TranslatedString { Et = "milliselt?", En = "from which?" },
                EtGrammaticalCase.Saav => new TranslatedString { Et = "milliseks?", En = "to which?" },
                EtGrammaticalCase.Rajav => new TranslatedString { Et = "milliseni?", En = "until which?" },
                EtGrammaticalCase.Olev => new TranslatedString { Et = "millisena?", En = "as which?" },
                EtGrammaticalCase.Ilmaütlev => new TranslatedString { Et = "milliseta?", En = "without which?" },
                EtGrammaticalCase.Kaasaütlev => new TranslatedString { Et = "millisega?", En = "with which?" },
                _ => new TranslatedString { Et = "", En = "" }
            };
        }

        public static TranslatedString PluralQuestion(EtGrammaticalCase grammaticalCase)
        {
            return grammaticalCase switch
            {
                EtGrammaticalCase.Nimetav => new TranslatedString { Et = "millised?", En = "which?" },
                EtGrammaticalCase.Omastav => new TranslatedString { Et = "milliste?", En = "of which?" },
                EtGrammaticalCase.Osastav => new TranslatedString { Et = "milliseid?", En = "[to] which?" },
                EtGrammaticalCase.Sisseütlev => new TranslatedString { Et = "millistesse?", En = "into which?" },
                EtGrammaticalCase.Seesütlev => new TranslatedString { Et = "millistes?", En = "in which?" },
                EtGrammaticalCase.Seestütlev => new TranslatedString { Et = "millistest?", En = "from which?" },
                EtGrammaticalCase.Alaleütlev => new TranslatedString { Et = "millistele?", En = "to which?" },
                EtGrammaticalCase.Alalütlev => new TranslatedString { Et = "millistel?", En = "on which?" },
                EtGrammaticalCase.Alaltütlev => new TranslatedString { Et = "millistelt?", En = "from which?" },
                EtGrammaticalCase.Saav => new TranslatedString { Et = "millisteks?", En = "to which?" },
                EtGrammaticalCase.Rajav => new TranslatedString { Et = "millisteni?", En = "until which?" },
                EtGrammaticalCase.Olev => new TranslatedString { Et = "millistena?", En = "as which?" },
                EtGrammaticalCase.Ilmaütlev => new TranslatedString { Et = "millisteta?", En = "without which?" },
                EtGrammaticalCase.Kaasaütlev => new TranslatedString { Et = "millistega?", En = "with which?" },
                _ => new TranslatedString { Et = "", En = "" }
            };
        }
    }

    public struct AdjectiveDegreeForms
    {
        [Description("Ainsus")]
        public required NounForm[] Singular { get; init; }

        [Description("Mitmus")]
        public required NounForm[] Plural { get; init; }
    }
}
