using My1kWordsEe.Services.Cqs.Et;

namespace My1kWordsEe.Tests.Cqs
{
    public class AddEtWordCommandTests
    {
        // [Fact]
        public void Prompt_Has_No_Drift()
        {
            // unit test to prevent prompt/schema drift
            Assert.Equal(
"See on keeleõppe süsteem\n" +
"Teie väljund on JSON-i massiiv vastavalt järgmisele skeemile:\n" +
"```\n" +
"{\"type\":[\"array\",\"null\"],\"items\":{\"description\":\"A sense or meaning of the Estonian word with respective forms.\",\"type\":[\"object\",\"null\"],\"properties\":{\"Word\":{\"description\":\"The given Estonian word\",\"type\":\"string\"},\"Explanation\":{\"description\":\"Explanation of the word meaning and its given grammar form\",\"type\":\"object\",\"properties\":{\"Et\":{\"description\":\"Translation in Estonian (ISO 639 code: et)\",\"type\":\"string\"},\"En\":{\"description\":\"Translation in English (ISO 639 code: en)\",\"type\":\"string\"}},\"required\":[\"Et\",\"En\"]},\"BaseForm\":{\"description\":\"The base grammar form of the given Estonian word to which prefixes and suffixes can be added.\",\"type\":\"string\"},\"PartOfSpeech\":{\"description\":\"Part of speech, None if cannot be determined\",\"enum\":[\"None\",\"Noun\",\"Adjective\",\"Pronoun\",\"Numeral\",\"Verb\",\"Adverb\",\"Interjection\",\"Conjunction\",\"Preposition\",\"Article\",\"Determiner\"]}},\"required\":[\"Word\",\"Explanation\",\"BaseForm\",\"PartOfSpeech\"]}}\n" +
"```\n" +
"Teie sisend on JSON-objekt vastavalt järgmisele skeemile:\n" +
"```\n" +
"{\"type\":[\"object\",\"null\"],\"properties\":{\"EtWord\":{\"description\":\"Estonian word\",\"type\":\"string\"}},\"required\":[\"EtWord\"]}\n" +
"```",
                AddEtWordCommand.Prompt);
        }
    }
}