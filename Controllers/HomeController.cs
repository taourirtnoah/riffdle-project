using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Riffdle.Models.Domain;
using Riffdle.Models.Mock;
using Riffdle.Models;
using Riffdle.Models.ViewModels;

namespace Riffdle.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly SongMockRepository _songRepository;

        public HomeController(ILogger<HomeController> logger, SongMockRepository songRepository)
        {
            _logger = logger;
            _songRepository = songRepository;
        }

        public IActionResult Index()
        {
            var dailySong = _songRepository.GetDailyQuizSong();
            var viewModel = BuildDailyQuizViewModel(dailySong, string.Empty, isAnswered: false, isCorrect: false, attemptCount: 0);
            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Index(string guess, int attemptCount = 0)
        {
            var dailySong = _songRepository.GetDailyQuizSong();
            var normalizedGuess = guess?.Trim() ?? string.Empty;
            var isCorrect = !string.IsNullOrWhiteSpace(normalizedGuess) &&
                            dailySong is not null &&
                            string.Equals(normalizedGuess, dailySong.Title, StringComparison.OrdinalIgnoreCase);

            var viewModel = BuildDailyQuizViewModel(
                dailySong,
                normalizedGuess,
                isAnswered: true,
                isCorrect,
                attemptCount + 1);

            viewModel.FeedbackMessage = isCorrect
                ? "Correct. You nailed today's track."
                : "Not quite. The next clue is now unlocked.";

            return View(viewModel);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        private DailyQuizViewModel BuildDailyQuizViewModel(
            Song? dailySong,
            string guess,
            bool isAnswered,
            bool isCorrect,
            int attemptCount)
        {
            ViewData["DailyStreak"] = 6;

            return new DailyQuizViewModel
            {
                DailySong = dailySong,
                Guess = guess,
                IsAnswered = isAnswered,
                IsCorrect = isCorrect,
                AttemptCount = attemptCount
            };
        }
    }
}
