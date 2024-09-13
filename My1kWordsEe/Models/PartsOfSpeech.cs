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
