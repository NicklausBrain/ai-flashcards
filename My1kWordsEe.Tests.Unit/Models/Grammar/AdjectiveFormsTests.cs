using My1kWordsEe.Models;
using My1kWordsEe.Models.Grammar;
using My1kWordsEe.Models.Grammar.Forms;
using System.Text.Json;
using Xunit;

namespace My1kWordsEe.Tests.Unit.Models.Grammar
{
    public class AdjectiveFormsTests
    {
        [Fact]
        public void AdjectiveForms_CanBeSerializedAndDeserialized()
        {
            var forms = new AdjectiveForms
            {
                BaseForm = "suur",
                Positive = new AdjectiveDegreeForms
                {
                    Singular = new[] { new NounForm { GrammaticalCase = EtGrammaticalCase.Nimetav, CaseForm = new TranslatedString { Et = "suur", En = "big" } } },
                    Plural = new[] { new NounForm { GrammaticalCase = EtGrammaticalCase.Nimetav, CaseForm = new TranslatedString { Et = "suured", En = "big" } } }
                },
                Comparative = new AdjectiveDegreeForms
                {
                    Singular = new[] { new NounForm { GrammaticalCase = EtGrammaticalCase.Nimetav, CaseForm = new TranslatedString { Et = "suurem", En = "bigger" } } },
                    Plural = new[] { new NounForm { GrammaticalCase = EtGrammaticalCase.Nimetav, CaseForm = new TranslatedString { Et = "suuremad", En = "bigger" } } }
                },
                Superlative = new AdjectiveDegreeForms
                {
                    Singular = new[] { new NounForm { GrammaticalCase = EtGrammaticalCase.Nimetav, CaseForm = new TranslatedString { Et = "suurim", En = "biggest" } } },
                    Plural = new[] { new NounForm { GrammaticalCase = EtGrammaticalCase.Nimetav, CaseForm = new TranslatedString { Et = "suurimad", En = "biggest" } } }
                }
            };

            var json = JsonSerializer.Serialize(forms);
            var deserialized = JsonSerializer.Deserialize<AdjectiveForms>(json);

            Assert.Equal("suur", deserialized.BaseForm);
            Assert.Equal("suur", deserialized.Positive.Singular[0].CaseForm.Et);
        }
    }
}
