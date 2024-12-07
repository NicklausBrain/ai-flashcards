using CSharpFunctionalExtensions;

using My1kWordsEe.Models;

namespace My1kWordsEe.Services.Cqs
{
    public class ValidateSampleWordCommand
    {
        private readonly OpenAiClient openAiClient;

        public ValidateSampleWordCommand(OpenAiClient openAiClient)
        {
            this.openAiClient = openAiClient;
        }

        public async Task<Result<ValidationResult>> Invoke(SampleWord sample)
        {
            var prompt =
                "Validate understanding of an Estonian word by another model.\n" +
                "You will be given an Estonian word and respective English translations.\n" +
                "Your task is to check if the understanding is correct.\n" +

                $"Your input is JSON object:\n" +
                "```\n{\n" +
                "\"EeWord\": \"<eestikeelne sõna>\", " +
                "\"EnWord\": \"<default english translation>\n" +
                "\"EnExplanation\": \"<explanation of the estonian word in english>\n" +
                "}\n```\n" +
                "Your outpur is JSON object:\n" +
                "```\n{\n" +
                "\"IsValid\": <boolean:true|false>,\n" +
                "\"EnExplanationMessage\": \"<explanation of your judgement>\"\n" +
                "\"EeExplanationMessage\": \"<selgitus oma otsuse kohta>\"\n" +
                "}\n```\n";

            var input =
                "{" +
                " \"EeWord\": \"" + sample.EeWord + "\"," +
                " \"EnWord\": \"" + sample.EnWord + "\"," +
                " \"EnExplanation\": \"" + sample.EnExplanation + "\"" +
                "}";

            var result = await this.openAiClient.CompleteJsonAsync<ValidationResult>(prompt, input);

            return result;
        }

        public class ValidationResult
        {
            public bool IsValid { get; init; }

            public required string EnExplanationMessage { get; init; }

            public required string EeExplanationMessage { get; init; }
        }
    }
}