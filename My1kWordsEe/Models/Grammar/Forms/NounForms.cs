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

        [Description(
@"Eesti keeles 14 käänet, Ainsus
Nimetav,mis?
Omastav,kelle? mille?
Osastav,keda? mida?
Sisseütlev,kellesse? millesse?
Seesütlev,kelles? milles?
Seestütlev,kellest? millest?
Alaleütlev,kellele? millele?
Alalütlev,kellel? millel?
Alaltütlev,kellelt? millelt?
Saav,kelleks? milleks?
Rajav,kelleni? milleni?
Olev,kellena? millena?
Ilmaütlev,kelleta? milleta?
Kaasaütlev,kellega? millega?")]
        public required NounForm[] Singular { get; init; }

        [Description(
@"Eesti keeles 14 käänet, Mitmus
Kääne,Küsisõna
Nimetav,millised?
Omastav,kelle? mille?
Osastav,keda? mida?
Sisseütlev,kellessse? millesse?
Seesütlev,kelless? milles?
Seestütlev,kellest? millest?
Alaleütlev,kellele? millele?
Alalütlev,kellel? millel?
Alaltütlev,kellelt? millelt?
Saav,kelledeks? milledeks?
Rajav,kelledeni? milledeni?
Olev,kelledena? milledena?
Ilmaütlev,kelledeta? milledeta?
Kaasaütlev,kellega? millega?")]
        public required NounForm[] Plural { get; init; }
    }

    public struct NounForm
    {
        [Description("Kääne")]
        public required EtGrammaticalCase GrammaticalCase { get; init; }

        [Description("Sõna antud grammatika vormis ja selle otsetõlge")]
        public required TranslatedString CaseForm { get; init; }

        public static TranslatedString SingularQuestion(EtGrammaticalCase grammaticalCase)
        {
            return grammaticalCase switch
            {
                EtGrammaticalCase.Nimetav => new TranslatedString { Et = "kes? mis?", En = "who? what?" },
                EtGrammaticalCase.Omastav => new TranslatedString { Et = "kelle? mille?", En = "whose? of what?" },
                EtGrammaticalCase.Osastav => new TranslatedString { Et = "keda? mida?", En = "whom? what?" },
                EtGrammaticalCase.Sisseütlev => new TranslatedString { Et = "kellesse? millesse?", En = "into whom? into what?" },
                EtGrammaticalCase.Seesütlev => new TranslatedString { Et = "kelles? milles?", En = "in whom? in what?" },
                EtGrammaticalCase.Seestütlev => new TranslatedString { Et = "kellest? millest?", En = "from whom? from what?" },
                EtGrammaticalCase.Alaleütlev => new TranslatedString { Et = "kellele? millele?", En = "to whom? to what?" },
                EtGrammaticalCase.Alalütlev => new TranslatedString { Et = "kellel? millel?", En = "on whom? on what?" },
                EtGrammaticalCase.Alaltütlev => new TranslatedString { Et = "kellelt? millelt?", En = "from whom? from what?" },
                EtGrammaticalCase.Saav => new TranslatedString { Et = "kelleks? milleks?", En = "into whom? into what?" },
                EtGrammaticalCase.Rajav => new TranslatedString { Et = "kelleni? milleni?", En = "until whom? until what?" },
                EtGrammaticalCase.Olev => new TranslatedString { Et = "kellena? millena?", En = "as whom? as what?" },
                EtGrammaticalCase.Ilmaütlev => new TranslatedString { Et = "kelleta? milleta?", En = "without whom? without what?" },
                EtGrammaticalCase.Kaasaütlev => new TranslatedString { Et = "kellega? millega?", En = "with whom? with what?" },
                _ => new TranslatedString { Et = "", En = "" }
            };
        }

        public static TranslatedString PluralQuestion(EtGrammaticalCase grammaticalCase)
        {
            return grammaticalCase switch
            {
                EtGrammaticalCase.Nimetav => new TranslatedString { Et = "kes? mis?", En = "who? what?" },
                EtGrammaticalCase.Omastav => new TranslatedString { Et = "kelle? mille?", En = "whose? of what?" },
                EtGrammaticalCase.Osastav => new TranslatedString { Et = "keda? mida?", En = "whom? what?" },
                EtGrammaticalCase.Sisseütlev => new TranslatedString { Et = "kellessse? millesse?", En = "into whom? into what?" },
                EtGrammaticalCase.Seesütlev => new TranslatedString { Et = "kelless? milles?", En = "in whom? in what?" },
                EtGrammaticalCase.Seestütlev => new TranslatedString { Et = "kellest? millest?", En = "from whom? from what?" },
                EtGrammaticalCase.Alaleütlev => new TranslatedString { Et = "kellele? millele?", En = "to whom? to what?" },
                EtGrammaticalCase.Alalütlev => new TranslatedString { Et = "kellel? millel?", En = "on whom? on what?" },
                EtGrammaticalCase.Alaltütlev => new TranslatedString { Et = "kellelt? millelt?", En = "from whom? from what?" },
                EtGrammaticalCase.Saav => new TranslatedString { Et = "kelledeks? milledeks?", En = "into whom? into what?" },
                EtGrammaticalCase.Rajav => new TranslatedString { Et = "kelledeni? milledeni?", En = "until whom? until what?" },
                EtGrammaticalCase.Olev => new TranslatedString { Et = "kelledena? milledena?", En = "as whom? as what?" },
                EtGrammaticalCase.Ilmaütlev => new TranslatedString { Et = "kelledeta? milledeta?", En = "without whom? without what?" },
                EtGrammaticalCase.Kaasaütlev => new TranslatedString { Et = "kellega? millega?", En = "with whom? with what?" },
                _ => new TranslatedString { Et = "", En = "" }
            };
        }

    }
}
