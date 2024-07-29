using Microsoft.AspNetCore.Mvc;
using My1kWordsEe.Models;
using System.Diagnostics;

namespace My1kWordsEe.Controllers
{
    public class WordsController : Controller
    {
        private readonly ILogger<WordsController> _logger;

        public WordsController(ILogger<WordsController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index(string search, string selectedWord)
        {
            var words = new Ee1kWords();

            if (!string.IsNullOrWhiteSpace(search))
            {
                words = words.WithSearch(search);
            }

            if (!string.IsNullOrWhiteSpace(selectedWord))
            {
                words = words.WithSelectedWord(selectedWord);
            }

            return View(words);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
