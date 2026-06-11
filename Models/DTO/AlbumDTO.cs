using System.ComponentModel.DataAnnotations;

namespace Riffdle.Models.DTO;

public class AlbumDTO
{
    public int Id { get; set; }

    [Required]
    [StringLength(200)]
    public string Title { get; set; } = string.Empty;

    [Range(1900, 2100)]
    public int ReleaseYear { get; set; }

    [Range(1, int.MaxValue)]
    public int BandId { get; set; }
    public string BandName { get; set; } = string.Empty;
}
