using Microsoft.Extensions.Configuration;

using My1kWordsEe.Services;
using My1kWordsEe.Services.Cqs;
using My1kWordsEe.Services.Db;

namespace ConsoleApp
{
    internal class Program
    {

        private static readonly string[] Words = new string[]
        {
            "on","et","või","mida","nad","ole","sa","midagi","pärast","olnud","küll","tuleb","enam","need","vastu","meie","neid","ütles","me","aastal","minu","seal","tagasi","siin","peab","mulle","just","olen","kaks","mu","mind","koos","osa","tuli","krooni","võimalik","miks","selles","end","korral","raha","all","töö","seega","olin","tegelikult","jooksul","tea","pea","aeg","ent","jääb","kord","oled","asi","suure","tee","ütleb","sinna","tähendab","tulnud","võttis","esimese","igal","mõne","mõned","teie","tööd","võrreldes","valmis","tabel","teised","pigem","oleme","joonis","hakkab","tähelepanu","sageli","väike","keda","õige","kuidagi","andis","rääkida","hulgas","kahju","tule","eks","kiiresti","kust","su","viimase","suurt","uuesti","toimub","too","kõiki","sajandi","seejärel","head","huvi","küsimus","kõrvale","loomulikult","mil","vahele","keeles","said","juriidilise","tahab","võtab","vastas","hulka","tüdruk","president","seas","justkui","sees","elus","kaua","süsteemi","suutnud","nimi","asju","politsei","naise","peamiselt","no","pean","sellel","keskmine","sain","õpetaja","kasutatud","tööle","üha","piisavalt","noh","rahva","käib","alt","määral","muutus","nõukogude","uusi","hakkama","aastast","surma","järgmisel","hakanud","vaba","ilus","tekib","leiab","tundi","naised","teksti","võimaldab","toob","mõte","istus","koha","käe","mõnikord","andmed","peetakse","noor","probleeme","teiseks","arvata","muuta","piima","lõi","nelja","informatsiooni","lihtne","hoolimata","järjest","olete","nemad","võimalusi","üht","ükski","kasutamine","püsti","mõiste","kirjutab","tunda","teevad","sõnad","naiste","juhtus","paraku","kiire","sisu","silmas","kusagil","kuulnud","kirjutas","lähedal","süü","organisatsiooni","oodata","vanaema","linn","kodu","õnneks","tugev","aina","hakkasid","nõus","märkis","tekkis","püüdis","olime","pandud","kerge","sõnu","võime","esimesed","küsida","väljas","heaks","lapsi","laua","pakub","kohus","kaheksa","jõuab","läksid","noored","väärtus","kasutades","last","kolmas","otsuse","arvestades"
        };

        static async Task Main(string[] args)
        {
            var builder = new ConfigurationBuilder();
            IConfigurationRoot config = builder
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
            var openAi = new OpenAiService(openAiKey);
            var ensure = new EnsureWordCommand(blob, openAi);

            var errors = new List<string>();


            Console.WriteLine(Words.Length);
            return;
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

            File.WriteAllLines("cmd-errors.txt", errors);
        }
    }
}