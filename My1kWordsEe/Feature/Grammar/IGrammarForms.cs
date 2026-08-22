namespace My1kWordsEe.Feature.Grammar
{
    public interface IGrammarForms
    {
        public TranslatedString PartOfSpeech { get; init; }

        public string BaseForm { get; init; }
    }
}