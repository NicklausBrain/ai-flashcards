using System.Text.Json;
using System.Text.Unicode;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

using My1kWordsEe.Models;
using My1kWordsEe.Models.Grammar;
using My1kWordsEe.Models.Semantics;
using My1kWordsEe.Services.Cqs;
using My1kWordsEe.Services.Cqs.Et;

namespace My1kWordsEe.Cmd
{
    internal class Program
    {
        static async Task Main(string[] args)
        {

            //Console.WriteLine(filesInOrder[0]);
            //var topWords = System.IO.File.ReadAllLines("/Users/nick/Desktop/Merged_Estonian_Vocabulary.csv");
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