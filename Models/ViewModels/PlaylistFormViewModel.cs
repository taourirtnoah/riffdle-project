using Riffdle.Models.Domain;

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

    public bool IsPublic { get; set; }

    public List<Song> AvailableSongs { get; set; } = new();

    public List<PlaylistSong> PlaylistSongs { get; set; } = new();

    public DateTime? NewSongAddedAt { get; set; } = DateTime.UtcNow;
}
