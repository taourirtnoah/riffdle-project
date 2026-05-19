using System.ComponentModel.DataAnnotations;

namespace Riffdle.Models.ViewModels;

public class SongFormViewModel
{
    public int Id { get; set; }

    [Required]
    [StringLength(200)]
    public string Title { get; set; } = string.Empty;

    [Range(1, 3600)]
    [Display(Name = "Duration (seconds)")]
    public int DurationSeconds { get; set; }

    [Required]
    [Display(Name = "Album")]
    public int AlbumId { get; set; }

    [Required]
    [StringLength(2000)]
    [Display(Name = "Opening lyric")]
    public string OpeningLyric { get; set; } = string.Empty;

    [Display(Name = "Daily quiz song")]
    public bool IsDailyQuizSong { get; set; }

    [Required]
    [Url]
    public string AlbumCoverUrl { get; set; } = string.Empty;

    public string? AlbumTitle { get; set; }
}
