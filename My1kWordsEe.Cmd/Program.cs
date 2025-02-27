using System.Text.Json;
using System.Text.Unicode;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

using My1kWordsEe.Models.Semantics;
using My1kWordsEe.Services.Cqs.Et;

namespace My1kWordsEe.Cmd
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            //await SamplesGenerationProcedure();
            //Console.WriteLine(filesInOrder[0]);
            // await WordsCorrectionProcedure(topWords);

            // Console.WriteLine(AddEtSampleSentenceCommand.Prompt);
            // Console.WriteLine("------------------------------------");
            // Console.WriteLine(JsonSchemaRecord.For(typeof(Models.Semantics.SampleEtSentence)));
            //Console.WriteLine("------------------------------------");
            //Console.WriteLine(JsonSchemaRecord.For(typeof(EtGrammaticalCases)));

            //Console.WriteLine(AddEtSampleSentenceCommand.Prompt);
            //Console.WriteLine(JsonSchemaRecord.For(typeof(SampleEtSentence)));
            //Console.WriteLine(AddEtWordCommand.Prompt);
            //Console.WriteLine(JsonSchemaRecord.For(typeof(WordSenses)));
        }

        public static async Task SamplesGenerationProcedure()
        {
            var host = My1kWordsEe.Program.BuildWebHost(new string[] { });
            var addEtSampleSentenceCommand = host.Services.GetRequiredService<AddEtSampleSentenceCommand>();
            var getEtSampleSentencesQuery = host.Services.GetRequiredService<GetEtSampleSentencesQuery>();

            var topWords = File.ReadAllBytes(@"C:\Users\Nick\source\repos\ai-flashcards\My1kWordsEe\wwwroot\data\et-words.json");
            var etWords = JsonSerializer.Deserialize<EtWord[]>(topWords);

            foreach (var etWord in etWords)
            {
                Console.WriteLine(etWord.DefaultSense.Word.Et);
                var existingSamples = await getEtSampleSentencesQuery.Invoke(etWord, 0);
                if (existingSamples.IsSuccess && existingSamples.Value.Any())
                {
                    continue;
                }

                var sample = await addEtSampleSentenceCommand.Invoke(etWord, 0);
                if (sample.IsFailure)
                {
                    Console.WriteLine(sample.Error);
                }
                else
                {
                    Console.WriteLine(sample.Value.Last().Sentence.Et);
                }
                Console.WriteLine();
            }
        }

        public static async Task WordsCorrectionProcedure(string[] topWords)
        {
            var host = My1kWordsEe.Program.BuildWebHost(new string[] { });

            var getOrAddEtWordCommand = host.Services.GetRequiredService<GetOrAddEtWordCommand>();
            var log = host.Services.GetRequiredService<ILoggerFactory>().CreateLogger("cmd");
            Directory.CreateDirectory("./et-words");
            for (var i = 0; i < topWords.Length; i++)
            {
                var word = topWords[i];
                var etWord = await getOrAddEtWordCommand.Invoke(word);

                if (etWord.IsFailure)
                {
                    log.LogError($"Failed to get word #{i}. {word} : {etWord.Error}");
                    break;
                }

                log.LogInformation($"#{i}: {etWord.Value.DefaultSense.Word.Et} = {etWord.Value.DefaultSense.Word.En}");

                await File.WriteAllBytesAsync($"./et-words/{word}.json", JsonSerializer.SerializeToUtf8Bytes(etWord.Value, options: new JsonSerializerOptions
                {
                    Encoder = System.Text.Encodings.Web.JavaScriptEncoder.Create(UnicodeRanges.All),
                    WriteIndented = false
                }));
            }
        }
    }
}