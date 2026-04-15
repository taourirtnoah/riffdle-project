namespace Riffdle.Models.Domain;

public class Song
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public int DurationSeconds { get; set; }
    public Album Album { get; set; } = new();
    public string OpeningLyric { get; set; } = string.Empty;
    public bool IsDailyQuizSong { get; set; }
}
