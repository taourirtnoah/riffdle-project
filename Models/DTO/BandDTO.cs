using System.ComponentModel.DataAnnotations;

namespace Riffdle.Models.DTO;

public class BandDTO
{
    public int Id { get; set; }

    [Required]
    [StringLength(200)]
    public string Name { get; set; } = string.Empty;

    [Range(1900, 2100)]
    public int FormedYear { get; set; }

    [Required]
    [StringLength(120)]
    public string Country { get; set; } = string.Empty;

    [Required]
    [StringLength(4000)]
    public string Description { get; set; } = string.Empty;

    [Range(1, int.MaxValue)]
    public int GenreId { get; set; }
    public string GenreName { get; set; } = string.Empty;
}
