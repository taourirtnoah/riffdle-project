namespace Riffdle.Models.Lab1;

public class Song
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public MetalGenre Genre { get; set; }
    public DateTime ReleaseDate { get; set; }
    public string LyricsSnippet { get; set; } = string.Empty;
    public string AudioSnippetUrl { get; set; } = string.Empty;
    public string AlbumCoverUrl { get; set; } = string.Empty;
    public int DurationSeconds { get; set; }
    public int BandId { get; set; }
    public Band? Band { get; set; }
    public List<QuizRound> QuizRounds { get; set; } = new();
    public List<PlaylistSong> PlaylistSongs { get; set; } = new();
}
