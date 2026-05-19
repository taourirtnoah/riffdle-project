using System.ComponentModel.DataAnnotations;

namespace Riffdle.Models.ViewModels;

public class PlaylistFormViewModel
{
    public int Id { get; set; }

    [Required]
    [StringLength(200)]
    public string Name { get; set; } = string.Empty;

    [Required]
    [StringLength(100)]
    [Display(Name = "Owner username")]
    public string OwnerUserName { get; set; } = string.Empty;

    [Required]
    [StringLength(2000)]
    public string Description { get; set; } = string.Empty;

    [Display(Name = "Created at")]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public bool IsPublic { get; set; }

    [Range(0, int.MaxValue)]
    public int Likes { get; set; }
}
