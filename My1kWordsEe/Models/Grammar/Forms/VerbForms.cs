using System.Text.Json.Serialization;

namespace My1kWordsEe.Models.Grammar
{
    public class VerbForms
    {
        public PartOfSpeech PartOfSpeech => PartOfSpeech.Verb;
        public required string BaseForm { get; init; }
        public VerbForm[] List { get; init; } = Array.Empty<VerbForm>();
    }

    public record VerbForm
    {
        Pronoun Pronoun { get; init; }
        Tense Tense { get; init; }
        string Value { get; init; }
    }
}