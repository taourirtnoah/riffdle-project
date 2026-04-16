namespace Riffdle.Models.Domain;

public class Song
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public int DurationSeconds { get; set; }
    public Album Album { get; set; } = new();
    public Band? Band { get; set; }
    public string OpeningLyric { get; set; } = string.Empty;
    public bool IsDailyQuizSong { get; set; }
    public string AudioSnippetUrl { get; set; } = string.Empty;
    public string AlbumCoverUrl { get; set; } = string.Empty;
    public List<PlaylistSong> PlaylistSongs { get; set; } = new();
    public List<QuizRound> QuizRounds { get; set; } = new();
}
