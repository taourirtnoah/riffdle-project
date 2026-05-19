using System.ComponentModel.DataAnnotations;

namespace Riffdle.Models.ViewModels;

public class GenreFormViewModel
{
    public int Id { get; set; }

    [Required]
    [StringLength(200)]
    public string Name { get; set; } = string.Empty;

    [Required]
    [StringLength(2000)]
    public string Description { get; set; } = string.Empty;
}
