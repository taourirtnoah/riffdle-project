using System.ComponentModel.DataAnnotations;

namespace Riffdle.Models.ViewModels;

public class BandFormViewModel
{
    public int Id { get; set; }

    [Required]
    [StringLength(200)]
    public string Name { get; set; } = string.Empty;

    [Range(1900, 2100)]
    [Display(Name = "Formed Year")]
    public int FormedYear { get; set; }

    [Required]
    [StringLength(100)]
    public string Country { get; set; } = string.Empty;

    [Required]
    [StringLength(2000)]
    public string Description { get; set; } = string.Empty;

    [Required]
    [Display(Name = "Genre")]
    public int GenreId { get; set; }

    public string? GenreName { get; set; }
}
