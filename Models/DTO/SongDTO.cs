using System.ComponentModel.DataAnnotations;

namespace Riffdle.Models.DTO;

public class SongDTO
{
    public int Id { get; set; }

    [Required]
    [StringLength(200)]
    public string Title { get; set; } = string.Empty;

    [Range(1, 3600)]
    public int DurationSeconds { get; set; }

    [Range(1, int.MaxValue)]
    public int AlbumId { get; set; }
    public string AlbumTitle { get; set; } = string.Empty;

    [Required]
    [StringLength(2000)]
    public string OpeningLyric { get; set; } = string.Empty;
    public bool IsDailyQuizSong { get; set; }

    [Required]
    [Url]
    public string AudioSnippetUrl { get; set; } = string.Empty;

    [Required]
    [Url]
    public string AlbumCoverUrl { get; set; } = string.Empty;
}
