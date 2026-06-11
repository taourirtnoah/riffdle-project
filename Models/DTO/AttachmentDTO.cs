using System.ComponentModel.DataAnnotations;

namespace Riffdle.Models.DTO;

public class AttachmentDTO
{
    public int Id { get; set; }

    [Range(1, int.MaxValue)]
    public int SongId { get; set; }

    public string SongTitle { get; set; } = string.Empty;

    public int? QuizRoundId { get; set; }

    [Required]
    [StringLength(260)]
    public string FileName { get; set; } = string.Empty;

    [Required]
    [StringLength(260)]
    public string OriginalName { get; set; } = string.Empty;

    [Required]
    [StringLength(120)]
    public string ContentType { get; set; } = string.Empty;

    [Range(0, long.MaxValue)]
    public long Size { get; set; }

    [Required]
    [StringLength(1000)]
    public string Url { get; set; } = string.Empty;

    public DateTime CreatedAt { get; set; }
}
