using System.Text.Json.Serialization;

namespace My1kWordsEe.Models.Grammar
{
    /// <summary>
    /// Sõnaliik (Estonian parts of speech)
    /// </summary>
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum EtPartOfSpeech
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
        /// ehk Adpositsioon (Adposition - Preposition/Postposition)
        /// </summary>
        Kaassõna,

        /// <summary>
        /// ehk Osake (Particle)
        /// </summary>
        Osake
    }
}
