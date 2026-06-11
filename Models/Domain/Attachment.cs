using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Riffdle.Models.Domain;

public class Attachment
{
    [Key]
    public int Id { get; set; }

    public string FileName { get; set; } = string.Empty; // stored filename
    public string OriginalName { get; set; } = string.Empty; // original uploaded name
    public string ContentType { get; set; } = string.Empty;
    public long Size { get; set; }
    public string Url { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // Link to Song (adapted from Lab requirement to tie uploads to an entity)
    [ForeignKey(nameof(Song))]
    public int SongId { get; set; }
    public virtual Song? Song { get; set; }
}
