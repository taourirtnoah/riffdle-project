namespace Riffdle.Models.Domain;

public class Hint
{
    public int Id { get; set; }
    public int QuizRoundId { get; set; }
    public QuizRound? QuizRound { get; set; }
    public HintType Type { get; set; }
    public int Order { get; set; }
    public string Content { get; set; } = string.Empty;
}

public enum HintType
{
    AlbumName,
    SongReleaseYear,
    AudioSnippet,
    AlbumCover,
    BandName
}
