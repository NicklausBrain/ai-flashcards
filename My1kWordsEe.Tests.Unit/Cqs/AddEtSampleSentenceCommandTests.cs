using My1kWordsEe.Services.Cqs.Et;

namespace My1kWordsEe.Tests.Cqs
{
    public class AddEtSampleSentenceCommandTests
    {
        // [Fact]
        public void Prompt_Has_No_Drift()
        {
            // unit test to prevent prompt/schema drift
            Assert.Equal(
"Sa oled keeleõppe süsteemi abiline, mis aitab õppida enim levinud eesti keele sõnu.\n" +
"Sisendiks on JSON-objekt, mis kirjeldab eesti keele sõna tähendust ja grammatilist vormi:\n" +
"```\n" +
"{\"description\":\"A sense or meaning of the Estonian word with respective forms.\",\"type\":[\"object\",\"null\"],\"properties\":{\"Word\":{\"description\":\"The given Estonian word\",\"type\":\"string\"},\"Explanation\":{\"description\":\"Explanation of the word meaning and its given grammar form\",\"type\":\"object\",\"properties\":{\"Et\":{\"description\":\"Translation in Estonian (ISO 639 code: et)\",\"type\":\"string\"},\"En\":{\"description\":\"Translation in English (ISO 639 code: en)\",\"type\":\"string\"}},\"required\":[\"Et\",\"En\"]},\"BaseForm\":{\"description\":\"The base grammar form of the given Estonian word to which prefixes and suffixes can be added.\",\"type\":\"string\"},\"PartOfSpeech\":{\"description\":\"Part of speech, None if cannot be determined\",\"enum\":[\"None\",\"Noun\",\"Adjective\",\"Pronoun\",\"Numeral\",\"Verb\",\"Adverb\",\"Interjection\",\"Conjunction\",\"Preposition\",\"Article\",\"Determiner\"]}},\"required\":[\"Word\",\"Explanation\",\"BaseForm\",\"PartOfSpeech\"]}\n" +
"```\n" +
"Teie ülesanne on genereerida JSON-objekt, mis sisaldab näidislauseid eesti ja inglise keeles, kasutades antud sõna sobivas grammatilises vormis:\n" +
"```\n" +
"{\"type\":[\"object\",\"null\"],\"properties\":{\"Sentence\":{\"type\":\"object\",\"properties\":{\"Et\":{\"description\":\"Translation in Estonian (ISO 639 code: et)\",\"type\":\"string\"},\"En\":{\"description\":\"Translation in English (ISO 639 code: en)\",\"type\":\"string\"}},\"required\":[\"Et\",\"En\"]}},\"required\":[\"Sentence\"]}\n" +
"```\n",
                AddEtSampleSentenceCommand.Prompt);
        }
    }
}