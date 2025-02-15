using System.ComponentModel;
using System.Text.Json.Serialization;

namespace My1kWordsEe.Models.Grammar
{
    /// <summary>
    /// Grammatical cases in Estonian language
    /// </summary>
    [JsonConverter(typeof(JsonStringEnumConverter))]
    [Description("Kääne. Sisseütlev peab olema pikk vorm -sse.")]
    public enum EtGrammaticalCase
    {
        Nimetav, // (nominatiiv)
        Omastav, // (genitiiv)
        Osastav, // (partitiiv)
        Sisseütlev, // (illatiiv)
        Seesütlev, // (inessiiv)
        Seestütlev, // (elatiiv)
        Alaleütlev, // (allatiiv)
        Alalütlev, // (adessiiv)
        Alaltütlev, // (ablatiiv)
        Saav, // (translatiiv)
        Rajav, // (terminatiiv)
        Olev, // (essiiv)
        Ilmaütlev, // (abessiiv)
        Kaasaütlev, // (komitatiiv)
    }
}