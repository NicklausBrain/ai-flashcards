using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

using My1kWordsEe.Services;
using My1kWordsEe.Services.Cqs;
using My1kWordsEe.Services.Db;

namespace ConsoleApp
{
    internal class Program
    {

        private static readonly string[] Words = new string[]
        {
            "on","et","enam","need","just","mind","end","all","tea","ent","too","said","president","no","last"
        };

        static async Task Main(string[] args)
        {
            using ILoggerFactory factory = LoggerFactory.Create(builder => builder.AddConsole());
            IConfigurationRoot config = new ConfigurationBuilder()
                .AddUserSecrets<Program>()
                .Build();

            var openAiKey = config["Secrets:OpenAiKey"];

            if (string.IsNullOrWhiteSpace(openAiKey))
            {
                throw new ApplicationException("Secrets:OpenAiKey is missing");
            }

            var azureBlobConnectionString = config["Secrets:AzureBlobConnectionString"];

            if (string.IsNullOrWhiteSpace(azureBlobConnectionString))
            {
                throw new ApplicationException("Secrets:AzureBlobConnectionString is missing");
            }

            var blob = new AzureBlobService(azureBlobConnectionString);
            var openAi = new OpenAiService(factory.CreateLogger<OpenAiService>(), openAiKey);
            var ensure = new EnsureWordCommand(blob, openAi);

            var errors = new List<string>();

            Console.WriteLine(Words.Length);
            foreach (var word in Words)
            {
                Console.WriteLine($"Processing {word}");

                var ensureCmd = await ensure.Invoke(word);
                if (ensureCmd.IsFailure)
                {
                    Console.WriteLine($"Failed to ensure {word}: {ensureCmd.Error}");
                    errors.Add($"Failed to ensure {word}: {ensureCmd.Error}");
                }

                var data = await blob.GetWordData(word);

                if (data.IsSuccess)
                {
                    var wordData = data.Value;
                    var update = wordData with
                    {
                        EeAudioUrl = new Uri(
                            $"https://my1kee.blob.core.windows.net/audio/{wordData.EeWord}.wav")
                    };

                    await blob.SaveWordData(update);
                }
                else
                {
                    errors.Add($"Failed to get data for {word}");
                    Console.WriteLine($"Failed to get data for {word}");
                }
            }

            File.WriteAllLines("cmd-errors-3.txt", errors);
        }
    }
}