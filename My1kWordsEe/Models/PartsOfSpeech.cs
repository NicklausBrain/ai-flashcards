using System.Text.Json.Serialization;

namespace My1kWordsEe.Models
{
    /// <summary>
    /// Sõnaliik (Estonian parts of speech)
    /// </summary>
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum EePartOfSpeech
    {
        /// <summary>
        /// ehk Substantiiv (Noun)
        /// </summary>
        Nimisõna,

        /// <summary>
        /// ehk Adjektiiv (Adjective)
        /// </summary>
        Omadussõna,

        /// <summary>
        /// ehk Pronoomen (Pronoun)
        /// </summary>
        Asesõna,

        /// <summary>
        /// ehk Numeraal (Numeral)
        /// </summary>
        Arvsõna,

        /// <summary>
        /// ehk Verb (Verb)
        /// </summary>
        Tegusõna,

        /// <summary>
        /// ehk Adverb (Adverb)
        /// </summary>
        Määrsõna,

        /// <summary>
        /// ehk Interjektsioon (Interjection)
        /// </summary>
        Hüüdsõna,

        /// <summary>
        /// ehk Konjunktsioon (Conjunction)
        /// </summary>
        Sidesõna,

        /// <summary>
        /// ehk Adpositsioon (Adposition)
        /// </summary>
        Kaassõna
    }

    /// <summary>
    /// Standard English parts of speech
    /// </summary>
    public enum EnPartOfSpeech
    {
        Noun,
        Adjective,
        Pronoun,
        Numeral,
        Verb,
        Adverb,
        Interjection,
        Conjunction,
        Preposition,
        Article,
        Determiner
    }

    public static class PartsOfSpeechExtensions
    {
        public static EnPartOfSpeech ToEnPartOfSpeech(this EePartOfSpeech eePartOfSpeech)
        {
            switch (eePartOfSpeech)
            {
                case EePartOfSpeech.Nimisõna:
                    return EnPartOfSpeech.Noun;
                case EePartOfSpeech.Omadussõna:
                    return EnPartOfSpeech.Adjective;
                case EePartOfSpeech.Asesõna:
                    return EnPartOfSpeech.Pronoun;
                case EePartOfSpeech.Arvsõna:
                    return EnPartOfSpeech.Numeral;
                case EePartOfSpeech.Tegusõna:
                    return EnPartOfSpeech.Verb;
                case EePartOfSpeech.Määrsõna:
                    return EnPartOfSpeech.Adverb;
                case EePartOfSpeech.Hüüdsõna:
                    return EnPartOfSpeech.Interjection;
                case EePartOfSpeech.Sidesõna:
                    return EnPartOfSpeech.Conjunction;
                case EePartOfSpeech.Kaassõna:
                    return EnPartOfSpeech.Preposition;
                // case EePartOfSpeech.Artikkel:
                //     return EnPartOfSpeech.Article;
                // case EePartOfSpeech.ArtikkelMäärsõna:
                //     return EnPartOfSpeech.Determiner;
                default:
                    throw new ArgumentOutOfRangeException(nameof(eePartOfSpeech), eePartOfSpeech, "Invalid EE part of speech value.");
            }
        }
    }
}

// English Part of Speech,Estonian Part of Speech
// Noun,Nimisõna (Substantiiv)
// Adjective,Omadussõna (Adjektiiv)
// Pronoun,Asesõna (Pronoomen)
// Numeral,Arvsõna (Numeraal)
// Verb,Tegusõna (Verb)
// Adverb,Määrsõna (Adverb)
// Interjection,Hüüdsõna (Interjektsioon)
// Conjunction,Sidesõna (Konjunktsioon)
// Preposition,Kaassõna (Adpositsioon)
// Article,Artikkel (no direct equivalent)
// Determiner,Artikkel / Määrsõna (depending on usage)

