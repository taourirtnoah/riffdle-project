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
            var emptyGuessHistory = new List<string>();
            var viewModel = BuildDailyQuizViewModel(
                dailySong,
                string.Empty,
                isAnswered: false,
                isCorrect: false,
                attemptCount: 0,
                emptyGuessHistory,
                BuildGuessHistoryEntries(emptyGuessHistory, dailySong));
            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Index(string guess, int attemptCount = 0, string previousGuesses = "")
        {
            var dailySong = _songRepository.GetDailyQuizSong();
            var normalizedGuess = guess?.Trim() ?? string.Empty;
            var isCorrect = !string.IsNullOrWhiteSpace(normalizedGuess) &&
                            dailySong is not null &&
                            string.Equals(normalizedGuess, dailySong.Title, StringComparison.OrdinalIgnoreCase);

            var newAttemptCount = attemptCount + 1;
            var isLastAttempt = newAttemptCount >= DailyQuizViewModel.MaxAttempts;

            // Parse previous guesses
            var guessHistory = new List<string>();
            if (!string.IsNullOrEmpty(previousGuesses))
            {
                guessHistory = previousGuesses.Split('|', StringSplitOptions.RemoveEmptyEntries).ToList();
            }

            // Add current guess to history
            if (!string.IsNullOrWhiteSpace(normalizedGuess))
            {
                guessHistory.Add(normalizedGuess);
            }

            var guessHistoryEntries = BuildGuessHistoryEntries(guessHistory, dailySong);

            var viewModel = BuildDailyQuizViewModel(
                dailySong,
                normalizedGuess,
                isAnswered: true,
                isCorrect,
                newAttemptCount,
                guessHistory,
                guessHistoryEntries);

            if (isCorrect)
            {
                viewModel.FeedbackMessage = "Correct. You nailed today's track.";
            }
            else if (isLastAttempt)
            {
                viewModel.FeedbackMessage = "No more guesses. Better luck tomorrow!";
            }
            else
            {
                viewModel.FeedbackMessage = "Not quite. The next clue is now unlocked.";
            }

            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult PlayAgain()
        {
            _songRepository.ResetDailyQuizSong();
            return RedirectToAction(nameof(Index));
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
            int attemptCount,
            List<string> previousGuesses,
            List<GuessHistoryEntry> guessHistory)
        {
            ViewData["DailyStreak"] = 6;

            var availableSongs = _songRepository
                .GetAll()
                .Select(song => song.Title)
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .OrderBy(title => title)
                .ToList();

            return new DailyQuizViewModel
            {
                DailySong = dailySong,
                Guess = guess,
                IsAnswered = isAnswered,
                IsCorrect = isCorrect,
                AttemptCount = attemptCount,
                PreviousGuesses = previousGuesses,
                GuessHistory = guessHistory,
                AvailableSongs = availableSongs
            };
        }

        private List<GuessHistoryEntry> BuildGuessHistoryEntries(List<string> previousGuesses, Song? dailySong)
        {
            var entries = new List<GuessHistoryEntry>();
            if (dailySong is null)
            {
                return entries;
            }

            var songsByTitle = _songRepository
                .GetAll()
                .GroupBy(song => song.Title, StringComparer.OrdinalIgnoreCase)
                .ToDictionary(group => group.Key, group => group.First(), StringComparer.OrdinalIgnoreCase);

            foreach (var previousGuess in previousGuesses)
            {
                var outcome = GuessOutcome.Incorrect;

                if (string.Equals(previousGuess, dailySong.Title, StringComparison.OrdinalIgnoreCase))
                {
                    outcome = GuessOutcome.Correct;
                }
                else if (songsByTitle.TryGetValue(previousGuess, out var guessedSong) &&
                         guessedSong.Album.Band.Id == dailySong.Album.Band.Id)
                {
                    outcome = GuessOutcome.SameBand;
                }

                entries.Add(new GuessHistoryEntry
                {
                    Guess = previousGuess,
                    Outcome = outcome
                });
            }

            return entries;
        }
    }
}
