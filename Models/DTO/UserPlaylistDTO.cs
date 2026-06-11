using System.ComponentModel.DataAnnotations;

namespace Riffdle.Models.DTO;

public class UserPlaylistDTO
{
    public int Id { get; set; }

    [Required]
    [StringLength(200)]
    public string Name { get; set; } = string.Empty;

    [Required]
    [StringLength(100)]
    public string OwnerUserName { get; set; } = string.Empty;

    [Required]
    [StringLength(2000)]
    public string Description { get; set; } = string.Empty;

    public DateTime CreatedAt { get; set; }
    public bool IsPublic { get; set; }

    [Range(0, int.MaxValue)]
    public int Likes { get; set; }

    public List<PlaylistSongDTO> Songs { get; set; } = new();
}
