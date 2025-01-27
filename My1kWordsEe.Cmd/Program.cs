using System.Text.Json;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

using My1kWordsEe.Models;
using My1kWordsEe.Services.Cqs;

namespace My1kWordsEe.Cmd
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            var host = My1kWordsEe.Program.BuildWebHost(new string[] { });

            var getOrAddSampleWordCommand = host.Services.GetRequiredService<GetOrAddSampleWordCommand>();
            var validateSampleWordCommand = host.Services.GetRequiredService<ValidateSampleWordCommand>();
            var redoSampleWordCommand = host.Services.GetRequiredService<RedoSampleWordCommand>();
            var log = host.Services.GetRequiredService<ILoggerFactory>().CreateLogger("cmd");

            var topData = new Dictionary<string, SampleWord>();
            var validationErrors = new List<string>();

            for (var i = 0; i < TopWords.Length; i++)
            {
                var word = TopWords[i];
                var wordSample = await getOrAddSampleWordCommand.Invoke(word);

                if (wordSample.IsFailure)
                {
                    log.LogError($"Failed to get word #{i}. {word} : {wordSample.Error}");
                    break;
                }

                log.LogInformation($"{wordSample.Value.EeWord} = {wordSample.Value.EnWord}");

                var validationResult = await validateSampleWordCommand.Invoke(wordSample.Value);

                if (validationResult.IsFailure)
                {
                    log.LogError($"Failed to get validation result #{i}. {word} : {validationResult.Error}");
                    break;
                }

                if (validationResult.Value.IsValid)
                {
                    log.LogInformation($"#{i}. {word} is ok!");
                    topData[wordSample.Value.EeWord] = wordSample.Value;
                }
                else
                {
                    validationErrors.Add(validationResult.Value.EnExplanationMessage);
                    log.LogError($"#{i}. {word} is not ok! {validationResult.Value.EnExplanationMessage}");

                    var redoCommand = await redoSampleWordCommand.Invoke(word, validationResult.Value.EeExplanationMessage);
                    if (redoCommand.IsFailure)
                    {
                        log.LogError($"#{i}. {word} redo failed! {redoCommand.Error}");
                    }
                    log.LogInformation($"repeat #{i}. {word}");
                    i--;
                }

                await File.WriteAllBytesAsync("./top-data.json", JsonSerializer.SerializeToUtf8Bytes(topData));
                await File.WriteAllBytesAsync("./validation-errors.json", JsonSerializer.SerializeToUtf8Bytes(validationErrors));
            }
        }

        private static readonly string[] TopWords = new string[]
        {
            "ja", "on", "ei", "et", "kui", "oli", "ta", "ka", "see", "ning", "mis", "aga", "ma", "oma", "siis", "või", "nii", "seda", "selle", "kes", "nagu", "kuid", "tema", "pole", "veel", "kas", "mida", "välja", "juba", "võib", "vaid", "nad", "mitte", "kõik", "ole", "nende", "sest", "olid", "sa", "kus", "üle", "ära", "midagi", "pärast", "oleks", "üks", "olnud", "küll", "nüüd", "väga", "ainult", "tuleb", "enam", "need", "vastu", "mille", "olla", "kõige", "meie", "ju", "neid", "kuidas", "eest", "aasta", "palju", "ütles", "saab", "ise", "kohta", "me", "ega", "peale", "sellest", "aastal", "minu", "näiteks", "kogu", "läbi", "seal", "tagasi", "siin", "peab", "rohkem", "polnud", "mulle", "just", "olen", "teda", "kaks", "isegi", "ette", "ajal", "vahel", "mina", "ikka", "siiski", "ehk", "talle", "teha", "enne", "mul", "saa", "saanud", "poolt", "alla", "kokku", "mu", "ühe", "tal", "puhul", "samuti", "sai", "mind", "koos", "osa", "poole", "tuli", "iga", "krooni", "olema", "kuni", "kuigi", "hea", "mees", "praegu", "keegi", "võimalik", "miks", "selles", "eriti", "samas", "kuna", "elu", "peaks", "end", "aastat", "järgi", "korral", "raha", "korda", "juurde", "all", "jaoks", "kohe", "edasi", "suur", "ees", "aega", "sõnul", "juhul", "teine", "lihtsalt", "jäi", "saada", "sama", "maha", "alati", "vaja", "alles", "töö", "seotud", "seega", "olin", "ema", "kunagi", "üles", "muidugi", "tegelikult", "üldse", "enda", "teise", "võiks", "sisse", "jooksul", "hästi", "kaasa", "selleks", "aru", "ilma", "sel", "tea", "mingi", "juures", "pea", "isa", "vähemalt", "inimesed", "kinni", "meil", "aeg", "lisaks", "kelle", "ent", "endale", "sellele", "kolm", "hiljem", "jääb", "te", "ennast", "läks", "kord", "neist", "teada", "sellega", "ümber", "kahe", "uue", "teiste", "öelda", "hakkas", "lõpuks", "liiga", "selline", "pidi", "tõttu", "võtta", "ilmselt", "naine", "inimene", "päris", "antud", "minna", "näha", "olevat", "oled", "jälle", "taha", "hoopis", "suhtes", "varem", "maailma", "läheb", "tegemist", "kindlasti", "mõju", "kaudu", "taga", "neil", "lahti", "jäänud", "asi", "tegi", "peaaegu", "umbes", "mööda", "päeva", "võis", "jah", "suure", "tee", "vana", "neile", "inimese", "pool", "esimene", "vist", "eile", "tõesti", "täna", "võivad", "ütleb", "täiesti", "sinna", "viis", "teinud", "mõni", "tähendab", "riigi", "suurem", "millest", "raske", "tehtud", "teeb", "tulnud", "vähem", "järele", "võttis", "üsna", "aja", "kuhu", "sõna", "teatud", "mingit", "esimese", "uus", "oluline", "igal", "annab", "ometi", "vastavalt", "mõne", "mõned", "kasutada", "teie", "olemas", "tööd", "seetõttu", "läinud", "võrreldes", "valmis", "tabel", "alusel", "inimeste", "osas", "linna", "muud", "kohaselt", "samal", "kõrval", "isiku", "kätte", "mitu", "teised", "asja", "pigem", "maja", "eelkõige", "abil", "oleme", "anda", "tuleks", "joonis", "miljonit", "hakkab", "ongi", "ikkagi", "täis", "sinu", "kolme", "tähelepanu", "sulle", "taas", "alates", "peavad", "sageli", "küsis", "järel", "paar", "väike", "keda", "vähe", "rääkis", "sul", "täpselt", "veidi", "maa", "keele", "õige", "sind", "kuidagi", "seni", "ligi", "jõudnud", "andis", "leida", "poleks", "rääkida", "panna", "hulgas", "sina", "inimest", "kahju", "minema", "parem", "tule", "oluliselt", "vaatas", "õhtul", "sealt", "tavaliselt", "eks", "ringi", "kiiresti", "milles", "kohal", "inimesi", "arvates", "kust", "põhjal", "su", "erinevate", "viimase", "kõigi", "otsa", "saaks", "näinud", "pidada", "arv", "üksnes", "mehed", "saavad", "esitatud", "aastatel", "silmad", "siia", "ühel", "suhteliselt", "millega", "hetkel", "päeval", "paljud", "suurt", "neli", "uuesti", "mehe", "toimub", "natuke", "too", "muidu", "kõiki", "sajandi", "nimetatud", "seejärel", "peal", "maailmas", "pidanud", "viimane", "mõttes", "head", "hommikul", "teist", "võimalus", "meid", "sellist", "mõlemad", "selge", "kell", "huvi", "meile", "küsimus", "asemel", "kõrvale", "loomulikult", "sellepärast", "mil", "vahele", "keeles", "teatas", "käes", "ehkki", "kohale", "terve", "päev", "koju", "tegema", "enamasti", "äkki", "lausa", "lugu", "lapse", "said", "kuu", "õigus", "lapsed", "käigus", "kodus", "otse", "juriidilise", "pisut", "tahab", "pani", "käis", "tahtnud", "nime", "laste", "võtab", "vastas", "tunne", "poiss", "muutunud", "ülikooli", "hakata", "hulka", "auto", "tüdruk", "teisel", "pead", "silma", "sellise", "ühes", "tulla", "president", "kõike", "kasutatakse", "uut", "paremini", "nõnda", "meelde", "toodud", "kindel", "kõrge", "teed", "seas", "justkui", "sees", "nimelt", "seoses", "esimest", "esile", "vaadata", "saadud", "kooli", "teab", "elus", "kaua", "tundus", "leidis", "räägib", "näitab", "süsteemi", "viimasel", "käest", "alguses", "suutnud", "nimi", "hulk", "tänu", "üheks", "tulemused", "veelgi", "tundis", "tihti", "olgu", "lisas", "asju", "protsenti", "tohi", "võtnud", "politsei", "naise", "ravi", "nägu", "peamiselt", "korraga", "no", "aastate", "teel", "mõtles", "laps", "erinevad", "pean", "sellel", "keskmine", "tundub", "algul", "ukse", "sain", "õpetaja", "kasutatud", "riik", "kinnitas", "selgus", "tööle", "üha", "muu", "piisavalt", "firma", "noh", "kohaliku", "suunas", "jõudis", "võetud", "kuus", "rahva", "andnud", "eraldi", "leidnud", "käib", "temaga", "koguni", "alt", "määral", "tekkinud", "jääda", "õiguse", "tuntud", "ühte", "muutus", "nägi", "lugeda", "ütlesin", "temast", "tõepoolest", "nõukogude", "võibolla", "uusi", "sõltub", "toimunud", "tegu", "esineb", "poolest", "kellel", "eri", "võrra", "keelt", "koht", "kedagi", "juttu", "kuulub", "küllap", "suured", "hakkama", "seepärast", "aastast", "hoida", "surma", "tõi", "mõtlesin", "pidevalt", "tahtis", "käinud", "järgmisel", "seisis", "abi", "hakanud", "enamik", "tuua", "puudub", "vaba", "ilus", "eesmärgiks", "ammu", "valitsuse", "tekib", "leiab", "tean", "suurema", "tundi", "naised", "esimesel", "endast", "vajalik", "näeb", "teksti", "tarvis", "arvu", "juhataja", "uute", "käed", "miljoni", "meest", "siit", "varsti", "muutub", "omakorda", "omavahel", "võimaldab", "oleksid", "lausus", "toob", "toimus", "keha", "kellele", "valge", "aluseks", "mõte", "teadis", "pealegi", "istus", "viia", "esialgu", "milline", "koha", "pealt", "käe", "tänavu", "toime", "suuda", "mõnikord", "erinevaid", "saama", "igatahes", "lõpus", "käsi", "tegevuse", "üksi", "jumal", "asjad", "andmed", "peetakse", "millele", "pidas", "tõusis", "ilmus", "teadnud", "noor", "jätta", "ühest", "probleeme", "teiseks", "arvata", "paari", "möödunud", "seejuures", "kõigile", "mõista", "surnud", "eesmärk", "nädala", "muuta", "ajaks", "olukord", "kadunud", "tahan", "öelnud", "piima", "kirja", "kümme", "lõi", "mõelda", "ainus", "tehakse", "nelja", "jäid", "informatsiooni", "väiksem", "lihtne", "hoolimata", "järjest", "autor", "olete", "nemad", "vaatamata", "linnas", "kõigepealt", "pärit", "võimalusi", "tahaks", "üht", "tead", "pikk", "kusjuures", "tõenäoliselt", "kella", "ülejäänud", "korra", "valitsus", "meelest", "hääl", "ajaloo", "kuuluvad", "tagant", "ükski", "millal", "teises", "riigikogu", "vahepeal", "aitab", "nägin", "arvas", "ühiskonna", "oligi", "näe", "luua", "võimalust", "tulevad", "tähtis", "kasutamine", "püsti", "keskmiselt", "uued", "ettevõtte", "loodud", "hinnangul", "mõiste", "oska", "andmete", "kirjutab", "töötajate", "muusika", "osanud", "kasutati", "tehti", "olemasolu", "huvitav", "projekti", "tunda", "selliseid", "meeste", "teisi", "põhjuseks", "mitmeid", "sellised", "riikide", "teevad", "leitud", "erinevalt", "eelmisel", "sõnas", "üldiselt", "keel", "kahjuks", "tunduvalt", "sõnad", "rahvas", "naiste", "mõeldud", "elada", "võeti", "seaduse", "ajas", "probleem", "näitas", "jäävad", "ilmunud", "pähe", "mitmed", "juhtus", "esimees", "arvan", "peaksid", "paraku", "kiire", "andmetel", "sisu", "ühtegi", "astus", "tulid", "avatud", "silmas", "kirjutatud", "nendega", "sündinud", "kusagil", "kuulnud", "mingil", "mõtet", "saan", "vaata", "kirjutas", "ühtlasi", "lähedal", "süü", "vabariigi", "osta", "edaspidi", "organisatsiooni", "asjaolu", "aastas", "ruumi", "oodata", "teil", "suvel", "väikese", "pannud", "vanaema", "linn", "nõuab", "kodu", "niisama", "lõpuni", "oleksin", "millel", "isik", "tahad", "asub", "teid", "korras", "õnneks", "tugev", "endiselt", "valida", "aina", "teile", "kuud", "selgelt", "mängu", "punkti", "elab", "tegevus", "sugugi", "hakkasid", "andmeid", "paistis", "määratud", "nõus", "niisiis", "kuhugi", "olles", "märkis", "maailm", "milleks", "niisugune", "juht", "võimalikult", "tegid", "rolli", "tekkis", "paljude", "püüdis", "juhatuse", "olime", "võimaluse", "paneb", "külge", "anna", "rahul", "vaevalt", "leidub", "maksab", "viga", "järgmise", "pandud", "kerge", "kasvanud", "seekord", "kellegi", "mingeid", "metsa", "sõnu", "nimel", "koolis", "foto", "võime", "analüüsi", "nõukogu", "lasta", "kõigil", "eelmise", "vett", "esimesed", "küsida", "väljas", "sõnade", "aastaid", "vastava", "tõsi", "heaks", "vaatama", "usu", "arvestada", "vara", "miski", "selgub", "tundsin", "lapsi", "suunatud", "poeg", "otsustas", "laua", "pakub", "kohus", "kaheksa", "jõuab", "läksid", "põhjust", "noored", "ääres", "viisil", "täielikult", "väärtus", "teisiti", "autori", "kõne", "kasutades", "ükskõik", "raamatu", "last", "võtma", "kolmas", "vastupidi", "käesoleva", "suu", "käega", "vajab", "töös", "tulemus", "selja", "otsuse", "otsekui", "tingitud", "küllalt", "kevadel", "hind", "seisab", "teistest", "õieti", "arvestades", "hinnata", "liige", "pika", "pere", "mitme", "idee", "sügisel", "kultuuri", "niimoodi", "tüüpi", "kõigest", "arengu", "algab", "tulevikus", "minust", "kasutamise"
        };
    }
}