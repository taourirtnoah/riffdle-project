using System.ComponentModel.DataAnnotations;

namespace Riffdle.Models.DTO;

public class GenreDTO
{
    public int Id { get; set; }

    [Required]
    [StringLength(120)]
    public string Name { get; set; } = string.Empty;

    [StringLength(2000)]
    public string Description { get; set; } = string.Empty;
}
