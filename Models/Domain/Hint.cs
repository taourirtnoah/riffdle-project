using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Riffdle.Models.Domain;

public class Hint
{
    [Key]
    public int Id { get; set; }

    [ForeignKey(nameof(QuizRound))]
    public int QuizRoundId { get; set; }

    public virtual QuizRound? QuizRound { get; set; }
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
