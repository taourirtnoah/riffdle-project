using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Riffdle.Models.Domain;

public class Song
{
    [Key]
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public int DurationSeconds { get; set; }

    [ForeignKey(nameof(Album))]
    public int AlbumId { get; set; }

    public virtual Album? Album { get; set; }
    public string OpeningLyric { get; set; } = string.Empty;
    public bool IsDailyQuizSong { get; set; }
    public string AudioSnippetUrl { get; set; } = string.Empty;
    public string AlbumCoverUrl { get; set; } = string.Empty;

    public virtual ICollection<PlaylistSong> PlaylistSongs { get; set; } = new List<PlaylistSong>();
    public virtual ICollection<QuizRound> QuizRounds { get; set; } = new List<QuizRound>();
    public virtual ICollection<Attachment> Attachments { get; set; } = new List<Attachment>();
}
