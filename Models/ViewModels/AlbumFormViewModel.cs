using System.ComponentModel.DataAnnotations;

namespace Riffdle.Models.ViewModels;

public class AlbumFormViewModel
{
    public int Id { get; set; }

    [Required]
    [StringLength(200)]
    public string Title { get; set; } = string.Empty;

    [Range(1900, 2100)]
    [Display(Name = "Release Year")]
    public int ReleaseYear { get; set; }

    [Required]
    [Display(Name = "Band")]
    public int BandId { get; set; }

    // Display helper for edit views
    public string? BandName { get; set; }
}
