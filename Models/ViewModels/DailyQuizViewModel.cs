using Riffdle.Models.Domain;

namespace Riffdle.Models.ViewModels;

public class DailyQuizViewModel
{
    public Song? DailySong { get; set; }
    public string Guess { get; set; } = string.Empty;
    public bool IsAnswered { get; set; }
    public bool IsCorrect { get; set; }
    public int AttemptCount { get; set; }
    public string FeedbackMessage { get; set; } = string.Empty;

    public string BandInitials
    {
        get
        {
            var bandName = DailySong?.Album?.Band?.Name;
            if (string.IsNullOrWhiteSpace(bandName))
            {
                return "N/A";
            }

            var initials = bandName
                .Split(' ', StringSplitOptions.RemoveEmptyEntries)
                .Select(part => char.ToUpperInvariant(part[0]));

            return string.Join(string.Empty, initials);
        }
    }

    public int AlbumYear => DailySong?.Album?.ReleaseYear ?? 0;
    public string GenreName => DailySong?.Album?.Band?.Genre?.Name ?? "Unknown";
    public string FirstLyric => DailySong?.OpeningLyric ?? string.Empty;
}
